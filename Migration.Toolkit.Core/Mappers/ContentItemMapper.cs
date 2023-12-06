using System.Diagnostics;
using CMS.FormEngine;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Services;
using Migration.Toolkit.Core.Services.CmsClass;
using Migration.Toolkit.Core.Services.CmsRelationship;
using CMS.ContentEngine;
using CMS.MediaLibrary;
using CMS.Websites;
using Kentico.Xperience.UMT.Model;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Core.Auxiliary;
using Migration.Toolkit.KX13.Auxiliary;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Api.Auxiliary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Migration.Toolkit.Core.Mappers;

using CMS.ContentEngine.Internal;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Services;
using Migration.Toolkit.Common.Services.Ipc;

public record CmsTreeMapperSource(CmsTree CmsTree, string SafeNodeName, Guid SiteGuid, Guid? NodeParentGuid,
    Dictionary<string, Guid> CultureToLanguageGuid,
    string TargetFormDefinition,
    string SourceFormDefinition,
    List<CmsDocument> MigratedDocuments
);

public class ContentItemMapper : UmtMapperBase<CmsTreeMapperSource>
{
    private const string CLASS_FIELD_CONTROL_NAME = "controlname";

    private readonly ILogger<ContentItemMapper> _logger;
    private readonly CoupledDataService _coupledDataService;
    private readonly ClassService _classService;
    private readonly AttachmentMigrator _attachmentMigrator;
    private readonly CmsRelationshipService _relationshipService;
    private readonly SourceInstanceContext _sourceInstanceContext;
    private readonly FieldMigrationService _fieldMigrationService;
    private readonly KxpMediaFileFacade _mediaFileFacade;
    private readonly KeyMappingContext _keyMappingContext;
    private readonly KxpPageFacade _pageFacade;

    public ContentItemMapper(
        ILogger<ContentItemMapper> logger,
        CoupledDataService coupledDataService,
        ClassService classService,
        AttachmentMigrator attachmentMigrator,
        CmsRelationshipService relationshipService,
        SourceInstanceContext sourceInstanceContext,
        FieldMigrationService fieldMigrationService,
        KxpMediaFileFacade mediaFileFacade,
        KeyMappingContext keyMappingContext,
        KxpPageFacade pageFacade
    )
    {
        _logger = logger;
        _coupledDataService = coupledDataService;
        _classService = classService;
        _attachmentMigrator = attachmentMigrator;
        _relationshipService = relationshipService;
        _sourceInstanceContext = sourceInstanceContext;
        _fieldMigrationService = fieldMigrationService;
        _mediaFileFacade = mediaFileFacade;
        _keyMappingContext = keyMappingContext;
        _pageFacade = pageFacade;
    }

    protected override IEnumerable<IUmtModel> MapInternal(CmsTreeMapperSource source)
    {
        var (cmsTree, safeNodeName, siteGuid, nodeParentGuid, cultureToLanguageGuid, targetFormDefinition, sourceFormDefinition, migratedDocuments) = source;

        var contentItemGuid = cmsTree.NodeGuid;
        yield return new ContentItemModel
        {
            ContentItemGUID = contentItemGuid,
            ContentItemName = safeNodeName,
            ContentItemIsReusable = false, // page is not reusable
            ContentItemIsSecured = cmsTree.IsSecuredNode ?? false,
            ContentItemDataClassGuid = cmsTree.NodeClass.ClassGuid,
            ContentItemChannelGuid = siteGuid,
        };

        foreach (var cmsDocument in migratedDocuments)
        {
            if (!cultureToLanguageGuid.TryGetValue(cmsDocument.DocumentCulture, out var languageGuid))
                // TODO tomas.krch: 2023-11-15 WARN about skipped document
                continue;

            var hasDraft = cmsDocument.DocumentPublishedVersionHistoryId is not null &&
                           cmsDocument.DocumentPublishedVersionHistoryId != cmsDocument.DocumentCheckedOutVersionHistoryId;

            var checkoutVersion = hasDraft
                ? cmsDocument.DocumentCheckedOutVersionHistory
                : null;

            var draftMigrated = false;
            if (checkoutVersion is { PublishFrom: null } draftVersion)
            {
                List<IUmtModel>? migratedDraft = null;
                try
                {
                    migratedDraft = MigrateDraft(draftVersion, cmsTree, sourceFormDefinition, targetFormDefinition, contentItemGuid, languageGuid).ToList();
                    draftMigrated = true;
                }
                catch
                {
                    _logger.LogWarning("Failed to migrate checkout version of document with DocumentID={CmsDocumentDocumentId} VersionHistoryID={CmsDocumentDocumentCheckedOutVersionHistoryId}", cmsDocument.DocumentId, cmsDocument.DocumentCheckedOutVersionHistoryId);
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
                { DocumentIsArchived: true } => VersionStatus.Archived,
                { DocumentPublishedVersionHistoryId: null, DocumentCheckedOutVersionHistoryId: null } => VersionStatus.Published,
                { DocumentPublishedVersionHistoryId: { } pubId, DocumentCheckedOutVersionHistoryId: { } chId } when pubId <= chId => VersionStatus.Published,
                { DocumentPublishedVersionHistoryId: null, DocumentCheckedOutVersionHistoryId: not null } => VersionStatus.InitialDraft,
                _ => draftMigrated ? VersionStatus.Published : VersionStatus.InitialDraft
            };

            string? contentItemCommonDataPageBuilderWidgets = cmsDocument.DocumentPageBuilderWidgets;
            string? contentItemCommonDataPageTemplateConfiguration = cmsDocument.DocumentPageTemplateConfiguration;
            PatchJsonDefinitions(source.CmsTree.NodeSiteId, ref contentItemCommonDataPageTemplateConfiguration, ref contentItemCommonDataPageBuilderWidgets);

            var commonDataModel = new ContentItemCommonDataModel
            {
                ContentItemCommonDataGUID = cmsDocument.DocumentGuid ?? throw new InvalidOperationException($"DocumentGUID is null"),
                ContentItemCommonDataContentItemGuid = contentItemGuid,
                ContentItemCommonDataContentLanguageGuid = languageGuid, // DocumentCulture -> language entity needs to be created and its ID used here
                ContentItemCommonDataVersionStatus = versionStatus,
                ContentItemCommonDataIsLatest = !draftMigrated, // Flag for latest record to know what to retrieve for the UI
                ContentItemCommonDataPageBuilderWidgets = contentItemCommonDataPageBuilderWidgets, // PatchPageBuilderWidgets(cmsDocument.DocumentPageBuilderWidgets, cmsTree.NodeID, "cms.node", out _),
                ContentItemCommonDataPageTemplateConfiguration = contentItemCommonDataPageTemplateConfiguration, // PatchPageTemplateConfiguration(cmsDocument.DocumentPageTemplateConfiguration, cmsTree.NodeID, "cms.node", out var _)
                // ContentItemDataGuid = cmsDocument.DocumentGuid
            };

            var dataModel = new ContentItemDataModel
            {
                ContentItemDataGUID = commonDataModel.ContentItemCommonDataGUID,
                ContentItemDataCommonDataGuid = commonDataModel.ContentItemCommonDataGUID,
                ContentItemContentTypeName = cmsTree.NodeClass.ClassName
            };

            if (cmsTree.NodeClass.ClassIsCoupledClass)
            {
                var fi = new FormInfo(targetFormDefinition);
                var sfi = new FormInfo(sourceFormDefinition);
                var primaryKeyName = "";
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

                var coupledDataRow = _coupledDataService.GetSourceCoupledDataRow(cmsTree.NodeClass.ClassTableName, primaryKeyName, cmsDocument.DocumentForeignKeyValue);
                MapCoupledDataFieldValues(dataModel.CustomProperties,
                    (columnName) => coupledDataRow?[columnName],
                    columnName => coupledDataRow?.ContainsKey(columnName) ?? false
                    , cmsTree, cmsDocument.DocumentId, fi.GetColumnNames(), sfi, false);
            }

            dataModel.CustomProperties.Add("DocumentName", cmsDocument.DocumentName);

            yield return commonDataModel;
            yield return dataModel;

            Guid? documentCreatedByUserGuid = null;
            if (_keyMappingContext.GetGuid<KX13M.CmsUser>(u => u.UserId, u => u.UserGuid, cmsDocument.DocumentCreatedByUserId) is (true, var createdByUserGuid))
            {
                documentCreatedByUserGuid = createdByUserGuid;
            }
            else
            {

            }

            Guid? documentModifiedByUserGuid = null;
            if (_keyMappingContext.GetGuid<KX13M.CmsUser>(u => u.UserId, u => u.UserGuid, cmsDocument.DocumentModifiedByUserId) is (true, var modifiedByUserGuid))
            {
                documentModifiedByUserGuid = modifiedByUserGuid;
            }
            else
            {

            }

            var languageMetadataInfo = new ContentItemLanguageMetadataModel
            {
                ContentItemLanguageMetadataGUID = cmsDocument.DocumentGuid,
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
                ContentItemLanguageMetadataContentLanguageGuid = languageGuid // DocumentCulture -> language entity needs to be created and its ID used here
            };
            yield return languageMetadataInfo;
        }

        // mapping of linked nodes is not supported
        Debug.Assert(cmsTree.NodeLinkedNodeId == null, "cmsTree.NodeLinkedNodeId == null");
        Debug.Assert(cmsTree.NodeLinkedNodeSiteId == null, "cmsTree.NodeLinkedNodeSiteId == null");

        yield return new WebPageItemModel
        {
            WebPageItemParentGuid = nodeParentGuid, // NULL => under root
            WebPageItemGUID = contentItemGuid,
            WebPageItemName = safeNodeName,
            WebPageItemTreePath = cmsTree.NodeAliasPath,
            WebPageItemWebsiteChannelGuid = siteGuid,
            WebPageItemContentItemGuid = contentItemGuid,
            WebPageItemOrder = cmsTree.NodeOrder ?? 0 // 0 is nullish value
        };
    }

    private void PatchJsonDefinitions(int sourceSiteId, ref string? pageTemplateConfiguration, ref string? pageBuilderWidgets)
    {
        if (_sourceInstanceContext.HasInfo)
        {
            if (pageTemplateConfiguration != null)
            {
                var pageTemplateConfigurationObj = JsonConvert.DeserializeObject<Migration.Toolkit.Core.Services.CmsClass.PageTemplateConfiguration>(pageTemplateConfiguration);
                if (pageTemplateConfigurationObj?.Identifier != null)
                {
                    _logger.LogTrace("Walk page template configuration {Identifier}", pageTemplateConfigurationObj.Identifier);

                    var pageTemplateConfigurationFcs =
                        _sourceInstanceContext.GetPageTemplateFormComponents(sourceSiteId, pageTemplateConfigurationObj?.Identifier);
                    if (pageTemplateConfigurationObj.Properties is { Count: > 0 })
                    {
                        WalkProperties(pageTemplateConfigurationObj.Properties, pageTemplateConfigurationFcs);
                    }

                    pageTemplateConfiguration = JsonConvert.SerializeObject(pageTemplateConfigurationObj);
                }
            }

            if (pageBuilderWidgets != null)
            {
                var areas = JsonConvert.DeserializeObject<Migration.Toolkit.Core.Services.CmsClass.EditableAreasConfiguration>(pageBuilderWidgets);
                if (areas?.EditableAreas is { Count : > 0 })
                {
                    WalkAreas(sourceSiteId, areas.EditableAreas);
                }

                pageBuilderWidgets = JsonConvert.SerializeObject(areas);
            }
        }
    }

    private IEnumerable<IUmtModel> MigrateDraft(KX13M.CmsVersionHistory checkoutVersion, KX13M.CmsTree cmsTree, string sourceFormClassDefinition, string targetFormDefinition, Guid contentItemGuid, Guid contentLanguageGuid)
    {
        var adapter = new NodeXmlAdapter(checkoutVersion.NodeXml);

        ContentItemCommonDataModel? commonDataModel = null;
        ContentItemDataModel? dataModel = null;
        try
        {
            var pageTemplateConfiguration = adapter.DocumentPageTemplateConfiguration;
            var pageBuildWidgets = adapter.DocumentPageBuilderWidgets;
            PatchJsonDefinitions(checkoutVersion.NodeSiteId, ref pageTemplateConfiguration, ref pageBuildWidgets);

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
                ContentItemCommonDataPageTemplateConfiguration = pageTemplateConfiguration,
            };

            dataModel = new ContentItemDataModel
            {
                ContentItemDataGUID = commonDataModel.ContentItemCommonDataGUID,
                ContentItemDataCommonDataGuid = commonDataModel.ContentItemCommonDataGUID,
                ContentItemContentTypeName = cmsTree.NodeClass.ClassName
            };

            if (cmsTree.NodeClass.ClassIsCoupledClass)
            {
                var fi = new FormInfo(targetFormDefinition);
                var sfi = new FormInfo(sourceFormClassDefinition);
                var primaryKeyName = "";
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

                MapCoupledDataFieldValues(dataModel.CustomProperties,
                    s => adapter.GetValue(s),
                    s => adapter.HasValueSet(s)
                    , cmsTree, adapter.DocumentID, fi.GetColumnNames(), sfi, true);
            }

            // supply document name
            dataModel.CustomProperties.Add("DocumentName", adapter.DocumentName);
        }
        catch(Exception ex)
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

    #region "Page template & page widget walkers"


    private void WalkAreas(int siteId, List<EditableAreaConfiguration> areas)
    {
        foreach (var area in areas)
        {
            _logger.LogTrace("Walk area {Identifier}", area.Identifier);

            if (area.Sections is { Count: > 0 })
            {
                WalkSections(siteId, area.Sections);
            }
        }
    }

    private void WalkSections(int siteId, List<SectionConfiguration> sections)
    {
        foreach (var section in sections)
        {
            _logger.LogTrace("Walk section {TypeIdentifier}|{Identifier}", section.TypeIdentifier, section.Identifier);

            // TODO tk: 2022-09-14 find other acronym for FormComponents
            var sectionFcs = _sourceInstanceContext.GetSectionFormComponents(siteId, section.TypeIdentifier);
            WalkProperties(section.Properties, sectionFcs);

            if (section.Zones is { Count: > 0 })
            {
                WalkZones(siteId, section.Zones);
            }
        }
    }

    private void WalkZones(int siteId, List<ZoneConfiguration> zones)
    {
        foreach (var zone in zones)
        {
            _logger.LogTrace("Walk zone {Name}|{Identifier}", zone.Name, zone.Identifier);

            if (zone.Widgets is { Count: > 0 })
            {
                WalkWidgets(siteId, zone.Widgets);
            }
        }
    }

    private void WalkWidgets(int siteId, List<WidgetConfiguration> widgets)
    {
        foreach (var widget in widgets)
        {
            _logger.LogTrace("Walk widget {TypeIdentifier}|{Identifier}", widget.TypeIdentifier, widget.Identifier);

            var widgetFcs = _sourceInstanceContext.GetWidgetPropertyFormComponents(siteId, widget.TypeIdentifier);
            foreach (var variant in widget.Variants)
            {
                _logger.LogTrace("Walk widget variant {Name}|{Identifier}", variant.Name, variant.Identifier);

                if (variant.Properties is { Count: > 0 })
                {
                    WalkProperties(variant.Properties, widgetFcs);
                }
            }
        }
    }

    private void WalkProperties(JObject properties, List<EditingFormControlModel>? formControlModels)
    {
        foreach (var (key, value) in properties)
        {
            _logger.LogTrace("Walk property {Name}|{Identifier}", key, value?.ToString());

            var editingFcm = formControlModels?.FirstOrDefault(x => x.PropertyName.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (editingFcm != null)
            {
                if (FieldMappingInstance.BuiltInModel.NotSupportedInKxpLegacyMode
                        .SingleOrDefault(x => x.OldFormComponent == editingFcm.FormComponentIdentifier) is var (oldFormComponent, newFormComponent))
                {
                    _logger.LogTrace("Editing form component found {FormComponentName} => no longer supported {Replacement}", editingFcm.FormComponentIdentifier, newFormComponent);

                    switch (oldFormComponent)
                    {
                        case Kx13FormComponents.Kentico_PathSelector:
                        {
                            // TODO tomas.krch: 2023-11-15 migrate?
                            break;
                        }
                        case Kx13FormComponents.Kentico_AttachmentSelector when newFormComponent == FormComponents.AdminAssetSelectorComponent:
                        {
                            if (value?.ToObject<List<AttachmentSelectorItem>>() is { Count: > 0 } items)
                            {
                                properties[key] = JToken.FromObject(items.Select(x => new AssetRelatedItem
                                {
                                    Identifier = x.FileGuid
                                }).ToList());
                            }
                            _logger.LogTrace("Value migrated from {Old} model to {New} model", oldFormComponent, newFormComponent);
                            break;
                        }
                        case Kx13FormComponents.Kentico_PageSelector when newFormComponent == FormComponents.AdminPageSelectorComponent:
                        {
                            if (value?.ToObject<List<PageSelectorItem>>() is { Count: > 0 } items)
                            {
                                properties[key] = JToken.FromObject(items.Select(x => new WebPageRelatedItem
                                {
                                    WebPageGuid = x.NodeGuid
                                }).ToList());
                            }
                            _logger.LogTrace("Value migrated from {Old} model to {New} model", oldFormComponent, newFormComponent);
                            break;
                        }
                    }
                }
                else if (FieldMappingInstance.BuiltInModel.SupportedInKxpLegacyMode.Contains(editingFcm.FormComponentIdentifier))
                {
                    // OK
                    _logger.LogTrace("Editing form component found {FormComponentName} => supported in legacy mode", editingFcm.FormComponentIdentifier);
                }
                else
                {
                    // unknown control, probably custom
                    _logger.LogTrace("Editing form component found {FormComponentName} => custom or inlined component, don't forget to migrate code accordingly", editingFcm.FormComponentIdentifier);
                }
            }
        }
    }

    #endregion

    private void MapCoupledDataFieldValues(
        Dictionary<string, object?> target,
        Func<string, object> getSourceValue,
        Func<string, bool> containsSourceValue,
        CmsTree cmsTree,
        int? documentId,
        List<string> newColumnNames,
        FormInfo oldFormInfo,
        bool migratingFromVersionHistory
        )
    {
        Debug.Assert(cmsTree.NodeClass.ClassTableName != null, "cmsTree.NodeClass.ClassTableName != null");

        foreach (var columnName in newColumnNames)
        {
            if (
                columnName.Equals("ContentItemDataID", StringComparison.InvariantCultureIgnoreCase) ||
                columnName.Equals("ContentItemDataCommonDataID", StringComparison.InvariantCultureIgnoreCase) ||
                columnName.Equals("ContentItemDataGUID", StringComparison.InvariantCultureIgnoreCase) ||
                columnName.Equals("DocumentName", StringComparison.InvariantCultureIgnoreCase)
            )
            {
                continue;
            }

            if (!containsSourceValue(columnName))
            {
                if (migratingFromVersionHistory)
                {
                    _logger.LogDebug("Value is not contained in source, field '{Field}' (possibly because version existed before field was added to class form)", columnName);
                }
                else
                {
                    _logger.LogWarning("Value is not contained in source, field '{Field}'", columnName);
                }
                continue;
            }


            var field = oldFormInfo.GetFormField(columnName);
            var controlName = field.Settings[CLASS_FIELD_CONTROL_NAME]?.ToString()?.ToLowerInvariant();

            var value = getSourceValue(columnName);
            target[columnName] = value;

            MediaFileInfo?[]? mfis = null;
            bool hasMigratedMediaFile = false;

            var fieldMigration = _fieldMigrationService.GetFieldMigration(field.DataType, controlName, columnName);
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
                            switch (_attachmentMigrator.TryMigrateAttachmentByPath(path, $"__{columnName}"))
                            {
                                case (true, _, var mediaFileInfo, _):
                                {
                                    mfis = new[] { mediaFileInfo };
                                    hasMigratedMediaFile = true;
                                    break;
                                }
                                default:
                                {
                                    _logger.LogTrace("Unsuccessful attachment migration '{Field}': '{Value}'", columnName, path);
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
                            _attachmentMigrator.MigrateAttachment(mg, $"__{columnName}") is (_, _, var mediaFileInfo, _) { Success: true })
                        {
                            mfis = new[] { mediaFileInfo };
                            hasMigratedMediaFile = true;
                            _logger.LogTrace("MediaFile migrated from attachment '{Field}': '{Value}'", columnName, mg);
                        }

                        if (mediaKind == MediaKind.MediaFile && _mediaFileFacade.GetMediaFile(mg) is { } mfi)
                        {
                            mfis = new[] { mfi };
                            hasMigratedMediaFile = true;
                            _logger.LogTrace("MediaFile migrated from media file '{Field}': '{Value}'", columnName, mg);
                        }
                    }
                }
                else if (_classService.GetFormControlDefinition(controlName) is { } formControl)
                {
                    switch (formControl)
                    {
                        case { UserControlForFile: true }:
                        {
                            if (value is Guid attachmentGuid)
                            {
                                var (success, _, mediaFileInfo, mediaLibraryInfo) = _attachmentMigrator.MigrateAttachment(attachmentGuid, $"__{columnName}");
                                if (success && mediaFileInfo != null)
                                {
                                    mfis = new[] { mediaFileInfo };
                                    hasMigratedMediaFile = true;
                                    _logger.LogTrace("MediaFile migrated from attachment '{Field}': '{Value}'", columnName, attachmentGuid);
                                }
                            }
                            else if (value is string attachmentGuidStr && Guid.TryParse(attachmentGuidStr, out attachmentGuid))
                            {
                                var (success, _, mediaFileInfo, mediaLibraryInfo) = _attachmentMigrator.MigrateAttachment(attachmentGuid, $"__{columnName}");
                                if (success && mediaFileInfo != null)
                                {
                                    mfis = new[] { mediaFileInfo };
                                    hasMigratedMediaFile = true;
                                    _logger.LogTrace("MediaFile migrated from attachment '{Field}': '{Value}' (parsed)", columnName, attachmentGuid);
                                }
                            }

                            break;
                        }
                        case { UserControlForDocAttachments: true }:
                        {
                            if (documentId is { } docId)
                            {
                                var migratedAttachments =
                                    _attachmentMigrator.MigrateGroupedAttachments(docId, field.Guid, field.Name);

                                mfis = migratedAttachments
                                    .Where(x => x.MediaFileInfo != null)
                                    .Select(x => x.MediaFileInfo).ToArray();
                                hasMigratedMediaFile = true;
                            }

                            break;
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("Unable to map value based on selected migration '{Migration}', value: '{Value}'", fieldMigration, value);
                    continue;
                }

                if (hasMigratedMediaFile && mfis is { Length: > 0 })
                {
                    target.SetValueAsJson(columnName,
                        mfis.Select(x => new AssetRelatedItem
                        {
                            Identifier = x.FileGUID, Dimensions = new AssetDimensions { Height = x.FileImageHeight, Width = x.FileImageWidth, }, Name = x.FileName, Size = x.FileSize
                        })
                    );
                }
                continue;
            }

            if (controlName != null)
            {
                if (fieldMigration.Actions?.Contains(TcaDirective.ConvertToPages) ?? false)
                {
                    // relation to other document
                    var convertedRelation = _relationshipService.GetNodeRelationships(cmsTree.NodeId)
                        .Select(r => new WebPageRelatedItem { WebPageGuid = r.RightNode.NodeGuid });

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
                        target[columnName] = pageReferenceJson.Replace("\"NodeGuid\"", "\"WebPageGuid\"");
                    }
                }
            }
            else
            {
                target[columnName] = value;
            }

        }
    }
}