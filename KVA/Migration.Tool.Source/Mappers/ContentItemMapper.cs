using System.Diagnostics;
using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.Core;
using CMS.Core.Internal;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.MediaLibrary;
using CMS.Websites;
using CMS.Websites.Internal;
using Kentico.Xperience.UMT.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Builders;
using Migration.Tool.Common.Enumerations;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.Services;
using Migration.Tool.Common.Services.Ipc;
using Migration.Tool.KXP.Api;
using Migration.Tool.KXP.Api.Auxiliary;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source.Auxiliary;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Helpers;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Providers;
using Migration.Tool.Source.Services;
using Migration.Tool.Source.Services.Model;
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
    ICmsSite SourceSite
);

public class ContentItemMapper(
    ILogger<ContentItemMapper> logger,
    CoupledDataService coupledDataService,
    ClassService classService,
    IAttachmentMigrator attachmentMigrator,
    CmsRelationshipService relationshipService,
    SourceInstanceContext sourceInstanceContext,
    FieldMigrationService fieldMigrationService,
    KxpMediaFileFacade mediaFileFacade,
    ModelFacade modelFacade,
    ReusableSchemaService reusableSchemaService,
    DeferredPathService deferredPathService,
    SpoiledGuidContext spoiledGuidContext,
    EntityIdentityFacade entityIdentityFacade,
    IAssetFacade assetFacade,
    MediaLinkServiceFactory mediaLinkServiceFactory,
    ToolConfiguration configuration,
    ClassMappingProvider classMappingProvider
) : UmtMapperBase<CmsTreeMapperSource>
{
    private const string CLASS_FIELD_CONTROL_NAME = "controlname";

    protected override IEnumerable<IUmtModel> MapInternal(CmsTreeMapperSource source)
    {
        (var cmsTree, string safeNodeName, var siteGuid, var nodeParentGuid, var cultureToLanguageGuid, string? targetFormDefinition, string sourceFormDefinition, var migratedDocuments, var sourceSite) = source;

        logger.LogTrace("Mapping {Value}", new { cmsTree.NodeAliasPath, cmsTree.NodeName, cmsTree.NodeGUID, cmsTree.NodeSiteID });

        var sourceNodeClass = modelFacade.SelectById<ICmsClass>(cmsTree.NodeClassID) ?? throw new InvalidOperationException($"Fatal: node class is missing, class id '{cmsTree.NodeClassID}'");
        var mapping = classMappingProvider.GetMapping(sourceNodeClass.ClassName);
        var targetClassGuid = sourceNodeClass.ClassGUID;
        if (mapping != null)
        {
            targetClassGuid = DataClassInfoProvider.ProviderObject.Get(mapping.TargetClassName)?.ClassGUID ?? throw new InvalidOperationException($"Unable to find target class '{mapping.TargetClassName}'");
        }

        bool migratedAsContentFolder = sourceNodeClass.ClassName.Equals("cms.folder", StringComparison.InvariantCultureIgnoreCase) && !configuration.UseDeprecatedFolderPageType.GetValueOrDefault(false);

        var contentItemGuid = spoiledGuidContext.EnsureNodeGuid(cmsTree.NodeGUID, cmsTree.NodeSiteID, cmsTree.NodeID);
        yield return new ContentItemModel
        {
            ContentItemGUID = contentItemGuid,
            ContentItemName = safeNodeName,
            ContentItemIsReusable = false, // page is not reusable
            ContentItemIsSecured = cmsTree.IsSecuredNode ?? false,
            ContentItemDataClassGuid = migratedAsContentFolder ? null : targetClassGuid,
            ContentItemChannelGuid = siteGuid
        };

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
                    migratedDraft = MigrateDraft(draftVersion, cmsTree, sourceFormDefinition, targetFormDefinition, contentItemGuid, languageGuid, sourceNodeClass, websiteChannelInfo, sourceSite, mapping).ToList();
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
            string? contentItemCommonDataPageBuilderWidgets = null;
            string? contentItemCommonDataPageTemplateConfiguration = null;

            bool ndp = false;
            if (!migratedAsContentFolder)
            {
                if (cmsDocument.DocumentPublishFrom is { } publishFrom)
                {
                    var now = Service.Resolve<IDateTimeNowService>().GetDateTimeNow();
                    if (publishFrom > now)
                    {
                        versionStatus = VersionStatus.Unpublished;
                    }
                    else
                    {
                        scheduledPublishWhen = publishFrom;
                    }
                }

                if (cmsDocument.DocumentPublishTo is { } publishTo)
                {
                    var now = Service.Resolve<IDateTimeNowService>().GetDateTimeNow();
                    if (publishTo < now)
                    {
                        versionStatus = VersionStatus.Unpublished;
                    }
                    else
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
                        contentItemCommonDataPageBuilderWidgets = doc.DocumentPageBuilderWidgets;
                        contentItemCommonDataPageTemplateConfiguration = doc.DocumentPageTemplateConfiguration;
                        break;
                    }
                    case CmsDocumentK13 doc:
                    {
                        contentItemCommonDataPageBuilderWidgets = doc.DocumentPageBuilderWidgets;
                        contentItemCommonDataPageTemplateConfiguration = doc.DocumentPageTemplateConfiguration;
                        break;
                    }

                    default:
                        break;
                }

                PatchJsonDefinitions(source.CmsTree.NodeSiteID, ref contentItemCommonDataPageTemplateConfiguration, ref contentItemCommonDataPageBuilderWidgets, out ndp);
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
                ContentItemCommonDataPageBuilderWidgets = contentItemCommonDataPageBuilderWidgets,
                ContentItemCommonDataPageTemplateConfiguration = contentItemCommonDataPageTemplateConfiguration,
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

                    var commonFields = UnpackReusableFieldSchemas(fi.GetFields<FormSchemaInfo>()).ToArray();
                    var targetColumns = commonFields
                        .Select(cf => ReusableSchemaService.RemoveClassPrefix(sourceNodeClass.ClassName, cf.Name))
                        .Union(fi.GetColumnNames(false))
                        .Except([CmsClassMapper.GetLegacyDocumentName(fi, sourceNodeClass.ClassName)])
                        .ToList();

                    var coupledDataRow = coupledDataService.GetSourceCoupledDataRow(sourceNodeClass.ClassTableName, primaryKeyName, cmsDocument.DocumentForeignKeyValue);
                    // TODO tomas.krch: 2024-09-05 propagate async to root
                    MapCoupledDataFieldValues(dataModel.CustomProperties,
                        columnName => coupledDataRow?[columnName],
                        columnName => coupledDataRow?.ContainsKey(columnName) ?? false,
                        cmsTree, cmsDocument.DocumentID,
                        targetColumns, sfi, fi,
                        false, sourceNodeClass, sourceSite, mapping
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

                string targetClassName = mapping?.TargetClassName ?? sourceNodeClass.ClassName;
                if (CmsClassMapper.GetLegacyDocumentName(fi, targetClassName) is { } legacyDocumentNameFieldName)
                {
                    if (reusableSchemaService.IsConversionToReusableFieldSchemaRequested(targetClassName))
                    {
                        string fieldName = ReusableSchemaService.GetUniqueFieldName(targetClassName, legacyDocumentNameFieldName);
                        commonDataModel.CustomProperties.Add(fieldName, cmsDocument.DocumentName);
                    }
                    else
                    {
                        dataModel.CustomProperties.Add(legacyDocumentNameFieldName, cmsDocument.DocumentName);
                    }
                }

                yield return commonDataModel;
                yield return dataModel;
            }

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

        // mapping of linked nodes is not supported
        Debug.Assert(cmsTree.NodeLinkedNodeID == null, "cmsTree.NodeLinkedNodeId == null");
        Debug.Assert(cmsTree.NodeLinkedNodeSiteID == null, "cmsTree.NodeLinkedNodeSiteId == null");

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

    private void PatchJsonDefinitions(int sourceSiteId, ref string? pageTemplateConfiguration, ref string? pageBuilderWidgets, out bool needsDeferredPatch)
    {
        needsDeferredPatch = false;
        if (sourceInstanceContext.HasInfo)
        {
            if (pageTemplateConfiguration != null)
            {
                var pageTemplateConfigurationObj = JsonConvert.DeserializeObject<Services.Model.PageTemplateConfiguration>(pageTemplateConfiguration);
                if (pageTemplateConfigurationObj?.Identifier != null)
                {
                    logger.LogTrace("Walk page template configuration {Identifier}", pageTemplateConfigurationObj.Identifier);

                    var pageTemplateConfigurationFcs =
                        sourceInstanceContext.GetPageTemplateFormComponents(sourceSiteId, pageTemplateConfigurationObj?.Identifier);
                    if (pageTemplateConfigurationObj.Properties is { Count: > 0 })
                    {
                        WalkProperties(sourceSiteId, pageTemplateConfigurationObj.Properties, pageTemplateConfigurationFcs, out bool ndp);
                        needsDeferredPatch = ndp || needsDeferredPatch;
                    }

                    pageTemplateConfiguration = JsonConvert.SerializeObject(pageTemplateConfigurationObj);
                }
            }

            if (pageBuilderWidgets != null)
            {
                var areas = JsonConvert.DeserializeObject<EditableAreasConfiguration>(pageBuilderWidgets);
                if (areas?.EditableAreas is { Count: > 0 })
                {
                    WalkAreas(sourceSiteId, areas.EditableAreas, out bool ndp);
                    needsDeferredPatch = ndp || needsDeferredPatch;
                }

                pageBuilderWidgets = JsonConvert.SerializeObject(areas);
            }
        }
    }

    private IEnumerable<IUmtModel> MigrateDraft(ICmsVersionHistory checkoutVersion, ICmsTree cmsTree, string sourceFormClassDefinition, string targetFormDefinition, Guid contentItemGuid,
        Guid contentLanguageGuid, ICmsClass sourceNodeClass, WebsiteChannelInfo websiteChannelInfo, ICmsSite sourceSite, IClassMapping mapping)
    {
        var adapter = new NodeXmlAdapter(checkoutVersion.NodeXML);

        ContentItemCommonDataModel? commonDataModel = null;
        ContentItemDataModel? dataModel = null;
        try
        {
            string? pageTemplateConfiguration = adapter.DocumentPageTemplateConfiguration;
            string? pageBuildWidgets = adapter.DocumentPageBuilderWidgets;
            PatchJsonDefinitions(checkoutVersion.NodeSiteID, ref pageTemplateConfiguration, ref pageBuildWidgets, out bool ndp);

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
                ContentItemCommonDataPageBuilderWidgets = pageBuildWidgets,
                ContentItemCommonDataPageTemplateConfiguration = pageTemplateConfiguration
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
                    .Except([CmsClassMapper.GetLegacyDocumentName(fi, sourceNodeClass.ClassName)])
                    .ToList();

                // TODO tomas.krch: 2024-09-05 propagate async to root
                MapCoupledDataFieldValues(dataModel.CustomProperties,
                    s => adapter.GetValue(s),
                    s => adapter.HasValueSet(s)
                    , cmsTree, adapter.DocumentID, sourceColumns, sfi, fi, true, sourceNodeClass, sourceSite, mapping).GetAwaiter().GetResult();

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

            // supply document name
            if (reusableSchemaService.IsConversionToReusableFieldSchemaRequested(sourceNodeClass.ClassName))
            {
                string fieldName = ReusableSchemaService.GetUniqueFieldName(sourceNodeClass.ClassName, "DocumentName");
                commonDataModel.CustomProperties.Add(fieldName, adapter.DocumentName);
            }
            else
            {
                dataModel.CustomProperties.Add("DocumentName", adapter.DocumentName);
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
        ICmsTree cmsTree,
        int? documentId,
        List<string> newColumnNames,
        FormInfo oldFormInfo,
        FormInfo newFormInfo,
        bool migratingFromVersionHistory,
        ICmsClass sourceNodeClass,
        ICmsSite site,
        IClassMapping mapping
    )
    {
        Debug.Assert(sourceNodeClass.ClassTableName != null, "sourceNodeClass.ClassTableName != null");

        foreach (string targetColumnName in newColumnNames)
        {
            string targetFieldName = null!;
            Func<object?, object?> valueConvertor = sourceValue => sourceValue;
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
                    valueConvertor = sourceValue => sourceValue;
                    break;
                }
                case null:
                {
                    targetFieldName = targetColumnName;
                    valueConvertor = sourceValue => sourceValue;
                    break;
                }

                default:
                    break;
            }

            if (
                targetFieldName.Equals("ContentItemDataID", StringComparison.InvariantCultureIgnoreCase) ||
                targetFieldName.Equals("ContentItemDataCommonDataID", StringComparison.InvariantCultureIgnoreCase) ||
                targetFieldName.Equals("ContentItemDataGUID", StringComparison.InvariantCultureIgnoreCase) ||
                targetFieldName.Equals(CmsClassMapper.GetLegacyDocumentName(newFormInfo, sourceNodeClass.ClassName), StringComparison.InvariantCultureIgnoreCase)
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
            target[targetFieldName] = valueConvertor.Invoke(sourceValue);
            var fvmc = new FieldMigrationContext(field.DataType, controlName, targetColumnName, new DocumentSourceObjectContext(cmsTree, sourceNodeClass, site, oldFormInfo, newFormInfo, documentId));
            var fmb = fieldMigrationService.GetFieldMigration(fvmc);
            if (fmb is FieldMigration fieldMigration)
            {
                if (controlName != null)
                {
                    if (fieldMigration.Actions?.Contains(TcaDirective.ConvertToPages) ?? false)
                    {
                        // relation to other document
                        var convertedRelation = relationshipService.GetNodeRelationships(cmsTree.NodeID, sourceNodeClass.ClassName, field.Guid)
                            .Select(r => new WebPageRelatedItem { WebPageGuid = spoiledGuidContext.EnsureNodeGuid(r.RightNode.NodeGUID, r.RightNode.NodeSiteID, r.RightNode.NodeID) });

                        target.SetValueAsJson(targetFieldName, valueConvertor.Invoke(convertedRelation));
                    }
                    else
                    {
                        // leave as is
                        target[targetFieldName] = valueConvertor.Invoke(sourceValue);
                    }

                    if (fieldMigration.TargetFormComponent == "webpages")
                    {
                        if (sourceValue is string pageReferenceJson)
                        {
                            var parsed = JObject.Parse(pageReferenceJson);
                            foreach (var jToken in parsed.DescendantsAndSelf())
                            {
                                if (jToken.Path.EndsWith("NodeGUID", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var patchedGuid = spoiledGuidContext.EnsureNodeGuid(jToken.Value<Guid>(), cmsTree.NodeSiteID);
                                    jToken.Replace(JToken.FromObject(patchedGuid));
                                }
                            }

                            target[targetFieldName] = valueConvertor.Invoke(parsed.ToString().Replace("\"NodeGuid\"", "\"WebPageGuid\""));
                        }
                    }
                }
                else
                {
                    target[targetFieldName] = valueConvertor.Invoke(sourceValue);
                }
            }
            else if (fmb != null)
            {
                switch (await fmb.MigrateValue(sourceValue, fvmc))
                {
                    case { Success: true } result:
                    {
                        target[targetFieldName] = valueConvertor.Invoke(result.MigratedValue);
                        break;
                    }
                    case { Success: false }:
                    {
                        logger.LogError("Error while migrating field '{Field}' value {Value}", targetFieldName, sourceValue);
                        break;
                    }

                    default:
                        break;
                }
            }
            else
            {
                target[targetFieldName] = valueConvertor?.Invoke(sourceValue);
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

                target[targetColumnName] = await htmlProcessor.ProcessHtml(site.SiteID, async (result, original) =>
                {
                    switch (result)
                    {
                        case { LinkKind: MediaLinkKind.Guid or MediaLinkKind.DirectMediaPath, MediaKind: MediaKind.MediaFile }:
                        {
                            var mediaFile = MediaHelper.GetMediaFile(result, modelFacade);
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

    #region "Page template & page widget walkers"

    private void WalkAreas(int siteId, List<EditableAreaConfiguration> areas, out bool needsDeferredPatch)
    {
        needsDeferredPatch = false;
        foreach (var area in areas)
        {
            logger.LogTrace("Walk area {Identifier}", area.Identifier);

            if (area.Sections is { Count: > 0 })
            {
                WalkSections(siteId, area.Sections, out bool ndp);
                needsDeferredPatch = ndp || needsDeferredPatch;
            }
        }
    }

    private void WalkSections(int siteId, List<SectionConfiguration> sections, out bool needsDeferredPatch)
    {
        needsDeferredPatch = false;
        foreach (var section in sections)
        {
            logger.LogTrace("Walk section {TypeIdentifier}|{Identifier}", section.TypeIdentifier, section.Identifier);

            var sectionFcs = sourceInstanceContext.GetSectionFormComponents(siteId, section.TypeIdentifier);
            WalkProperties(siteId, section.Properties, sectionFcs, out bool ndp1);
            needsDeferredPatch = ndp1 || needsDeferredPatch;

            if (section.Zones is { Count: > 0 })
            {
                WalkZones(siteId, section.Zones, out bool ndp);
                needsDeferredPatch = ndp || needsDeferredPatch;
            }
        }
    }

    private void WalkZones(int siteId, List<ZoneConfiguration> zones, out bool needsDeferredPatch)
    {
        needsDeferredPatch = false;
        foreach (var zone in zones)
        {
            logger.LogTrace("Walk zone {Name}|{Identifier}", zone.Name, zone.Identifier);

            if (zone.Widgets is { Count: > 0 })
            {
                WalkWidgets(siteId, zone.Widgets, out bool ndp);
                needsDeferredPatch = ndp || needsDeferredPatch;
            }
        }
    }

    private void WalkWidgets(int siteId, List<WidgetConfiguration> widgets, out bool needsDeferredPatch)
    {
        needsDeferredPatch = false;
        foreach (var widget in widgets)
        {
            logger.LogTrace("Walk widget {TypeIdentifier}|{Identifier}", widget.TypeIdentifier, widget.Identifier);

            var widgetFcs = sourceInstanceContext.GetWidgetPropertyFormComponents(siteId, widget.TypeIdentifier);
            foreach (var variant in widget.Variants)
            {
                logger.LogTrace("Walk widget variant {Name}|{Identifier}", variant.Name, variant.Identifier);

                if (variant.Properties is { Count: > 0 })
                {
                    WalkProperties(siteId, variant.Properties, widgetFcs, out bool ndp);
                    needsDeferredPatch = ndp || needsDeferredPatch;
                }
            }
        }
    }

    private void WalkProperties(int siteId, JObject properties, List<EditingFormControlModel>? formControlModels, out bool needsDeferredPatch)
    {
        needsDeferredPatch = false;
        foreach ((string key, var value) in properties)
        {
            logger.LogTrace("Walk property {Name}|{Identifier}", key, value?.ToString());

            var editingFcm = formControlModels?.FirstOrDefault(x => x.PropertyName.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (editingFcm != null)
            {
                if (FieldMappingInstance.BuiltInModel.NotSupportedInKxpLegacyMode
                        .SingleOrDefault(x => x.OldFormComponent == editingFcm.FormComponentIdentifier) is var (oldFormComponent, newFormComponent))
                {
                    logger.LogTrace("Editing form component found {FormComponentName} => no longer supported {Replacement}", editingFcm.FormComponentIdentifier, newFormComponent);

                    switch (oldFormComponent)
                    {
                        case Kx13FormComponents.Kentico_MediaFilesSelector:
                        {
                            var mfis = new List<object>();
                            if (value?.ToObject<List<MediaFilesSelectorItem>>() is { Count: > 0 } items)
                            {
                                foreach (var mfsi in items)
                                {
                                    if (configuration.MigrateMediaToMediaLibrary)
                                    {
                                        if (entityIdentityFacade.Translate<IMediaFile>(mfsi.FileGuid, siteId) is { } mf && mediaFileFacade.GetMediaFile(mf.Identity) is { } mfi)
                                        {
                                            mfis.Add(new Kentico.Components.Web.Mvc.FormComponents.MediaFilesSelectorItem { FileGuid = mfi.FileGUID });
                                        }
                                    }
                                    else
                                    {
                                        var sourceMediaFile = modelFacade.SelectWhere<IMediaFile>("FileGUID = @mediaFileGuid AND FileSiteID = @fileSiteID", new SqlParameter("mediaFileGuid", mfsi.FileGuid), new SqlParameter("fileSiteID", siteId))
                                            .FirstOrDefault();
                                        if (sourceMediaFile != null)
                                        {
                                            var (ownerContentItemGuid, _) = assetFacade.GetRef(sourceMediaFile);
                                            mfis.Add(new ContentItemReference { Identifier = ownerContentItemGuid });
                                        }
                                    }
                                }


                                properties[key] = JToken.FromObject(items.Select(x => new Kentico.Components.Web.Mvc.FormComponents.MediaFilesSelectorItem { FileGuid = entityIdentityFacade.Translate<IMediaFile>(x.FileGuid, siteId).Identity })
                                    .ToList());
                            }

                            break;
                        }
                        case Kx13FormComponents.Kentico_PathSelector:
                        {
                            if (value?.ToObject<List<PathSelectorItem>>() is { Count: > 0 } items)
                            {
                                properties[key] = JToken.FromObject(items.Select(x => new Kentico.Components.Web.Mvc.FormComponents.PathSelectorItem { TreePath = x.NodeAliasPath }).ToList());
                            }

                            break;
                        }
                        case Kx13FormComponents.Kentico_AttachmentSelector when newFormComponent == FormComponents.AdminAssetSelectorComponent:
                        {
                            if (value?.ToObject<List<AttachmentSelectorItem>>() is { Count: > 0 } items)
                            {
                                var nv = new List<object>();
                                foreach (var asi in items)
                                {
                                    var attachment = modelFacade.SelectWhere<ICmsAttachment>("AttachmentSiteID = @attachmentSiteId AND AttachmentGUID = @attachmentGUID",
                                            new SqlParameter("attachmentSiteID", siteId),
                                            new SqlParameter("attachmentGUID", asi.FileGuid)
                                        )
                                        .FirstOrDefault();
                                    if (attachment != null)
                                    {
                                        switch (attachmentMigrator.MigrateAttachment(attachment).GetAwaiter().GetResult())
                                        {
                                            case MigrateAttachmentResultMediaFile { Success: true, MediaFileInfo: { } x }:
                                            {
                                                nv.Add(new AssetRelatedItem { Identifier = x.FileGUID, Dimensions = new AssetDimensions { Height = x.FileImageHeight, Width = x.FileImageWidth }, Name = x.FileName, Size = x.FileSize });
                                                break;
                                            }
                                            case MigrateAttachmentResultContentItem { Success: true, ContentItemGuid: { } contentItemGuid }:
                                            {
                                                nv.Add(new ContentItemReference { Identifier = contentItemGuid });
                                                break;
                                            }
                                            default:
                                            {
                                                logger.LogWarning("Attachment '{AttachmentGUID}' failed to migrate", asi.FileGuid);
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        logger.LogWarning("Attachment '{AttachmentGUID}' not found", asi.FileGuid);
                                    }
                                }

                                properties[key] = JToken.FromObject(nv);
                            }

                            logger.LogTrace("Value migrated from {Old} model to {New} model", oldFormComponent, newFormComponent);
                            break;
                        }
                        case Kx13FormComponents.Kentico_PageSelector when newFormComponent == FormComponents.Kentico_Xperience_Admin_Websites_WebPageSelectorComponent:
                        {
                            if (value?.ToObject<List<PageSelectorItem>>() is { Count: > 0 } items)
                            {
                                properties[key] = JToken.FromObject(items.Select(x => new WebPageRelatedItem { WebPageGuid = spoiledGuidContext.EnsureNodeGuid(x.NodeGuid, siteId) }).ToList());
                            }

                            logger.LogTrace("Value migrated from {Old} model to {New} model", oldFormComponent, newFormComponent);
                            break;
                        }

                        default:
                            break;
                    }
                }
                else if (FieldMappingInstance.BuiltInModel.SupportedInKxpLegacyMode.Contains(editingFcm.FormComponentIdentifier))
                {
                    // OK
                    logger.LogTrace("Editing form component found {FormComponentName} => supported in legacy mode", editingFcm.FormComponentIdentifier);
                }
                else
                {
                    // unknown control, probably custom
                    logger.LogTrace("Editing form component found {FormComponentName} => custom or inlined component, don't forget to migrate code accordingly", editingFcm.FormComponentIdentifier);
                }
            }

            if ("NodeAliasPath".Equals(key, StringComparison.InvariantCultureIgnoreCase))
            {
                needsDeferredPatch = true;
                properties["TreePath"] = value;
                properties.Remove(key);
            }
        }
    }

    #endregion
}
