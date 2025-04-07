using System.Collections.Concurrent;
using System.Diagnostics;
using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.MediaLibrary;
using CMS.Websites;
using CMS.Websites.Internal;
using Kentico.Xperience.UMT.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Builders;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.Model;
using Migration.Tool.Common.Services;
using Migration.Tool.KXP.Api.Auxiliary;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source.Auxiliary;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Helpers;
using Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Providers;
using Migration.Tool.Source.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Mappers;

public record CmsTreeMapperSource(
    ICmsTree CmsTree,
    string SafeNodeName,
    Guid SiteGuid,
    Guid? NodeParentGuid,
    Dictionary<string, Guid> CultureToLanguageGuid,
    string? TargetFormDefinition,
    string SourceFormDefinition,
    List<ICmsDocument> MigratedDocuments,
    ICmsSite SourceSite,
    bool Deferred
);

public record CustomTableMapperSource(
    string? TargetFormDefinition,
    string SourceFormDefinition,
    Guid ContentItemGuid,
    ICmsClass SourceClass,
    Dictionary<string, object?> Values,
    IClassMapping ClassMapping
);

public class ContentItemMapper(
    ILogger<ContentItemMapper> logger,
    CoupledDataService coupledDataService,
    IAttachmentMigrator attachmentMigrator,
    CmsRelationshipService relationshipService,
    FieldMigrationService fieldMigrationService,
    ModelFacade modelFacade,
    ReusableSchemaService reusableSchemaService,
    DeferredPathService deferredPathService,
    DeferredTreeNodesService deferredTreeNodesService,
    SpoiledGuidContext spoiledGuidContext,
    IAssetFacade assetFacade,
    MediaLinkServiceFactory mediaLinkServiceFactory,
    ToolConfiguration configuration,
    ClassMappingProvider classMappingProvider,
    VisualBuilderPatcher visualBuilderPatcher,
    IServiceProvider serviceProvider,
    IEnumerable<ContentItemDirectorBase> directors,
    ContentFolderService contentFolderService
    ) : UmtMapperBase<CmsTreeMapperSource>, IUmtMapper<CustomTableMapperSource>
{
    private const string CLASS_FIELD_CONTROL_NAME = "controlname";

    protected override IEnumerable<IUmtModel> MapInternal(CmsTreeMapperSource source)
    {
        (var cmsTree, string safeNodeName, var siteGuid, var nodeParentGuid, var cultureToLanguageGuid, string? targetFormDefinition, string sourceFormDefinition, var migratedDocuments, var sourceSite, bool deferred) = source;

        logger.LogTrace("Mapping {Value}", new { cmsTree.NodeAliasPath, cmsTree.NodeName, cmsTree.NodeGUID, cmsTree.NodeSiteID });

        var sourceNodeClass = modelFacade.SelectById<ICmsClass>(cmsTree.NodeClassID) ?? throw new InvalidOperationException($"Fatal: node class is missing, class id '{cmsTree.NodeClassID}'");
        var mapping = classMappingProvider.GetMapping(sourceNodeClass.ClassName);
        var targetClassGuid = sourceNodeClass.ClassGUID;
        var targetClassInfo = DataClassInfoProvider.ProviderObject.Get(sourceNodeClass.ClassName);
        if (mapping is not null)
        {
            targetClassInfo = DataClassInfoProvider.ProviderObject.Get(mapping.TargetClassName) ?? throw new InvalidOperationException($"Unable to find target class '{mapping.TargetClassName}'");
            targetClassGuid = targetClassInfo.ClassGUID;
        }

        if (targetClassInfo is null)
        {
            logger.LogError("Could not map content item. Target class DataClassInfo ClassGUID={ClassGUID} not found.", targetClassGuid);
            yield break;
        }

        var formerUrlPaths = GetFormerUrlPaths(cmsTree);

        var directive = GetDirective(new ContentItemSource(cmsTree, sourceNodeClass.ClassName, mapping?.TargetClassName ?? sourceNodeClass.ClassName, sourceSite, formerUrlPaths));
        yield return directive;

        if (directive is DropDirective)
        {
            logger.LogInformation("Content item skipped. Reason: {Explicit drop directive} NodeGUID: {NodeGUID} NodeAliasPath: {NodeAliasPath}", "Explicit drop directive", cmsTree.NodeGUID, cmsTree.NodeAliasPath);
            yield break;
        }
        else if (directive is ConvertToWidgetDirective && cmsTree.NodeHasChildren == true && !deferred)
        {
            deferredTreeNodesService.AddNode(cmsTree);
            yield break;
        }

        bool migratedAsContentFolder = sourceNodeClass.ClassName.Equals("cms.folder", StringComparison.InvariantCultureIgnoreCase) && !configuration.UseDeprecatedFolderPageType.GetValueOrDefault(false);

        var contentItemGuid = spoiledGuidContext.EnsureNodeGuid(cmsTree.NodeGUID, cmsTree.NodeSiteID, cmsTree.NodeID);
        bool isMappedTypeReusable = targetClassInfo?.ClassContentTypeType is ClassContentTypeType.REUSABLE || configuration.ClassNamesConvertToContentHub.Contains(sourceNodeClass.ClassName);
        if (isMappedTypeReusable)
        {
            logger.LogTrace("Target is reusable {Info}", new { cmsTree.NodeAliasPath, targetClassInfo?.ClassName });
        }

        bool storeContentItem = !(directive is ConvertToWidgetDirective ctw && !ctw.WrapInReusableItem);

        Guid? contentFolderGuid = isMappedTypeReusable
            ? directive.ContentFolderOptions switch
            {
                null => null,
                { Guid: { } guid } => guid,
                { DisplayNamePath: { } displayNamePath } => contentFolderService.EnsureStandardFolderStructure(siteGuid.ToString(), displayNamePath).GetAwaiter().GetResult(),
                _ => throw new InvalidOperationException($"{nameof(ContentFolderOptions)} has neither {nameof(ContentFolderOptions.Guid)} nor {nameof(ContentFolderOptions.DisplayNamePath)} specified")
            }
            : null;

        var contentItemModel = new ContentItemModel
        {
            ContentItemGUID = contentItemGuid,
            ContentItemName = safeNodeName,
            ContentItemIsReusable = isMappedTypeReusable,
            ContentItemIsSecured = cmsTree.IsSecuredNode ?? false,
            ContentItemDataClassGuid = migratedAsContentFolder ? null : targetClassGuid,
            ContentItemChannelGuid = isMappedTypeReusable ? null : siteGuid,
            ContentItemContentFolderGUID = contentFolderGuid,
        };
        if (storeContentItem)
        {
            yield return contentItemModel;
        }

        var targetWebPage = WebPageItemInfo.Provider.Get()
            .WhereEquals(nameof(WebPageItemInfo.WebPageItemGUID), contentItemGuid)
            .FirstOrDefault();
        string? treePath = targetWebPage?.WebPageItemTreePath;

        var websiteChannelInfo = WebsiteChannelInfoProvider.ProviderObject.Get(siteGuid);
        var treePathConvertor = TreePathConvertor.GetSiteConverter(websiteChannelInfo.WebsiteChannelID);
        if (treePath == null)
        {
            (bool treePathIsDifferent, treePath) = treePathConvertor.ConvertAndEnsureUniqueness(cmsTree.NodeAliasPath).GetAwaiter().GetResult();
            if (treePathIsDifferent)
            {
                logger.LogInformation($"Original node alias path '{cmsTree.NodeAliasPath}' of '{cmsTree.NodeName}' item was converted to '{treePath}' since the value does not allow original range of allowed characters.");
            }
        }

        foreach (var cmsDocument in migratedDocuments)
        {
            if (!cultureToLanguageGuid.TryGetValue(cmsDocument.DocumentCulture, out var languageGuid))
            {
                logger.LogWarning("Document '{DocumentGUID}' was skipped, unknown culture", cmsDocument.DocumentGUID);
                continue;
            }

            bool hasDraft = cmsDocument.DocumentPublishedVersionHistoryID is not null &&
                            cmsDocument.DocumentPublishedVersionHistoryID != cmsDocument.DocumentCheckedOutVersionHistoryID;

            var checkoutVersion = hasDraft
                ? modelFacade.SelectById<ICmsVersionHistory>(cmsDocument.DocumentCheckedOutVersionHistoryID)
                : null;

            bool draftMigrated = false;
            if (checkoutVersion is { PublishFrom: null } draftVersion && !migratedAsContentFolder)
            {
                List<IUmtModel>? migratedDraft = null;
                try
                {
                    migratedDraft = MigrateDraft(draftVersion, cmsTree, sourceFormDefinition, targetFormDefinition!, contentItemGuid, languageGuid, sourceNodeClass, websiteChannelInfo, sourceSite, mapping).ToList();
                    draftMigrated = true;
                }
                catch
                {
                    logger.LogWarning("Failed to migrate checkout version of document with DocumentID={CmsDocumentDocumentId} VersionHistoryID={CmsDocumentDocumentCheckedOutVersionHistoryId}",
                        cmsDocument.DocumentID, cmsDocument.DocumentCheckedOutVersionHistoryID);
                    draftMigrated = false;
                }

                if (migratedDraft != null)
                {
                    foreach (var umtModel in migratedDraft)
                    {
                        yield return umtModel;
                    }
                }
            }

            var versionStatus = cmsDocument switch
            {
                { DocumentIsArchived: true } => VersionStatus.Unpublished,
                { DocumentPublishedVersionHistoryID: null, DocumentCheckedOutVersionHistoryID: null } => VersionStatus.Published,
                { DocumentPublishedVersionHistoryID: { } pubId, DocumentCheckedOutVersionHistoryID: { } chId } when pubId <= chId => VersionStatus.Published,
                { DocumentPublishedVersionHistoryID: null, DocumentCheckedOutVersionHistoryID: not null } => VersionStatus.InitialDraft,
                _ => draftMigrated ? VersionStatus.Published : VersionStatus.InitialDraft
            };
            if (migratedAsContentFolder)
            {
                versionStatus = VersionStatus.Published; // folder is automatically published
            }

            DateTime? scheduledPublishWhen = null;
            DateTime? scheduleUnpublishWhen = null;
            string? contentItemCommonDataVisualBuilderWidgets = null;
            string? contentItemCommonDataVisualBuilderTemplateConfiguration = null;

            bool ndp = false;
            if (!migratedAsContentFolder)
            {
                if (cmsDocument.DocumentPublishFrom is { } publishFrom)
                {
                    if (versionStatus == VersionStatus.Published)
                    {
                        versionStatus = VersionStatus.InitialDraft;
                    }
                    scheduledPublishWhen = publishFrom;
                }

                if (cmsDocument.DocumentPublishTo is { } publishTo)
                {
                    if (versionStatus == VersionStatus.Published)
                    {
                        scheduleUnpublishWhen = publishTo;
                    }
                }

                switch (cmsDocument)
                {
                    case CmsDocumentK11:
                    {
                        break;
                    }
                    case CmsDocumentK12 doc:
                    {
                        contentItemCommonDataVisualBuilderWidgets = doc.DocumentPageBuilderWidgets;
                        contentItemCommonDataVisualBuilderTemplateConfiguration = doc.DocumentPageTemplateConfiguration;
                        break;
                    }
                    case CmsDocumentK13 doc:
                    {
                        contentItemCommonDataVisualBuilderWidgets = doc.DocumentPageBuilderWidgets;
                        contentItemCommonDataVisualBuilderTemplateConfiguration = doc.DocumentPageTemplateConfiguration;
                        break;
                    }

                    default:
                        break;
                }

                (contentItemCommonDataVisualBuilderTemplateConfiguration, contentItemCommonDataVisualBuilderWidgets, ndp) = visualBuilderPatcher.PatchJsonDefinitions(source.CmsTree.NodeSiteID, contentItemCommonDataVisualBuilderTemplateConfiguration, contentItemCommonDataVisualBuilderWidgets).GetAwaiter().GetResult();
            }

            if (directive.PageTemplateIdentifier is not null)
            {
                contentItemCommonDataVisualBuilderTemplateConfiguration = JsonConvert.SerializeObject(new PageTemplateConfiguration { Identifier = directive.PageTemplateIdentifier, Properties = directive.PageTemplateProperties });
            }

            var documentGuid = spoiledGuidContext.EnsureDocumentGuid(
                cmsDocument.DocumentGUID ?? throw new InvalidOperationException("DocumentGUID is null"),
                cmsTree.NodeSiteID,
                cmsTree.NodeID,
                cmsDocument.DocumentID
            );

            var commonDataModel = new ContentItemCommonDataModel
            {
                ContentItemCommonDataGUID = documentGuid,
                ContentItemCommonDataContentItemGuid = contentItemGuid,
                ContentItemCommonDataContentLanguageGuid = languageGuid, // DocumentCulture -> language entity needs to be created and its ID used here
                ContentItemCommonDataVersionStatus = versionStatus,
                ContentItemCommonDataIsLatest = !draftMigrated, // Flag for latest record to know what to retrieve for the UI
                ContentItemCommonDataVisualBuilderWidgets = contentItemCommonDataVisualBuilderWidgets,
                ContentItemCommonDataVisualBuilderTemplateConfiguration = contentItemCommonDataVisualBuilderTemplateConfiguration,
            };

            if (ndp)
            {
                deferredPathService.AddPatch(
                    commonDataModel.ContentItemCommonDataGUID ?? throw new InvalidOperationException("DocumentGUID is null"),
                    ContentItemCommonDataInfo.TYPEINFO.ObjectClassName,
                    websiteChannelInfo.WebsiteChannelID
                );
            }

            if (!migratedAsContentFolder)
            {
                var dataModel = new ContentItemDataModel { ContentItemDataGUID = commonDataModel.ContentItemCommonDataGUID, ContentItemDataCommonDataGuid = commonDataModel.ContentItemCommonDataGUID, ContentItemContentTypeName = mapping?.TargetClassName ?? sourceNodeClass.ClassName };

                var fi = new FormInfo(targetFormDefinition);
                if (sourceNodeClass.ClassIsCoupledClass)
                {
                    var sfi = new FormInfo(sourceFormDefinition);
                    string primaryKeyName = "";
                    foreach (var sourceFieldInfo in sfi.GetFields(true, true))
                    {
                        if (sourceFieldInfo.PrimaryKey)
                        {
                            primaryKeyName = sourceFieldInfo.Name;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(primaryKeyName))
                    {
                        throw new Exception("Error, unable to find coupled data primary key");
                    }

                    FormFieldInfo[] commonFields = UnpackReusableFieldSchemas(fi.GetFields<FormSchemaInfo>()).ToArray();
                    List<string> targetColumns = commonFields
                        .Select(cf => ReusableSchemaService.RemoveClassPrefix(sourceNodeClass.ClassName, cf.Name))
                        .Union(fi.GetColumnNames(false))
                        .Except(CmsClassMapper.GetLegacyMetadataFields(modelFacade.SelectVersion(), configuration.IncludeExtendedMetadata.GetValueOrDefault(false))
                        .Select(x => CmsClassMapper.GetMappedLegacyField(fi, targetClassInfo!.ClassName, x.LegacyFieldName)))
                        .Where(x => x is not null)
                        .Select(x => x!)
                        .ToList();

                    var coupledDataRow = coupledDataService.GetSourceCoupledDataRow(sourceNodeClass.ClassTableName!, primaryKeyName, cmsDocument.DocumentForeignKeyValue);
                    var sourceObjectContext = new DocumentSourceObjectContext(cmsTree, sourceNodeClass, sourceSite, sfi, fi, cmsDocument.DocumentID);
                    var convertorContext = new ConvertorTreeNodeContext(cmsTree.NodeGUID, cmsTree.NodeSiteID, cmsDocument.DocumentID, false);
                    // TODO tomas.krch: 2024-09-05 propagate async to root
                    MapCoupledDataFieldValues(dataModel.CustomProperties,
                        columnName => coupledDataRow?[columnName],
                        columnName => coupledDataRow?.ContainsKey(columnName) ?? false,
                        //  cmsTree, cmsDocument.DocumentID,
                        targetColumns, sfi, fi,
                        false, sourceNodeClass,
                        // sourceSite, 
                        mapping,
                        sourceObjectContext,
                        convertorContext,
                        targetClassInfo!.ClassName
                    ).GetAwaiter().GetResult();

                    var refFields = fi.GetFields<FormFieldInfo>().Concat(commonFields).Where(x => x.DataType == "contentitemreference");
                    foreach (var refField in refFields)
                    {
                        if (dataModel.CustomProperties.TryGetValue(refField.Name, out var serializedValue) || commonDataModel.CustomProperties.TryGetValue(refField.Name, out serializedValue))
                        {
                            if (serializedValue is string valueString && !string.IsNullOrEmpty(valueString))
                            {
                                var value = JToken.Parse(valueString);
                                foreach (var targetGuid in value.Select(x => new Guid(x["Identifier"]!.Value<string>()!)).ToArray())
                                {
                                    if (ContentItemInfo.Provider.Get(targetGuid) is null)
                                    {
                                        // If the referenced object does not exist yet, create a placeholder
                                        yield return new ContentItemModel
                                        {
                                            ContentItemDataClassGuid = DataClassInfoProvider.GetDataClassInfo("CMS.ContentItemCommonData").ClassGUID,
                                            ContentItemGUID = targetGuid,
                                            ContentItemName = $"{targetGuid}",
                                            ContentItemIsReusable = true
                                        };
                                    }
                                }
                            }
                        }
                    }

                    foreach (var formFieldInfo in commonFields)
                    {
                        string originalFieldName = ReusableSchemaService.RemoveClassPrefix(sourceNodeClass.ClassName, formFieldInfo.Name);
                        if (dataModel.CustomProperties.TryGetValue(originalFieldName, out object? value))
                        {
                            commonDataModel.CustomProperties ??= [];
                            logger.LogTrace("Reusable schema field '{FieldName}' from schema '{SchemaGuid}' populated", formFieldInfo.Name, formFieldInfo.Properties[ReusableFieldSchemaConstants.SCHEMA_IDENTIFIER_KEY]);
                            commonDataModel.CustomProperties[formFieldInfo.Name] = value;
                            dataModel.CustomProperties.Remove(originalFieldName);
                        }
                        else
                        {
                            logger.LogTrace("Reusable schema field '{FieldName}' from schema '{SchemaGuid}' missing", formFieldInfo.Name, formFieldInfo.Properties[ReusableFieldSchemaConstants.SCHEMA_IDENTIFIER_KEY]);
                        }
                    }
                }

                string targetClassName = mapping?.TargetClassName ?? sourceNodeClass.ClassName;
                foreach (var legacyField in CmsClassMapper.GetLegacyMetadataFields(modelFacade.SelectVersion(), configuration.IncludeExtendedMetadata.GetValueOrDefault(false)))
                {
                    if (CmsClassMapper.GetMappedLegacyField(fi, targetClassInfo!.ClassName, legacyField.LegacyFieldName) is { } legacyDocumentNameFieldName)
                    {
                        if (reusableSchemaService.IsConversionToReusableFieldSchemaRequested(targetClassName))
                        {
                            string fieldName = ReusableSchemaService.GetUniqueFieldName(targetClassName, legacyDocumentNameFieldName);
                            commonDataModel.CustomProperties.Add(fieldName, legacyField.GetValue(cmsDocument));
                        }
                        else
                        {
                            dataModel.CustomProperties.Add(legacyDocumentNameFieldName, legacyField.GetValue(cmsDocument));
                        }
                    }
                }

                if (storeContentItem)
                {
                    yield return commonDataModel;
                    yield return dataModel;
                }

                if (directive is ConvertToWidgetDirective widgetDirective)
                {
                    // locate ancestor host page
                    var hostNode = cmsTree;
                    for (int i = 0; i < -widgetDirective.ParentLevel; i++)
                    {
                        hostNode = modelFacade.Select<ICmsTree>("NodeID = @nodeID", "NodeOrder", new SqlParameter("nodeID", hostNode.NodeParentID)).First();
                    }
                    var hostWebPageItem = WebPageItemInfo.Provider.Get(hostNode.NodeGUID) ?? throw new Exception("During conversion of ContentItem GUID={ContentItemGuid} to widget, host page was resolved to one with GUID={HostPageGuid}, but no such page was found");

                    // load host page common data
                    var languageInfo = ContentLanguageInfo.Provider.Get().WhereEquals(nameof(ContentLanguageInfo.ContentLanguageGUID), languageGuid).First();
                    var hostPageCommonDataInfo = ContentItemCommonDataInfo.Provider.Get().WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataContentItemID), hostWebPageItem.WebPageItemContentItemID).And()
                        .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataContentLanguageID), languageInfo.ContentLanguageID).First();
                    var hostPageContentItemInfo = ContentItemInfo.Provider.Get(hostPageCommonDataInfo.ContentItemCommonDataContentItemID);
                    var editableAreas = string.IsNullOrEmpty(hostPageCommonDataInfo.ContentItemCommonDataVisualBuilderWidgets)
                        ? new EditableAreasConfiguration()
                        : JsonConvert.DeserializeObject<EditableAreasConfiguration>(hostPageCommonDataInfo.ContentItemCommonDataVisualBuilderWidgets)!;

                    // fill widget properties
                    var childNodes = modelFacade.Select<ICmsTree>("NodeParentID = @nodeID", "NodeOrder", new SqlParameter("nodeID", cmsTree.NodeID));
                    var childItemGUIDs = childNodes.Select(x => ContentItemInfo.Provider.Get(x.NodeGUID)?.ContentItemGUID).Where(x => x is not null).Select(x => x!.Value).ToList();   // don't presume that ContentItem matching the Node exists. Director might have dropped it

                    var widgetProperties = widgetDirective.ItemToWidgetPropertiesMapping?.Invoke(commonDataModel.CustomProperties.Concat(dataModel.CustomProperties).ToDictionary(), storeContentItem ? contentItemModel.ContentItemGUID : null, childItemGUIDs) ?? [];

                    // attach widget object
                    if (widgetDirective.EditableAreaIdentifier is null)
                    {
                        logger.LogError("Could not convert document to widget. Editable area identifier not specified for widgetized CMS Document {DocumentGUID}", cmsDocument.DocumentGUID);
                        throw new Exception($"Could not convert document to widget. Editable area identifier not specified for widgetized CMS Document {cmsDocument.DocumentGUID}");
                    }
                    var editableArea = editableAreas.EditableAreas.FirstOrDefault(x => x.Identifier == widgetDirective.EditableAreaIdentifier);
                    if (editableArea is null)
                    {
                        editableArea = new EditableAreaConfiguration { Identifier = widgetDirective.EditableAreaIdentifier };
                        editableAreas.EditableAreas.Add(editableArea);
                    }

                    var section = editableArea.Sections.FirstOrDefault(x => x.Identifier.Equals(widgetDirective.SectionGuid));
                    if (section is null)
                    {
                        if (widgetDirective.SectionType is null)
                        {
                            logger.LogError("Could not convert document to widget. Section type required and not specified for widgetized CMS Document {DocumentGUID}", cmsDocument.DocumentGUID);
                            throw new Exception($"Could not convert document to widget. Section type required and not specified for widgetized CMS Document {cmsDocument.DocumentGUID}");
                        }
                        section = new SectionConfiguration
                        {
                            TypeIdentifier = widgetDirective.SectionType,
                            Identifier = widgetDirective.SectionGuid ?? Guid.NewGuid()
                        };
                        editableArea.Sections.Add(section);
                    }

                    var zone = widgetDirective.ZoneFirst
                        ? section.Zones.Count == 0 ? null : section.Zones[0]
                        : widgetDirective.ZoneName is not null
                            ? section.Zones.FirstOrDefault(x => widgetDirective.ZoneName.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase))
                            : section.Zones.FirstOrDefault(x => widgetDirective.ZoneGuid.Equals(x.Identifier));
                    if (zone is null)
                    {
                        zone = new ZoneConfiguration { Identifier = Guid.NewGuid() };
                        section.Zones.Add(zone);
                    }

                    var widgetVariant = new WidgetVariantConfiguration { Identifier = widgetDirective.WidgetVariantGuid ?? Guid.NewGuid(), Properties = widgetProperties };
                    var widgetConfig = new WidgetConfiguration
                    {
                        Identifier = widgetDirective.WidgetGuid ?? Guid.NewGuid(),
                        TypeIdentifier = widgetDirective.WidgetType,
                        Variants = [widgetVariant]
                    };
                    zone!.Widgets.Add(widgetConfig);

                    // store modified common data of the host page
                    var hostPageCommonDataModel = new ContentItemCommonDataModel
                    {
                        ContentItemCommonDataGUID = hostPageCommonDataInfo.ContentItemCommonDataGUID,
                        ContentItemCommonDataContentItemGuid = hostPageContentItemInfo.ContentItemGUID,
                        ContentItemCommonDataContentLanguageGuid = languageInfo.ContentLanguageGUID,
                        ContentItemCommonDataIsLatest = hostPageCommonDataInfo.ContentItemCommonDataIsLatest,
                        ContentItemCommonDataVersionStatus = hostPageCommonDataInfo.ContentItemCommonDataVersionStatus,
                        ContentItemCommonDataVisualBuilderTemplateConfiguration = hostPageCommonDataInfo.ContentItemCommonDataVisualBuilderTemplateConfiguration,
                        ContentItemCommonDataVisualBuilderWidgets = JsonConvert.SerializeObject(editableAreas)
                    };

                    deferredTreeNodesService.WidgetizedDocuments[documentGuid] = (hostPageCommonDataInfo.ContentItemCommonDataGUID, widgetVariant.Identifier);
                    yield return hostPageCommonDataModel;
                }
            }

            if (storeContentItem)
            {
                Guid? documentCreatedByUserGuid = null;
                if (modelFacade.TrySelectGuid<ICmsUser>(cmsDocument.DocumentCreatedByUserID, out var createdByUserGuid))
                {
                    documentCreatedByUserGuid = createdByUserGuid;
                }

                Guid? documentModifiedByUserGuid = null;
                if (modelFacade.TrySelectGuid<ICmsUser>(cmsDocument.DocumentModifiedByUserID, out var modifiedByUserGuid))
                {
                    documentModifiedByUserGuid = modifiedByUserGuid;
                }

                var languageMetadataInfo = new ContentItemLanguageMetadataModel
                {
                    ContentItemLanguageMetadataGUID = documentGuid,
                    ContentItemLanguageMetadataContentItemGuid = contentItemGuid,
                    ContentItemLanguageMetadataDisplayName = cmsDocument.DocumentName, // For the admin UI only
                    ContentItemLanguageMetadataLatestVersionStatus = draftMigrated ? VersionStatus.Draft : versionStatus, // That's the latest status of th item for admin optimization
                    ContentItemLanguageMetadataCreatedWhen = cmsDocument.DocumentCreatedWhen, // DocumentCreatedWhen
                    ContentItemLanguageMetadataModifiedWhen = cmsDocument.DocumentModifiedWhen, // DocumentModifiedWhen
                    ContentItemLanguageMetadataCreatedByUserGuid = documentCreatedByUserGuid,
                    ContentItemLanguageMetadataModifiedByUserGuid = documentModifiedByUserGuid,
                    // logic inaccessible, not supported
                    // ContentItemLanguageMetadataHasImageAsset = ContentItemAssetHasImageArbiter.HasImage(contentItemDataInfo), // This is for admin UI optimization - set to true if latest version contains a field with an image asset
                    ContentItemLanguageMetadataHasImageAsset = false,
                    ContentItemLanguageMetadataContentLanguageGuid = languageGuid, // DocumentCulture -> language entity needs to be created and its ID used here
                    ContentItemLanguageMetadataScheduledPublishWhen = scheduledPublishWhen,
                    ContentItemLanguageMetadataScheduledUnpublishWhen = scheduleUnpublishWhen
                };
                yield return languageMetadataInfo;
            }
        }

        // mapping of linked nodes is not supported
        Debug.Assert(cmsTree.NodeLinkedNodeID == null, "cmsTree.NodeLinkedNodeId == null");
        Debug.Assert(cmsTree.NodeLinkedNodeSiteID == null, "cmsTree.NodeLinkedNodeSiteId == null");

        if (!isMappedTypeReusable)
        {
            yield return new WebPageItemModel
            {
                WebPageItemParentGuid = nodeParentGuid, // NULL => under root
                WebPageItemGUID = contentItemGuid,
                WebPageItemName = safeNodeName,
                WebPageItemTreePath = treePath,
                WebPageItemWebsiteChannelGuid = siteGuid,
                WebPageItemContentItemGuid = contentItemGuid,
                WebPageItemOrder = cmsTree.NodeOrder ?? 0 // 0 is nullish value
            };
        }
    }

    private IEnumerable<FormerPageUrlPath> GetFormerUrlPaths(ICmsTree ksNode) => modelFacade.IsAvailable<ICmsPageFormerUrlPath>()
            ? modelFacade.SelectWhere<ICmsPageFormerUrlPath>(
                "PageFormerUrlPathSiteID = @siteId AND PageFormerUrlPathNodeID = @nodeId",
                new SqlParameter("siteId", ksNode.NodeSiteID),
                new SqlParameter("nodeId", ksNode.NodeID)
            ).Select(x => x switch
            {
                CmsPageFormerUrlPathK13 k13Path => new FormerPageUrlPath(k13Path.PageFormerUrlPathCulture, k13Path.PageFormerUrlPathUrlPath, k13Path.PageFormerUrlPathLastModified),
                _ => throw new NotImplementedException("Internal error 60fc462d-f59c-473c-bc4b-852263bb0ad7. Report this issue.")
            }).ToArray()
            : (IEnumerable<FormerPageUrlPath>)([]);


    private readonly ConcurrentDictionary<string, ContentLanguageInfo> languages = new(StringComparer.InvariantCultureIgnoreCase);
    private ContentLanguageInfo GetLanguageInfo(string culture) => languages.GetOrAdd(
                culture,
                s => ContentLanguageInfoProvider.ProviderObject.Get().WhereEquals(nameof(ContentLanguageInfo.ContentLanguageName), s).SingleOrDefault() ?? throw new InvalidOperationException($"Missing content language '{s}'")
            );

    private ContentItemDirectiveBase GetDirective(ContentItemSource contentItemSource)
    {
        var directiveFacade = new ContentItemActionProvider();
        foreach (var director in directors)
        {
            director.MediaInfoLoader = new Func<Guid, JToken>(LoadMediaInfo);
            director.Direct(contentItemSource, directiveFacade);
            if (directiveFacade.Directive is not null)
            {
                break;
            }
        }
        directiveFacade.Directive!.FormerUrlPaths ??= contentItemSource.FormerUrlPaths;
        return directiveFacade.Directive!;
    }

    private IEnumerable<IUmtModel> MigrateDraft(ICmsVersionHistory checkoutVersion, ICmsTree cmsTree, string sourceFormClassDefinition, string targetFormDefinition, Guid contentItemGuid,
        Guid contentLanguageGuid, ICmsClass sourceNodeClass, WebsiteChannelInfo websiteChannelInfo, ICmsSite sourceSite, IClassMapping? mapping)
    {
        var adapter = new NodeXmlAdapter(checkoutVersion.NodeXML);

        ContentItemCommonDataModel? commonDataModel = null;
        ContentItemDataModel? dataModel = null;

        string targetClassName = mapping?.TargetClassName ?? sourceNodeClass.ClassName;

        try
        {
            string? pageTemplateConfiguration = adapter.DocumentPageTemplateConfiguration;
            string? pageBuildWidgets = adapter.DocumentPageBuilderWidgets;
            (pageTemplateConfiguration, pageBuildWidgets, bool ndp) = visualBuilderPatcher.PatchJsonDefinitions(checkoutVersion.NodeSiteID, pageTemplateConfiguration, pageBuildWidgets).GetAwaiter().GetResult();

            #region Find existing guid

            var contentItemCommonDataGuid = Guid.NewGuid();
            var contentItemInfo = ContentItemInfo.Provider.Get()
                .WhereEquals(nameof(ContentItemInfo.ContentItemGUID), contentItemGuid)
                .FirstOrDefault();
            if (contentItemInfo != null)
            {
                var contentItems = ContentItemCommonDataInfo.Provider.Get()
                        .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataContentItemID), contentItemInfo.ContentItemID)
                        .ToList()
                    ;

                var existingDraft = contentItems.FirstOrDefault(x => x.ContentItemCommonDataVersionStatus == VersionStatus.Draft);
                if (existingDraft is { ContentItemCommonDataGUID: { } existingGuid })
                {
                    contentItemCommonDataGuid = existingGuid;
                }
            }

            #endregion

            commonDataModel = new ContentItemCommonDataModel
            {
                ContentItemCommonDataGUID = contentItemCommonDataGuid, // adapter.DocumentGUID ?? throw new InvalidOperationException($"DocumentGUID is null"),
                ContentItemCommonDataContentItemGuid = contentItemGuid,
                ContentItemCommonDataContentLanguageGuid = contentLanguageGuid,
                ContentItemCommonDataVersionStatus = VersionStatus.Draft,
                ContentItemCommonDataIsLatest = true, // Flag for latest record to know what to retrieve for the UI
                ContentItemCommonDataVisualBuilderWidgets = pageBuildWidgets,
                ContentItemCommonDataVisualBuilderTemplateConfiguration = pageTemplateConfiguration
            };

            if (ndp)
            {
                deferredPathService.AddPatch(
                    commonDataModel.ContentItemCommonDataGUID ?? throw new InvalidOperationException("DocumentGUID is null"),
                    ContentItemCommonDataInfo.TYPEINFO.ObjectClassName,// sourceNodeClass.ClassName,
                    websiteChannelInfo.WebsiteChannelID
                );
            }

            dataModel = new ContentItemDataModel
            {
                ContentItemDataGUID = commonDataModel.ContentItemCommonDataGUID,
                ContentItemDataCommonDataGuid = commonDataModel.ContentItemCommonDataGUID,
                ContentItemContentTypeName = mapping?.TargetClassName ?? sourceNodeClass.ClassName
            };

            if (sourceNodeClass.ClassIsCoupledClass)
            {
                var fi = new FormInfo(targetFormDefinition);
                var sfi = new FormInfo(sourceFormClassDefinition);
                string primaryKeyName = "";
                foreach (var sourceFieldInfo in sfi.GetFields(true, true))
                {
                    if (sourceFieldInfo.PrimaryKey)
                    {
                        primaryKeyName = sourceFieldInfo.Name;
                    }
                }

                if (string.IsNullOrWhiteSpace(primaryKeyName))
                {
                    throw new Exception("Error, unable to find coupled data primary key");
                }

                var commonFields = UnpackReusableFieldSchemas(fi.GetFields<FormSchemaInfo>()).ToArray();
                var sourceColumns = commonFields
                    .Select(cf => ReusableSchemaService.RemoveClassPrefix(sourceNodeClass.ClassName, cf.Name))
                    .Union(fi.GetColumnNames(false))
                    .Except(CmsClassMapper.GetLegacyMetadataFields(modelFacade.SelectVersion(), configuration.IncludeExtendedMetadata.GetValueOrDefault(false)).Select(x => CmsClassMapper.GetMappedLegacyField(fi, targetClassName, x.LegacyFieldName)))
                    .Where(x => x is not null)
                    .Select(x => x!)
                    .ToList();

                var sourceObjectContext = new DocumentSourceObjectContext(cmsTree, sourceNodeClass, sourceSite, sfi, fi, adapter.DocumentID);
                var convertorContext = new ConvertorTreeNodeContext(cmsTree.NodeGUID, cmsTree.NodeSiteID, adapter.DocumentID, false);
                // TODO tomas.krch: 2024-09-05 propagate async to root
                MapCoupledDataFieldValues(dataModel.CustomProperties,
                    s => adapter.GetValue(s),
                    s => adapter.HasValueSet(s),
                    // cmsTree, adapter.DocumentID,
                    sourceColumns, sfi, fi, true, sourceNodeClass,
                    // sourceSite, 
                    mapping,
                    sourceObjectContext,
                    convertorContext,
                    targetClassName
                    ).GetAwaiter().GetResult();

                foreach (var formFieldInfo in commonFields)
                {
                    string originalFieldName = ReusableSchemaService.RemoveClassPrefix(sourceNodeClass.ClassName, formFieldInfo.Name);
                    if (dataModel.CustomProperties.TryGetValue(originalFieldName, out object? value))
                    {
                        commonDataModel.CustomProperties ??= [];
                        logger.LogTrace("Reusable schema field '{FieldName}' from schema '{SchemaGuid}' populated", formFieldInfo.Name, formFieldInfo.Properties[ReusableFieldSchemaConstants.SCHEMA_IDENTIFIER_KEY]);
                        commonDataModel.CustomProperties[formFieldInfo.Name] = value;
                        dataModel.CustomProperties.Remove(originalFieldName);
                    }
                    else
                    {
                        logger.LogTrace("Reusable schema field '{FieldName}' from schema '{SchemaGuid}' missing", formFieldInfo.Name, formFieldInfo.Properties[ReusableFieldSchemaConstants.SCHEMA_IDENTIFIER_KEY]);
                    }
                }
            }

            // supply legacy document metadata fields
            foreach (var legacyField in CmsClassMapper.GetLegacyMetadataFields(modelFacade.SelectVersion(), configuration.IncludeExtendedMetadata.GetValueOrDefault(false)))
            {
                if (reusableSchemaService.IsConversionToReusableFieldSchemaRequested(sourceNodeClass.ClassName))
                {
                    string fieldName = ReusableSchemaService.GetUniqueFieldName(sourceNodeClass.ClassName, legacyField.LegacyFieldName);
                    commonDataModel.CustomProperties.Add(fieldName, adapter.GetValue(legacyField.LegacyFieldName));
                }
                else
                {
                    dataModel.CustomProperties.Add(legacyField.LegacyFieldName, adapter.GetValue(legacyField.LegacyFieldName));
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed attempt to create draft from '{checkoutVersion}'  {ex}");
            throw;
        }

        if (dataModel != null && commonDataModel != null)
        {
            yield return commonDataModel;
            yield return dataModel;
        }
    }

    private async Task MapCoupledDataFieldValues(
      Dictionary<string, object?> target,
      Func<string, object?> getSourceValue,
      Func<string, bool> containsSourceValue,
      List<string> newColumnNames,
      FormInfo oldFormInfo,
      FormInfo newFormInfo,
      bool migratingFromVersionHistory,
      ICmsClass sourceNodeClass,
      IClassMapping? mapping,
      ISourceObjectContext sourceObjectContext,
      IConvertorContext convertorContext,
      string targetClassName
  )
    {
        Debug.Assert(sourceNodeClass.ClassTableName != null, "sourceNodeClass.ClassTableName != null");

        foreach (string targetColumnName in newColumnNames)
        {
            string targetFieldName = null!;
            Func<object?, IConvertorContext, object?> valueConvertor = (sourceValue, _) => sourceValue;
            switch (mapping?.GetMapping(targetColumnName, sourceNodeClass.ClassName))
            {
                case FieldMappingWithConversion fieldMappingWithConversion:
                {
                    targetFieldName = fieldMappingWithConversion.TargetFieldName;
                    valueConvertor = fieldMappingWithConversion.Converter;
                    break;
                }
                case FieldMapping fieldMapping:
                {
                    targetFieldName = fieldMapping.TargetFieldName;
                    valueConvertor = (sourceValue, _) => sourceValue;
                    break;
                }
                case null:
                {
                    targetFieldName = targetColumnName;
                    valueConvertor = (sourceValue, _) => sourceValue;
                    break;
                }

                default:
                    break;
            }

            if (
                targetFieldName.Equals("ContentItemDataID", StringComparison.InvariantCultureIgnoreCase) ||
                targetFieldName.Equals("ContentItemDataCommonDataID", StringComparison.InvariantCultureIgnoreCase) ||
                targetFieldName.Equals("ContentItemDataGUID", StringComparison.InvariantCultureIgnoreCase) ||
                CmsClassMapper.GetLegacyMetadataFields(modelFacade.SelectVersion(), configuration.IncludeExtendedMetadata.GetValueOrDefault(false)).Any(x => targetFieldName.Equals(x.LegacyFieldName, StringComparison.InvariantCultureIgnoreCase))
            )
            {
                logger.LogTrace("Skipping '{FieldName}'", targetFieldName);
                continue;
            }

#pragma warning disable CS0618 // Type or member is obsolete
            if (oldFormInfo.GetFormField(targetFieldName)?.External is true)
#pragma warning restore CS0618 // Type or member is obsolete
            {
                logger.LogTrace("Skipping '{FieldName}' - is external", targetFieldName);
                continue;
            }

            string sourceFieldName = mapping?.GetSourceFieldName(targetColumnName, sourceNodeClass.ClassName) ?? targetColumnName;
            if (!containsSourceValue(sourceFieldName))
            {
                if (migratingFromVersionHistory)
                {
                    logger.LogDebug("Value is not contained in source, field '{Field}' (possibly because version existed before field was added to class form)", targetColumnName);
                }
                else
                {
                    logger.LogWarning("Value is not contained in source, field '{Field}'", targetColumnName);
                }

                continue;
            }


            var field = oldFormInfo.GetFormField(sourceFieldName);
            string? controlName = field.Settings[CLASS_FIELD_CONTROL_NAME]?.ToString()?.ToLowerInvariant();

            object? sourceValue = getSourceValue(sourceFieldName);
            target[targetFieldName] = valueConvertor.Invoke(sourceValue, convertorContext);
            var fvmc = new FieldMigrationContext(field.DataType, controlName, targetColumnName, sourceObjectContext);
            var fmb = fieldMigrationService.GetFieldMigration(fvmc);
            if (fmb is FieldMigration fieldMigration)
            {
                var documentSourceObjectContext = sourceObjectContext as DocumentSourceObjectContext;
                if (controlName != null)
                {
                    if ((fieldMigration.Actions?.Contains(TcaDirective.ConvertToPages) ?? false) && documentSourceObjectContext != null)
                    {
                        // relation to other document
                        var convertedRelation = relationshipService.GetNodeRelationships(documentSourceObjectContext.CmsTree.NodeID, sourceNodeClass.ClassName, field.Guid)
                            .Select(r => new WebPageRelatedItem { WebPageGuid = spoiledGuidContext.EnsureNodeGuid(r.RightNode!.NodeGUID, r.RightNode.NodeSiteID, r.RightNode.NodeID) });

                        target.SetValueAsJson(targetFieldName, valueConvertor.Invoke(convertedRelation, convertorContext));
                    }
                    else
                    {
                        // leave as is
                        target[targetFieldName] = valueConvertor.Invoke(sourceValue, convertorContext);
                    }

                    if (fieldMigration.TargetFormComponent == "webpages" && documentSourceObjectContext != null)
                    {
                        if (sourceValue is string pageReferenceJson)
                        {
                            var parsed = JObject.Parse(pageReferenceJson);
                            foreach (var jToken in parsed.DescendantsAndSelf())
                            {
                                if (jToken.Path.EndsWith("NodeGUID", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var patchedGuid = spoiledGuidContext.EnsureNodeGuid(jToken.Value<Guid>(), documentSourceObjectContext.CmsTree.NodeSiteID);
                                    jToken.Replace(JToken.FromObject(patchedGuid));
                                }
                            }

                            target[targetFieldName] = valueConvertor.Invoke(parsed.ToString().Replace("\"NodeGuid\"", "\"WebPageGuid\""), convertorContext);
                        }
                    }
                }
                else
                {
                    target[targetFieldName] = valueConvertor.Invoke(sourceValue, convertorContext);
                }
            }
            else if (fmb != null)
            {
                switch (await fmb.MigrateValue(sourceValue, fvmc))
                {
                    case { Success: true } result:
                    {
                        target[targetFieldName] = valueConvertor.Invoke(result.MigratedValue, convertorContext);
                        break;
                    }
                    case { Success: false }:
                    {
                        logger.LogError("Error while migrating field '{Field}' value {Value}", targetFieldName, sourceValue);
                        target[targetFieldName] = null;
                        break;
                    }

                    default:
                        break;
                }
            }
            else
            {
                target[targetFieldName] = valueConvertor?.Invoke(sourceValue, convertorContext);
            }


            var newField = newFormInfo.GetFormField(targetColumnName);
            if (newField == null)
            {

                var commonFields = UnpackReusableFieldSchemas(newFormInfo.GetFields<FormSchemaInfo>()).ToArray();
                newField = commonFields
                    .FirstOrDefault(cf => ReusableSchemaService.RemoveClassPrefix(mapping?.TargetClassName ?? sourceNodeClass.ClassName, cf.Name).Equals(targetColumnName, StringComparison.InvariantCultureIgnoreCase));
            }
            string? newControlName = newField?.Settings[CLASS_FIELD_CONTROL_NAME]?.ToString()?.ToLowerInvariant();
            if (newControlName?.Equals(FormComponents.AdminRichTextEditorComponent, StringComparison.InvariantCultureIgnoreCase) == true && target[targetColumnName] is string { } html && !string.IsNullOrWhiteSpace(html) &&
                !configuration.MigrateMediaToMediaLibrary)
            {
                var mediaLinkService = mediaLinkServiceFactory.Create();
                var htmlProcessor = new HtmlProcessor(html, mediaLinkService);
                if (sourceObjectContext is DocumentSourceObjectContext documentSourceObjectContext)
                {
                    target[targetColumnName] = await htmlProcessor.ProcessHtml(documentSourceObjectContext.Site.SiteID, async (result, original) =>
                    {
                        switch (result)
                        {
                            case { LinkKind: MediaLinkKind.Guid or MediaLinkKind.DirectMediaPath, MediaKind: MediaKind.MediaFile }:
                            {
                                var mediaFile = MediaHelper.GetMediaFile(result, modelFacade, original, logger);
                                if (mediaFile is null)
                                {
                                    return original;
                                }

                                return assetFacade.GetAssetUri(mediaFile);
                            }
                            case { LinkKind: MediaLinkKind.Guid, MediaKind: MediaKind.Attachment, MediaGuid: { } mediaGuid, LinkSiteId: var linkSiteId }:
                            {
                                var attachment = MediaHelper.GetAttachment(result, modelFacade);
                                if (attachment is null)
                                {
                                    return original;
                                }

                                await attachmentMigrator.MigrateAttachment(attachment);

                                string? culture = null;
                                if (attachment.AttachmentDocumentID is { } attachmentDocumentId)
                                {
                                    culture = modelFacade.SelectById<ICmsDocument>(attachmentDocumentId)?.DocumentCulture;
                                }

                                return assetFacade.GetAssetUri(attachment, culture);
                            }

                            default:
                                break;
                        }

                        return original;
                    });
                }
            }
        }
    }

    private static IEnumerable<FormFieldInfo> UnpackReusableFieldSchemas(IEnumerable<FormSchemaInfo> schemaInfos)
    {
        using var siEnum = schemaInfos.GetEnumerator();

        if (siEnum.MoveNext() && FormHelper.GetFormInfo(ContentItemCommonDataInfo.TYPEINFO.ObjectClassName, true) is { } cfi)
        {
            do
            {
                var fsi = siEnum.Current;
                var formFieldInfos = cfi
                    .GetFields(true, true)
                    .Where(f => string.Equals(f.Properties[ReusableFieldSchemaConstants.SCHEMA_IDENTIFIER_KEY] as string, fsi.Guid.ToString(),
                        StringComparison.InvariantCultureIgnoreCase));

                foreach (var formFieldInfo in formFieldInfos)
                {
                    yield return formFieldInfo;
                }
            } while (siEnum.MoveNext());
        }
    }


    public IEnumerable<IUmtModel> Map(CustomTableMapperSource source)
    {
        // only reusable items
        (string? targetFormDefinition, string sourceFormDefinition, var contentItemGuid, var sourceClass, var values, var classMapping) = source;

        var mapping = classMappingProvider.GetMapping(sourceClass.ClassName);
        var targetClassGuid = sourceClass.ClassGUID;
        var targetClassInfo = DataClassInfoProvider.ProviderObject.Get(sourceClass.ClassName);
        if (mapping != null)
        {
            targetClassInfo = DataClassInfoProvider.ProviderObject.Get(mapping.TargetClassName) ?? throw new InvalidOperationException($"Unable to find target class '{mapping.TargetClassName}'");
            targetClassGuid = targetClassInfo.ClassGUID;
        }

        var mappingHandler = typeof(DefaultCustomTableClassMappingHandler);
        if (classMapping.MappingHandler is not null)
        {
            mappingHandler = classMapping.MappingHandler;
        }

        if (serviceProvider.GetRequiredService(mappingHandler) is not IClassMappingHandler mappingHandlerInstance)
        {
            throw new InvalidOperationException($"Incorrect handler registered '{mappingHandler.FullName}'");
        }

        IClassMappingHandler handler = new ClassMappingHandlerWrapper(mappingHandlerInstance, logger);
        var ctms = new CustomTableMappingHandlerContext(values, targetClassInfo, sourceClass.ClassName);

        bool isMappedTypeReusable = targetClassInfo?.ClassContentTypeType is ClassContentTypeType.REUSABLE; // TODO tomas.krch: 2024-12-03 configuration here? || configuration.ClassNamesConvertToContentHub.Contains(sourceNodeClass.ClassName);
        if (!isMappedTypeReusable)
        {
            throw new InvalidOperationException("Mapping of custom table items to web site channel is currently not supported");
        }

        var contentItemModel = new ContentItemModel { ContentItemGUID = contentItemGuid, ContentItemIsReusable = isMappedTypeReusable, ContentItemDataClassGuid = targetClassGuid, };

        handler.EnsureContentItem(contentItemModel, ctms);

        yield return contentItemModel;

        var versionStatus = VersionStatus.Published;


        DateTime? scheduledPublishWhen = null;
        DateTime? scheduleUnpublishWhen = null;
        string? contentItemCommonDataPageBuilderWidgets = null;
        string? contentItemCommonDataPageTemplateConfiguration = null;


        // todo async
        var languageVersions = handler.ProduceLanguageVersions(ctms).GetAwaiter().GetResult();
        foreach (var (contentLanguageInfo, languageSensitiveValues) in languageVersions)
        {
            var commonDataModel = new ContentItemCommonDataModel
            {
                ContentItemCommonDataContentItemGuid = contentItemGuid,
                ContentItemCommonDataContentLanguageGuid = contentLanguageInfo.ContentLanguageGUID,
                ContentItemCommonDataVersionStatus = versionStatus,
                ContentItemCommonDataVisualBuilderWidgets = contentItemCommonDataPageBuilderWidgets,
                ContentItemCommonDataVisualBuilderTemplateConfiguration = contentItemCommonDataPageTemplateConfiguration,
            };

            handler.EnsureContentItemCommonData(commonDataModel, ctms);

            var dataModel = new ContentItemDataModel
            {
                ContentItemDataGUID = commonDataModel.ContentItemCommonDataGUID,
                ContentItemDataCommonDataGuid = commonDataModel.ContentItemCommonDataGUID,
                ContentItemContentTypeName = mapping?.TargetClassName ?? targetClassInfo?.ClassName
            };

            var fi = new FormInfo(targetFormDefinition);
            var sfi = new FormInfo(sourceFormDefinition);
            string primaryKeyName = "";
            foreach (var sourceFieldInfo in sfi.GetFields(true, true))
            {
                if (sourceFieldInfo.PrimaryKey)
                {
                    primaryKeyName = sourceFieldInfo.Name;
                }
            }

            if (string.IsNullOrWhiteSpace(primaryKeyName))
            {
                throw new Exception("Error, unable to find coupled data primary key");
            }

            var commonFields = UnpackReusableFieldSchemas(fi.GetFields<FormSchemaInfo>()).ToArray();
            var targetColumns = commonFields
                .Select(cf => ReusableSchemaService.RemoveClassPrefix(sourceClass.ClassName, cf.Name))
                .Union(fi.GetColumnNames(false))
                .Except(CmsClassMapper.GetLegacyMetadataFields(modelFacade.SelectVersion(), configuration.IncludeExtendedMetadata.GetValueOrDefault(false))
                .Select(x => CmsClassMapper.GetMappedLegacyField(fi, targetClassInfo!.ClassName, x.LegacyFieldName)))
                .Where(x => x is not null)
                .Select(x => x!)
                .ToList();

            var sourceObjectContext = new CustomTableSourceObjectContext();
            var convertorContext = new ConvertorCustomTableContext();

            // TODO tomas.krch: 2024-09-05 propagate async to root
            MapCoupledDataFieldValues(dataModel.CustomProperties,
                columnName => languageSensitiveValues[columnName],
                columnName => languageSensitiveValues.ContainsKey(columnName),
                // cmsTree, cmsDocument.DocumentID,
                targetColumns, sfi, fi,
                false, sourceClass,
                //sourceSite, 
                mapping,
                sourceObjectContext,
                convertorContext,
                targetClassInfo!.ClassName
            ).GetAwaiter().GetResult();

            foreach (var legacyField in CmsClassMapper.GetLegacyMetadataFields(modelFacade.SelectVersion(), configuration.IncludeExtendedMetadata.GetValueOrDefault(false)))
            {
                string? documentNameFieldName = CmsClassMapper.GetMappedLegacyField(fi, targetClassInfo.ClassName, legacyField.LegacyFieldName);
                if (documentNameFieldName is not null)
                {
                    dataModel.CustomProperties[documentNameFieldName] = contentItemModel.ContentItemName;
                }
            }

            foreach (var formFieldInfo in commonFields)
            {
                string originalFieldName = ReusableSchemaService.RemoveClassPrefix(sourceClass.ClassName, formFieldInfo.Name);
                if (dataModel.CustomProperties.TryGetValue(originalFieldName, out object? value))
                {
                    commonDataModel.CustomProperties ??= [];
                    logger.LogTrace("Reusable schema field '{FieldName}' from schema '{SchemaGuid}' populated", formFieldInfo.Name, formFieldInfo.Properties[ReusableFieldSchemaConstants.SCHEMA_IDENTIFIER_KEY]);
                    commonDataModel.CustomProperties[formFieldInfo.Name] = value;
                    dataModel.CustomProperties.Remove(originalFieldName);
                }
                else
                {
                    logger.LogTrace("Reusable schema field '{FieldName}' from schema '{SchemaGuid}' missing", formFieldInfo.Name, formFieldInfo.Properties[ReusableFieldSchemaConstants.SCHEMA_IDENTIFIER_KEY]);
                }
            }

            yield return commonDataModel;
            yield return dataModel;

            Guid? documentCreatedByUserGuid = null;
            int? createdByUserId = handler.GetCreatedByUserId(ctms, sourceClass.ClassName, sourceClass.ClassFormDefinition);
            if (createdByUserId.HasValue && modelFacade.TrySelectGuid<ICmsUser>(createdByUserId, out var createdByUserGuid))
            {
                documentCreatedByUserGuid = createdByUserGuid;
            }

            Guid? documentModifiedByUserGuid = null;
            int? modifiedByUserId = handler.GetModifiedByUserId(ctms, sourceClass.ClassName, sourceClass.ClassFormDefinition);
            if (modelFacade.TrySelectGuid<ICmsUser>(modifiedByUserId, out var modifiedByUserGuid))
            {
                documentModifiedByUserGuid = modifiedByUserGuid;
            }

            var languageMetadataInfo = new ContentItemLanguageMetadataModel
            {
                ContentItemLanguageMetadataContentItemGuid = contentItemGuid,
                ContentItemLanguageMetadataLatestVersionStatus = VersionStatus.Published, // That's the latest status of th item for admin optimization
                ContentItemLanguageMetadataCreatedByUserGuid = documentCreatedByUserGuid,
                ContentItemLanguageMetadataModifiedByUserGuid = documentModifiedByUserGuid,
                ContentItemLanguageMetadataHasImageAsset = false,
                ContentItemLanguageMetadataContentLanguageGuid = commonDataModel.ContentItemCommonDataContentLanguageGuid, // DocumentCulture -> language entity needs to be created and its ID used here
                ContentItemLanguageMetadataScheduledPublishWhen = scheduledPublishWhen,
                ContentItemLanguageMetadataScheduledUnpublishWhen = scheduleUnpublishWhen
            };

            handler.EnsureContentItemLanguageMetadata(languageMetadataInfo, ctms);

            yield return languageMetadataInfo;
        }
    }
    internal static JToken LoadMediaInfo(Guid mediaFileGuid)
    {
        var info = MediaFileInfo.Provider.Get(mediaFileGuid);
        return new JArray(new JObject
        {
            { "identifier", mediaFileGuid },
            { "name", info.FileName },
            { "size", info.FileSize },
            { "dimensions", new JObject {
                { "width", info.FileImageWidth },
                { "height", info.FileImageHeight },
            } },
        });
    }
}
