using System.Diagnostics;

using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.Core;
using CMS.Core.Internal;
using CMS.FormEngine;
using CMS.MediaLibrary;
using CMS.Websites;

using Kentico.Xperience.UMT.Model;

using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Enumerations;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.Services;
using Migration.Toolkit.Common.Services.Ipc;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Api.Auxiliary;
using Migration.Toolkit.KXP.Api.Services.CmsClass;
using Migration.Toolkit.Source.Auxiliary;
using Migration.Toolkit.Source.Contexts;
using Migration.Toolkit.Source.Model;
using Migration.Toolkit.Source.Services;
using Migration.Toolkit.Source.Services.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Migration.Toolkit.Source.Mappers;

public record CmsTreeMapperSource(
    ICmsTree CmsTree,
    string SafeNodeName,
    Guid SiteGuid,
    Guid? NodeParentGuid,
    Dictionary<string, Guid> CultureToLanguageGuid,
    string TargetFormDefinition,
    string SourceFormDefinition,
    List<ICmsDocument> MigratedDocuments
);

public class ContentItemMapper(
    ILogger<ContentItemMapper> logger,
    CoupledDataService coupledDataService,
    ClassService classService,
    AttachmentMigrator attachmentMigrator,
    CmsRelationshipService relationshipService,
    SourceInstanceContext sourceInstanceContext,
    FieldMigrationService fieldMigrationService,
    KxpMediaFileFacade mediaFileFacade,
    ModelFacade modelFacade,
    ReusableSchemaService reusableSchemaService,
    DeferredPathService deferredPathService,
    SpoiledGuidContext spoiledGuidContext
) : UmtMapperBase<CmsTreeMapperSource>
{
    private const string CLASS_FIELD_CONTROL_NAME = "controlname";

    protected override IEnumerable<IUmtModel> MapInternal(CmsTreeMapperSource source)
    {
        (var cmsTree, string safeNodeName, var siteGuid, var nodeParentGuid, var cultureToLanguageGuid, string targetFormDefinition, string sourceFormDefinition, var migratedDocuments) = source;

        var nodeClass = modelFacade.SelectById<ICmsClass>(cmsTree.NodeClassID) ?? throw new InvalidOperationException($"Fatal: node class is missing, class id '{cmsTree.NodeClassID}'");

        var contentItemGuid = spoiledGuidContext.EnsureNodeGuid(cmsTree.NodeGUID, cmsTree.NodeSiteID, cmsTree.NodeID);
        yield return new ContentItemModel
        {
            ContentItemGUID = contentItemGuid,
            ContentItemName = safeNodeName,
            ContentItemIsReusable = false, // page is not reusable
            ContentItemIsSecured = cmsTree.IsSecuredNode ?? false,
            ContentItemDataClassGuid = nodeClass.ClassGUID,
            ContentItemChannelGuid = siteGuid
        };

        var websiteChannelInfo = WebsiteChannelInfoProvider.ProviderObject.Get(siteGuid);
        var treePathConvertor = TreePathConvertor.GetSiteConverter(websiteChannelInfo.WebsiteChannelID);
        (bool treePathIsDifferent, string treePath) = treePathConvertor.ConvertAndEnsureUniqueness(cmsTree.NodeAliasPath).GetAwaiter().GetResult();
        if (treePathIsDifferent)
        {
            logger.LogInformation($"Original node alias path '{cmsTree.NodeAliasPath}' of '{cmsTree.NodeName}' item was converted to '{treePath}' since the value does not allow original range of allowed characters.");
        }

        foreach (var cmsDocument in migratedDocuments)
        {
            if (!cultureToLanguageGuid.TryGetValue(cmsDocument.DocumentCulture, out var languageGuid))
            {
                // TODO tomas.krch: 2023-11-15 WARN about skipped document
                continue;
            }

            bool hasDraft = cmsDocument.DocumentPublishedVersionHistoryID is not null &&
                            cmsDocument.DocumentPublishedVersionHistoryID != cmsDocument.DocumentCheckedOutVersionHistoryID;

            var checkoutVersion = hasDraft
                ? modelFacade.SelectById<ICmsVersionHistory>(cmsDocument.DocumentCheckedOutVersionHistoryID)
                : null;

            bool draftMigrated = false;
            if (checkoutVersion is { PublishFrom: null } draftVersion)
            {
                List<IUmtModel>? migratedDraft = null;
                try
                {
                    migratedDraft = MigrateDraft(draftVersion, cmsTree, sourceFormDefinition, targetFormDefinition, contentItemGuid, languageGuid, nodeClass, websiteChannelInfo).ToList();
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

            DateTime? scheduledPublishWhen = null;
            DateTime? scheduleUnpublishWhen = null;

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

            string? contentItemCommonDataPageBuilderWidgets = null;
            string? contentItemCommonDataPageTemplateConfiguration = null;
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

            PatchJsonDefinitions(source.CmsTree.NodeSiteID, ref contentItemCommonDataPageTemplateConfiguration, ref contentItemCommonDataPageBuilderWidgets, out bool ndp);

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
                    nodeClass.ClassName,
                    websiteChannelInfo.WebsiteChannelID
                );
            }

            var dataModel = new ContentItemDataModel { ContentItemDataGUID = commonDataModel.ContentItemCommonDataGUID, ContentItemDataCommonDataGuid = commonDataModel.ContentItemCommonDataGUID, ContentItemContentTypeName = nodeClass.ClassName };

            var fi = new FormInfo(targetFormDefinition);
            if (nodeClass.ClassIsCoupledClass)
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
                var sourceColumns = commonFields
                    .Select(cf => ReusableSchemaService.RemoveClassPrefix(nodeClass.ClassName, cf.Name))
                    .Union(fi.GetColumnNames(false))
                    .Except([CmsClassMapper.GetLegacyDocumentName(fi, nodeClass.ClassName)])
                    .ToList();

                var coupledDataRow = coupledDataService.GetSourceCoupledDataRow(nodeClass.ClassTableName, primaryKeyName, cmsDocument.DocumentForeignKeyValue);
                MapCoupledDataFieldValues(dataModel.CustomProperties,
                    columnName => coupledDataRow?[columnName],
                    columnName => coupledDataRow?.ContainsKey(columnName) ?? false,
                    cmsTree, cmsDocument.DocumentID, sourceColumns, sfi, fi, false, nodeClass
                );

                foreach (var formFieldInfo in commonFields)
                {
                    string originalFieldName = ReusableSchemaService.RemoveClassPrefix(nodeClass.ClassName, formFieldInfo.Name);
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

            if (CmsClassMapper.GetLegacyDocumentName(fi, nodeClass.ClassName) is { } legacyDocumentNameFieldName)
            {
                if (reusableSchemaService.IsConversionToReusableFieldSchemaRequested(nodeClass.ClassName))
                {
                    string fieldName = ReusableSchemaService.GetUniqueFieldName(nodeClass.ClassName, legacyDocumentNameFieldName);
                    commonDataModel.CustomProperties.Add(fieldName, cmsDocument.DocumentName);
                }
                else
                {
                    dataModel.CustomProperties.Add(legacyDocumentNameFieldName, cmsDocument.DocumentName);
                }
            }

            yield return commonDataModel;
            yield return dataModel;

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
                var pageTemplateConfigurationObj = JsonConvert.DeserializeObject<PageTemplateConfiguration>(pageTemplateConfiguration);
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
        Guid contentLanguageGuid, ICmsClass nodeClass, WebsiteChannelInfo websiteChannelInfo)
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
                    nodeClass.ClassName,
                    websiteChannelInfo.WebsiteChannelID
                );
            }

            dataModel = new ContentItemDataModel { ContentItemDataGUID = commonDataModel.ContentItemCommonDataGUID, ContentItemDataCommonDataGuid = commonDataModel.ContentItemCommonDataGUID, ContentItemContentTypeName = nodeClass.ClassName };

            if (nodeClass.ClassIsCoupledClass)
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
                    .Select(cf => ReusableSchemaService.RemoveClassPrefix(nodeClass.ClassName, cf.Name))
                    .Union(fi.GetColumnNames(false))
                    .Except([CmsClassMapper.GetLegacyDocumentName(fi, nodeClass.ClassName)])
                    .ToList();

                MapCoupledDataFieldValues(dataModel.CustomProperties,
                    s => adapter.GetValue(s),
                    s => adapter.HasValueSet(s)
                    , cmsTree, adapter.DocumentID, sourceColumns, sfi, fi, true, nodeClass);

                foreach (var formFieldInfo in commonFields)
                {
                    string originalFieldName = ReusableSchemaService.RemoveClassPrefix(nodeClass.ClassName, formFieldInfo.Name);
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
            if (reusableSchemaService.IsConversionToReusableFieldSchemaRequested(nodeClass.ClassName))
            {
                string fieldName = ReusableSchemaService.GetUniqueFieldName(nodeClass.ClassName, "DocumentName");
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

    private void MapCoupledDataFieldValues(
        Dictionary<string, object?> target,
        Func<string, object?> getSourceValue,
        Func<string, bool> containsSourceValue,
        ICmsTree cmsTree,
        int? documentId,
        List<string> newColumnNames,
        FormInfo oldFormInfo,
        FormInfo newFormInfo,
        bool migratingFromVersionHistory,
        ICmsClass nodeClass
    )
    {
        Debug.Assert(nodeClass.ClassTableName != null, "cmsTree.NodeClass.ClassTableName != null");

        foreach (string columnName in newColumnNames)
        {
            if (
                columnName.Equals("ContentItemDataID", StringComparison.InvariantCultureIgnoreCase) ||
                columnName.Equals("ContentItemDataCommonDataID", StringComparison.InvariantCultureIgnoreCase) ||
                columnName.Equals("ContentItemDataGUID", StringComparison.InvariantCultureIgnoreCase) ||
                columnName.Equals(CmsClassMapper.GetLegacyDocumentName(newFormInfo, nodeClass.ClassName), StringComparison.InvariantCultureIgnoreCase)
            )
            {
                continue;
            }

#pragma warning disable CS0618 // Type or member is obsolete
            if (oldFormInfo.GetFormField(columnName)?.External is true)
#pragma warning restore CS0618 // Type or member is obsolete
            {
                continue;
            }

            if (!containsSourceValue(columnName))
            {
                if (migratingFromVersionHistory)
                {
                    logger.LogDebug("Value is not contained in source, field '{Field}' (possibly because version existed before field was added to class form)", columnName);
                }
                else
                {
                    logger.LogWarning("Value is not contained in source, field '{Field}'", columnName);
                }

                continue;
            }


            var field = oldFormInfo.GetFormField(columnName);
            string? controlName = field.Settings[CLASS_FIELD_CONTROL_NAME]?.ToString()?.ToLowerInvariant();

            object? value = getSourceValue(columnName);
            target[columnName] = value;

            MediaFileInfo?[]? mfis = null;
            bool hasMigratedMediaFile = false;

            var fieldMigration = fieldMigrationService.GetFieldMigration(field.DataType, controlName, columnName);
            if (fieldMigration?.Actions?.Contains(TcaDirective.ConvertToAsset) ?? false)
            {
                if (value is string link &&
                    MediaHelper.MatchMediaLink(link) is (true, var mediaLinkKind, var mediaKind, var path, var mediaGuid))
                {
                    if (mediaLinkKind == MediaLinkKind.Path)
                    {
                        // path needs to be converted to GUID
                        if (mediaKind == MediaKind.Attachment && path != null)
                        {
                            switch (attachmentMigrator.TryMigrateAttachmentByPath(path, $"__{columnName}"))
                            {
                                case (true, _, var mediaFileInfo, _):
                                {
                                    mfis = new[] { mediaFileInfo };
                                    hasMigratedMediaFile = true;
                                    break;
                                }
                                default:
                                {
                                    logger.LogTrace("Unsuccessful attachment migration '{Field}': '{Value}'", columnName, path);
                                    break;
                                }
                            }
                        }

                        if (mediaKind == MediaKind.MediaFile)
                        {
                            // _mediaFileFacade.GetMediaFile()
                            // TODO tomas.krch: 2023-03-07 get media file by path
                            // attachmentDocument.DocumentNode.NodeAliasPath
                        }
                    }

                    if (mediaGuid is { } mg)
                    {
                        if (mediaKind == MediaKind.Attachment &&
                            attachmentMigrator.MigrateAttachment(mg, $"__{columnName}") is (_, _, var mediaFileInfo, _) { Success: true })
                        {
                            mfis = new[] { mediaFileInfo };
                            hasMigratedMediaFile = true;
                            logger.LogTrace("MediaFile migrated from attachment '{Field}': '{Value}'", columnName, mg);
                        }

                        if (mediaKind == MediaKind.MediaFile && mediaFileFacade.GetMediaFile(mg) is { } mfi)
                        {
                            mfis = new[] { mfi };
                            hasMigratedMediaFile = true;
                            logger.LogTrace("MediaFile migrated from media file '{Field}': '{Value}'", columnName, mg);
                        }
                    }
                }
                else if (classService.GetFormControlDefinition(controlName) is { } formControl)
                {
                    switch (formControl)
                    {
                        case { UserControlForFile: true }:
                        {
                            if (value is Guid attachmentGuid)
                            {
                                (bool success, _, var mediaFileInfo, var mediaLibraryInfo) = attachmentMigrator.MigrateAttachment(attachmentGuid, $"__{columnName}");
                                if (success && mediaFileInfo != null)
                                {
                                    mfis = new[] { mediaFileInfo };
                                    hasMigratedMediaFile = true;
                                    logger.LogTrace("MediaFile migrated from attachment '{Field}': '{Value}'", columnName, attachmentGuid);
                                }
                            }
                            else if (value is string attachmentGuidStr && Guid.TryParse(attachmentGuidStr, out attachmentGuid))
                            {
                                (bool success, _, var mediaFileInfo, var mediaLibraryInfo) = attachmentMigrator.MigrateAttachment(attachmentGuid, $"__{columnName}");
                                if (success && mediaFileInfo != null)
                                {
                                    mfis = new[] { mediaFileInfo };
                                    hasMigratedMediaFile = true;
                                    logger.LogTrace("MediaFile migrated from attachment '{Field}': '{Value}' (parsed)", columnName, attachmentGuid);
                                }
                            }

                            break;
                        }
                        case { UserControlForDocAttachments: true }:
                        {
                            if (documentId is { } docId)
                            {
                                var migratedAttachments =
                                    attachmentMigrator.MigrateGroupedAttachments(docId, field.Guid, field.Name);

                                mfis = migratedAttachments
                                    .Where(x => x.MediaFileInfo != null)
                                    .Select(x => x.MediaFileInfo).ToArray();
                                hasMigratedMediaFile = true;
                            }

                            break;
                        }

                        default:
                            break;
                    }
                }
                else
                {
                    logger.LogWarning("Unable to map value based on selected migration '{Migration}', value: '{Value}'", fieldMigration, value);
                    continue;
                }

                if (hasMigratedMediaFile && mfis is { Length: > 0 })
                {
                    target.SetValueAsJson(columnName,
                        mfis.Select(x => new AssetRelatedItem { Identifier = x.FileGUID, Dimensions = new AssetDimensions { Height = x.FileImageHeight, Width = x.FileImageWidth }, Name = x.FileName, Size = x.FileSize })
                    );
                }

                continue;
            }

            if (controlName != null)
            {
                if (fieldMigration.Actions?.Contains(TcaDirective.ConvertToPages) ?? false)
                {
                    // relation to other document
                    var convertedRelation = relationshipService.GetNodeRelationships(cmsTree.NodeID, nodeClass.ClassName, field.Guid)
                        .Select(r => new WebPageRelatedItem { WebPageGuid = spoiledGuidContext.EnsureNodeGuid(r.RightNode.NodeGUID, r.RightNode.NodeSiteID, r.RightNode.NodeID) });

                    target.SetValueAsJson(columnName, convertedRelation);
                }
                else
                {
                    // leave as is
                    target[columnName] = value;
                }

                if (fieldMigration.TargetFormComponent == "webpages")
                {
                    if (value is string pageReferenceJson)
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

                        target[columnName] = parsed.ToString().Replace("\"NodeGuid\"", "\"WebPageGuid\"");
                    }
                }
            }
            else
            {
                target[columnName] = value;
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

    private record WalkerContext(int SiteId);

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

            // TODO tk: 2022-09-14 find other acronym for FormComponents
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
                        // case Kx13FormComponents.Kentico_PathSelector:
                        // {
                        //     // new PathSelectorItem()
                        //     break;
                        // }
                        case Kx13FormComponents.Kentico_AttachmentSelector when newFormComponent == FormComponents.AdminAssetSelectorComponent:
                        {
                            if (value?.ToObject<List<AttachmentSelectorItem>>() is { Count: > 0 } items)
                            {
                                properties[key] = JToken.FromObject(items.Select(x => new AssetRelatedItem { Identifier = x.FileGuid }).ToList());
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
