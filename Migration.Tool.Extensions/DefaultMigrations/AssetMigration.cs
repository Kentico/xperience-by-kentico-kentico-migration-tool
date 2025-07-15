using System.Xml.Linq;
using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using CMS.MediaLibrary;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;
using Migration.Tool.Common.Enumerations;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.Services;
using Migration.Tool.KXP.Api;
using Migration.Tool.KXP.Api.Auxiliary;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source;
using Migration.Tool.Source.Auxiliary;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Helpers;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Services;
using Newtonsoft.Json;

namespace Migration.Tool.Extensions.DefaultMigrations;

public class AssetMigration(
    ILogger<AssetMigration> logger,
    ClassService classService,
    IAttachmentMigrator attachmentMigrator,
    ModelFacade modelFacade,
    KxpMediaFileFacade mediaFileFacade,
    ToolConfiguration configuration,
    EntityIdentityFacade entityIdentityFacade,
    IAssetFacade assetFacade,
    MediaLinkServiceFactory mediaLinkServiceFactory,
    InvokedCommands invokedCommands,
    IImporter importer,
    ContentFolderService contentFolderService) : IFieldMigration
{
    public int Rank => 100_000;

    public bool ShallMigrate(FieldMigrationContext context) =>
        (
            context.SourceDataType is KsFieldDataType.DocAttachments or KsFieldDataType.File ||
            Kx13FormControls.UserControlForText.MediaSelectionControl.Equals(context.SourceFormControl, StringComparison.InvariantCultureIgnoreCase)
        ) &&
        context.SourceObjectContext
            is DocumentSourceObjectContext or CustomTableSourceObjectContext
            // this migration also handles empty object context - for example when migrating data class, empty context is supplied
            or EmptySourceObjectContext;

    private readonly Lazy<IEnumerable<string>> languageNames = new(() => Service.Resolve<IInfoProvider<ContentLanguageInfo>>().Get().Select(x => x.ContentLanguageName));
    public async Task<FieldMigrationResult> MigrateValue(object? sourceValue, FieldMigrationContext context)
    {
        var contentItemCodeNameProvider = Service.Resolve<IContentItemCodeNameProvider>();

        (string? _, string? sourceFormControl, string? fieldName, var sourceObjectContext) = context;

        if (!invokedCommands.Commands.Any(x => x is MigrateMediaLibrariesCommand))
        {
            logger.LogError($"Trying to migrate asset field value {{FieldName}}, but command {MigrateMediaLibrariesCommand.Moniker} was not invoked", fieldName);
            return new(false, null);
        }

        ICmsSite? cmsSite;
        CMS.FormEngine.FormInfo? oldFormInfo;
        int? documentId;
        string valueKey;
        if (sourceObjectContext is DocumentSourceObjectContext docContext)
        {
            cmsSite = docContext.Site;
            oldFormInfo = docContext.OldFormInfo;
            documentId = docContext.DocumentId;
            valueKey = $"Document|{cmsSite.SiteGUID}|{documentId}";
        }
        else if (sourceObjectContext is CustomTableSourceObjectContext customTableContext)
        {
            cmsSite = null;
            oldFormInfo = null;
            documentId = null;
            valueKey = $"CustomTable|{customTableContext.UniqueKey}";
        }
        else
        {
            throw new ArgumentNullException(nameof(sourceObjectContext));
        }

        var field = oldFormInfo?.GetFormField(fieldName);

        List<object> mfis = [];
        bool hasMigratedAsset = false;
        if (sourceValue is string link &&
            mediaLinkServiceFactory.Create().MatchMediaLink(link, cmsSite?.SiteID) is (true, var mediaLinkKind, var mediaKind, var path, var mediaGuid, _, _) result)
        {
            if (mediaLinkKind == MediaLinkKind.Path)
            {
                // path needs to be converted to GUID
                if (mediaKind == MediaKind.Attachment && path != null)
                {
                    switch (await attachmentMigrator.TryMigrateAttachmentByPath(path, $"__{fieldName}"))
                    {
                        case MigrateAttachmentResultMediaFile(true, var x, _):
                        {
                            mfis = [new AssetRelatedItem { Identifier = x!.FileGUID, Dimensions = new AssetDimensions { Height = x.FileImageHeight, Width = x.FileImageWidth }, Name = x.FileName, Size = x.FileSize }];
                            hasMigratedAsset = true;
                            logger.LogTrace("'{FieldName}' migrated Match={Value}", fieldName, result);
                            break;
                        }
                        case MigrateAttachmentResultContentItem { Success: true, ContentItemGuid: { } contentItemGuid }:
                        {
                            mfis =
                            [
                                new ContentItemReference { Identifier = contentItemGuid }
                            ];
                            hasMigratedAsset = true;
                            logger.LogTrace("'{FieldName}' migrated Match={Value}", fieldName, result);
                            break;
                        }
                        default:
                        {
                            logger.LogTrace("Unsuccessful attachment migration '{Field}': '{Value}' - {Match}", fieldName, path, result);
                            break;
                        }
                    }
                }

                if (mediaKind == MediaKind.MediaFile)
                {
                    logger.LogTrace("'{FieldName}' Skipped Match={Value}", fieldName, result);
                }
            }

            if (mediaLinkKind == MediaLinkKind.DirectMediaPath)
            {
                if (mediaKind == MediaKind.MediaFile)
                {
                    var sourceMediaFile = MediaHelper.GetMediaFile(result, modelFacade, link, logger);
                    if (sourceMediaFile != null)
                    {
                        if (configuration.MigrateMediaToMediaLibrary)
                        {
                            if (entityIdentityFacade.Translate(sourceMediaFile) is { } mf && mediaFileFacade.GetMediaFile(mf.Identity) is { } x)
                            {
                                mfis = [new AssetRelatedItem { Identifier = x.FileGUID, Dimensions = new AssetDimensions { Height = x.FileImageHeight, Width = x.FileImageWidth }, Name = x.FileName, Size = x.FileSize }];
                                hasMigratedAsset = true;
                            }
                        }
                        else
                        {
                            var (ownerContentItemGuid, _) = assetFacade.GetRef(sourceMediaFile);
                            mfis =
                            [
                                new ContentItemReference { Identifier = ownerContentItemGuid }
                            ];
                            hasMigratedAsset = true;
                            logger.LogTrace("MediaFile migrated from media file '{Field}': '{Value}'", fieldName, result);
                        }
                    }
                }
            }

            if (mediaGuid is { } mg)
            {
                if (mediaKind == MediaKind.Attachment)
                {
                    switch (await attachmentMigrator.MigrateAttachment(mg, $"__{fieldName}", cmsSite!.SiteID))
                    {
                        case MigrateAttachmentResultMediaFile(true, var x, _):
                        {
                            mfis = [new AssetRelatedItem { Identifier = x!.FileGUID, Dimensions = new AssetDimensions { Height = x.FileImageHeight, Width = x.FileImageWidth }, Name = x.FileName, Size = x.FileSize }];
                            hasMigratedAsset = true;
                            logger.LogTrace("MediaFile migrated from attachment '{Field}': '{Value}'", fieldName, mg);
                            break;
                        }
                        case MigrateAttachmentResultContentItem { Success: true, ContentItemGuid: { } contentItemGuid }:
                        {
                            mfis =
                            [
                                new ContentItemReference { Identifier = contentItemGuid }
                            ];
                            hasMigratedAsset = true;
                            logger.LogTrace("Content item migrated from attachment '{Field}': '{Value}' to {ContentItemGUID}", fieldName, mg, contentItemGuid);
                            break;
                        }
                        default:
                        {
                            break;
                        }
                    }
                }

                if (mediaKind == MediaKind.MediaFile)
                {
                    var sourceMediaFile =
                        (cmsSite is not null
                            ? modelFacade.SelectWhere<IMediaFile>("FileGUID = @mediaFileGuid AND FileSiteID = @fileSiteID", new SqlParameter("mediaFileGuid", mg), new SqlParameter("fileSiteID", cmsSite.SiteID))
                            : modelFacade.SelectWhere<IMediaFile>("FileGUID = @mediaFileGuid", new SqlParameter("mediaFileGuid", mg))
                        ).FirstOrDefault();
                    if (sourceMediaFile != null)
                    {
                        if (configuration.MigrateMediaToMediaLibrary)
                        {
                            if (entityIdentityFacade.Translate(sourceMediaFile) is { } mf && mediaFileFacade.GetMediaFile(mf.Identity) is { } x)
                            {
                                mfis = [new AssetRelatedItem { Identifier = x.FileGUID, Dimensions = new AssetDimensions { Height = x.FileImageHeight, Width = x.FileImageWidth }, Name = x.FileName, Size = x.FileSize }];
                                hasMigratedAsset = true;
                            }
                        }
                        else
                        {
                            var (ownerContentItemGuid, _) = assetFacade.GetRef(sourceMediaFile);
                            mfis =
                            [
                                new ContentItemReference { Identifier = ownerContentItemGuid }
                            ];
                            hasMigratedAsset = true;
                            logger.LogTrace("MediaFile migrated from media file '{Field}': '{Value}'", fieldName, mg);
                        }
                    }
                }
            }
        }
        else if (Kx13FormControls.UserControlForText.MediaSelectionControl.Equals(sourceFormControl, StringComparison.InvariantCultureIgnoreCase) && sourceValue is string sourceUrl)
        {
            if (!configuration.MigrateMediaToMediaLibrary)
            {
                // If we're migrating assets to content hub, unmatched URL can be stored as legacy media link
                string folderPath = "/Legacy Media Links";
                var folderGuid = GuidHelper.CreateFolderGuid(folderPath);
                await contentFolderService.EnsureFolderStructure([new(folderGuid, folderGuid.ToString(), "Legacy Media Links", folderPath)]);
                const int nameLength = 60;    // number of characters to take from the end of source url as representative name
                string displayName = sourceUrl.Length >= nameLength ? sourceUrl[^nameLength..] : sourceUrl;
                string name = await contentItemCodeNameProvider.Get(displayName);
                var contentItemModel = new ContentItemSimplifiedModel
                {
                    CustomProperties = [],
                    ContentItemGUID = GuidHelper.CreateContentItemGuid($"MediaSelectorLink|{valueKey}|{fieldName}"),
                    ContentItemContentFolderGUID = folderGuid,
                    IsSecured = null,
                    ContentTypeName = AssetFacade.LegacyMediaLinkContentType.ClassName,
                    Name = name,
                    IsReusable = true,
                    LanguageData = languageNames.Value.Select(lang =>
                        new ContentItemLanguageData
                        {
                            LanguageName = lang,
                            DisplayName = displayName,
                            UserGuid = null,
                            VersionStatus = VersionStatus.Published,
                            ContentItemData = new Dictionary<string, object?>()
                            {
                                [AssetFacade.LegacyMediaLinkUrlField.Column!] = sourceUrl
                            }
                        }).ToList()
                };
                var importResult = await importer.ImportAsync(contentItemModel);
                if (importResult is { Success: true })
                {
                    logger.LogInformation($"Imported '{{Url}}' to content hub as {AssetFacade.LegacyMediaLinkContentType.ClassName}", sourceUrl);
                }
                else
                {
                    logger.LogError($"Failed to import '{{Url}}' to content hub as {AssetFacade.LegacyMediaLinkContentType.ClassName}: {{Exception}}", sourceUrl, importResult.Exception);
                }
                mfis =
                [
                    new ContentItemReference { Identifier = contentItemModel.ContentItemGUID.Value }
                ];
                hasMigratedAsset = true;
            }
            else
            {
                logger.LogWarning("Asset value '{Value}' of {FieldName} wasn't migrated. No matching asset in source instance could be identified. " +
                                  $"Migration as URL is not supported when {nameof(configuration.MigrateMediaToMediaLibrary)} = false. " +
                                  $"When {nameof(configuration.MigrateMediaToMediaLibrary)} = true, matched assets will be migrated as content items " +
                                  "and unmatched assets as original links.", sourceValue, fieldName);
            }
        }
        else if (classService.GetFormControlDefinition(sourceFormControl!) is { } formControl)
        {
            switch (formControl)
            {
                case { UserControlForFile: true }:
                {
                    if (sourceValue is Guid attachmentGuid)
                    {
                        switch (await attachmentMigrator.MigrateAttachment(attachmentGuid, $"__{fieldName}", cmsSite!.SiteID))
                        {
                            case MigrateAttachmentResultMediaFile(true, var mfi, _):
                            {
                                mfis = [new AssetRelatedItem { Identifier = mfi!.FileGUID, Dimensions = new AssetDimensions { Height = mfi.FileImageHeight, Width = mfi.FileImageWidth }, Name = mfi.FileName, Size = mfi.FileSize }];
                                hasMigratedAsset = true;
                                logger.LogTrace("MediaFile migrated from attachment '{Field}': '{Value}'", fieldName, attachmentGuid);
                                break;
                            }
                            case MigrateAttachmentResultContentItem { Success: true, ContentItemGuid: { } contentItemGuid }:
                            {
                                mfis =
                                [
                                    new ContentItemReference { Identifier = contentItemGuid }
                                ];
                                hasMigratedAsset = true;
                                logger.LogTrace("Content item migrated from attachment '{Field}': '{Value}' to {ContentItemGUID}", fieldName, attachmentGuid, contentItemGuid);
                                break;
                            }
                            default:
                            {
                                logger.LogTrace("'{FieldName}' UserControlForFile Success={Success} AttachmentGUID={attachmentGuid}", fieldName, false, attachmentGuid);
                                break;
                            }
                        }
                    }
                    else if (sourceValue is string attachmentGuidStr && Guid.TryParse(attachmentGuidStr, out attachmentGuid))
                    {
                        switch (await attachmentMigrator.MigrateAttachment(attachmentGuid, $"__{fieldName}", cmsSite!.SiteID))
                        {
                            case MigrateAttachmentResultMediaFile { Success: true, MediaFileInfo: { } x }:
                            {
                                mfis = [new AssetRelatedItem { Identifier = x.FileGUID, Dimensions = new AssetDimensions { Height = x.FileImageHeight, Width = x.FileImageWidth }, Name = x.FileName, Size = x.FileSize }];
                                hasMigratedAsset = true;
                                logger.LogTrace("MediaFile migrated from attachment '{Field}': '{Value}' (parsed)", fieldName, attachmentGuid);
                                break;
                            }
                            case MigrateAttachmentResultContentItem { Success: true, ContentItemGuid: { } contentItemGuid }:
                            {
                                mfis =
                                [
                                    new ContentItemReference { Identifier = contentItemGuid }
                                ];
                                hasMigratedAsset = true;
                                logger.LogTrace("Content item migrated from attachment '{Field}': '{Value}' to {ContentItemGUID}", fieldName, attachmentGuid, contentItemGuid);
                                break;
                            }
                            default:
                            {
                                logger.LogTrace("'{FieldName}' UserControlForFile Success={Success} AttachmentGUID={attachmentGuid}", fieldName, false, attachmentGuid);
                                break;
                            }
                        }
                    }
                    else
                    {
                        logger.LogTrace("'{FieldName}' UserControlForFile AttachmentGUID={Value}", fieldName, sourceValue);
                    }

                    break;
                }
                case { UserControlForDocAttachments: true }:
                {
                    // new AssetRelatedItem { Identifier = x.FileGUID, Dimensions = new AssetDimensions { Height = x.FileImageHeight, Width = x.FileImageWidth }, Name = x.FileName, Size = x.FileSize }
                    if (documentId is { } docId)
                    {
                        var mfisl = new List<object>();
                        await foreach (var migResult in attachmentMigrator.MigrateGroupedAttachments(docId, field!.Guid, field.Name))
                        {
                            switch (migResult)
                            {
                                case MigrateAttachmentResultMediaFile { Success: true, MediaFileInfo: { } x }:
                                {
                                    mfisl.Add(new AssetRelatedItem { Identifier = x.FileGUID, Dimensions = new AssetDimensions { Height = x.FileImageHeight, Width = x.FileImageWidth }, Name = x.FileName, Size = x.FileSize });
                                    hasMigratedAsset = true;
                                    break;
                                }
                                case MigrateAttachmentResultContentItem { Success: true, ContentItemGuid: { } contentItemGuid }:
                                {
                                    mfisl.Add(
                                        new ContentItemReference { Identifier = contentItemGuid }
                                    );
                                    hasMigratedAsset = true;
                                    logger.LogTrace("Content item migrated from document '{DocumentID}' attachment '{FiledName}' to {ContentItemGUID}", docId, field.Name, contentItemGuid);
                                    break;
                                }
                                default:
                                {
                                    hasMigratedAsset = false;
                                    break;
                                }
                            }
                        }

                        mfis = mfisl;
                    }
                    else
                    {
                        logger.LogTrace("'{FieldName}' UserControlForDocAttachments DocumentID={Value}", fieldName, documentId);
                    }

                    break;
                }
                default:
                    break;
            }
        }
        else
        {
            logger.LogWarning("Unable to map value based on selected migration, value: '{Value}'", sourceValue);
            return new FieldMigrationResult(false, null);
        }

        if (hasMigratedAsset && mfis is { Count: > 0 })
        {
            return new FieldMigrationResult(true, SerializationHelper.Serialize(mfis));
        }
        else if (DBNull.Value.Equals(sourceValue))
        {
            return new FieldMigrationResult(true, null);
        }
        else
        {
            logger.LogTrace("No assets migrated for '{FieldName}', value: '{Value}'", fieldName, sourceValue);
            return new FieldMigrationResult(false, null);
        }
    }

    public void MigrateFieldDefinition(FormDefinitionPatcher formDefinitionPatcher, XElement field, XAttribute? columnTypeAttr, string fieldDescriptor)
    {
        columnTypeAttr?.SetValue(configuration.MigrateMediaToMediaLibrary ? FieldDataType.Assets : FieldDataType.ContentItemReference);

        var settings = field.EnsureElement(FormDefinitionPatcher.FieldElemSettings);
        if (configuration.MigrateMediaToMediaLibrary)
        {
            settings.EnsureElement(FormDefinitionPatcher.SettingsElemControlname, e => e.Value = FormComponents.AdminAssetSelectorComponent);
            settings.EnsureElement(FormDefinitionPatcher.SettingsMaximumassets, maxAssets => maxAssets.Value = FormDefinitionPatcher.SettingsMaximumassetsFallback);
        }
        else
        {
            settings.EnsureElement(FormDefinitionPatcher.SettingsElemControlname, e => e.Value = FormComponents.AdminContentItemSelectorComponent);
            Guid[] allowedContentTypes = [AssetFacade.LegacyMediaFileContentType.ClassGUID!.Value, AssetFacade.LegacyMediaLinkContentType.ClassGUID!.Value, AssetFacade.LegacyAttachmentContentType.ClassGUID!.Value];
            settings.EnsureElement(FormDefinitionPatcher.AllowedContentItemTypeIdentifiers, e => e.Value = JsonConvert.SerializeObject(allowedContentTypes.Select(x => x.ToString()).ToArray()));
        }
    }
}
