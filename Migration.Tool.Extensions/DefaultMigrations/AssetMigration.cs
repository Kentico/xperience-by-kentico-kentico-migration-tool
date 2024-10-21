using System.Xml.Linq;
using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.MediaLibrary;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;
using Migration.Tool.Common.Enumerations;
using Migration.Tool.Common.Helpers;
using Migration.Tool.KXP.Api;
using Migration.Tool.KXP.Api.Auxiliary;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source;
using Migration.Tool.Source.Auxiliary;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Helpers;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Services;

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
    MediaLinkServiceFactory mediaLinkServiceFactory
) : IFieldMigration
{
    public int Rank => 100_000;

    public bool ShallMigrate(FieldMigrationContext context) =>
        (
            context.SourceDataType is KsFieldDataType.DocAttachments or KsFieldDataType.File ||
            Kx13FormControls.UserControlForText.MediaSelectionControl.Equals(context.SourceFormControl, StringComparison.InvariantCultureIgnoreCase)
        ) &&
        context.SourceObjectContext
            // this migration can handle only migration of documents to content items
            is DocumentSourceObjectContext
            // this migration also handles empty object context - for example when migrating data class, empty context is supplied
            or EmptySourceObjectContext;

    public async Task<FieldMigrationResult> MigrateValue(object? sourceValue, FieldMigrationContext context)
    {
        (string? _, string? sourceFormControl, string? fieldName, var sourceObjectContext) = context;
        if (sourceObjectContext is not DocumentSourceObjectContext(_, _, var cmsSite, var oldFormInfo, _, var documentId))
        {
            throw new ArgumentNullException(nameof(sourceObjectContext));
        }

        var field = oldFormInfo.GetFormField(fieldName);

        List<object> mfis = [];
        bool hasMigratedAsset = false;
        if (sourceValue is string link &&
            mediaLinkServiceFactory.Create().MatchMediaLink(link, cmsSite.SiteID) is (true, var mediaLinkKind, var mediaKind, var path, var mediaGuid, _, _) result)
        {
            if (mediaLinkKind == MediaLinkKind.Path)
            {
                // path needs to be converted to GUID
                if (mediaKind == MediaKind.Attachment && path != null)
                {
                    switch (await attachmentMigrator.TryMigrateAttachmentByPath(path, $"__{fieldName}"))
                    {
                        case MigrateAttachmentResultMediaFile(true, _, var x, _):
                        {
                            mfis = [new AssetRelatedItem { Identifier = x.FileGUID, Dimensions = new AssetDimensions { Height = x.FileImageHeight, Width = x.FileImageWidth }, Name = x.FileName, Size = x.FileSize }];
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
                    var sourceMediaFile = MediaHelper.GetMediaFile(result, modelFacade);
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
                    switch (await attachmentMigrator.MigrateAttachment(mg, $"__{fieldName}", cmsSite.SiteID))
                    {
                        case MigrateAttachmentResultMediaFile(true, _, var x, _):
                        {
                            mfis = [new AssetRelatedItem { Identifier = x.FileGUID, Dimensions = new AssetDimensions { Height = x.FileImageHeight, Width = x.FileImageWidth }, Name = x.FileName, Size = x.FileSize }];
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
                    var sourceMediaFile = modelFacade.SelectWhere<IMediaFile>("FileGUID = @mediaFileGuid AND FileSiteID = @fileSiteID", new SqlParameter("mediaFileGuid", mg), new SqlParameter("fileSiteID", cmsSite.SiteID))
                        .FirstOrDefault();
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
        else if (classService.GetFormControlDefinition(sourceFormControl) is { } formControl)
        {
            switch (formControl)
            {
                case { UserControlForFile: true }:
                {
                    if (sourceValue is Guid attachmentGuid)
                    {
                        switch (await attachmentMigrator.MigrateAttachment(attachmentGuid, $"__{fieldName}", cmsSite.SiteID))
                        {
                            case MigrateAttachmentResultMediaFile(true, _, var mfi, _):
                            {
                                mfis = [new AssetRelatedItem { Identifier = mfi.FileGUID, Dimensions = new AssetDimensions { Height = mfi.FileImageHeight, Width = mfi.FileImageWidth }, Name = mfi.FileName, Size = mfi.FileSize }];
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
                        switch (await attachmentMigrator.MigrateAttachment(attachmentGuid, $"__{fieldName}", cmsSite.SiteID))
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
                        await foreach (var migResult in attachmentMigrator.MigrateGroupedAttachments(docId, field.Guid, field.Name))
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
                                    mfis =
                                    [
                                        new ContentItemReference { Identifier = contentItemGuid }
                                    ];
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
        settings.EnsureElement(FormDefinitionPatcher.SettingsElemControlname, e => e.Value = configuration.MigrateMediaToMediaLibrary ? FormComponents.AdminAssetSelectorComponent : FormComponents.AdminContentItemSelectorComponent);
        if (configuration.MigrateMediaToMediaLibrary)
        {
            settings.EnsureElement(FormDefinitionPatcher.SettingsMaximumassets, maxAssets => maxAssets.Value = FormDefinitionPatcher.SettingsMaximumassetsFallback);
        }
    }
}
