using CMS.ContentEngine.Internal;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;

using MediatR;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Source.Services;

namespace Migration.Tool.Extensions.MetadataSchema;

/// <summary>
/// MediatR pipeline behavior that runs around <c>MigratePagesCommand</c>.
/// <para>
/// After the standard pages handler completes, this behavior iterates every
/// row in the <c>Migration_EffectiveMetadata</c> auxiliary table and writes
/// the effective SEO metadata values into the <c>MetaTitle</c>,
/// <c>MetaDescription</c>, and <c>MetaKeywords</c> fields of the corresponding
/// migrated content item's data record in XbyK.
/// </para>
/// </summary>
public class PagesEffectiveMetadataBehavior(
    ILogger<PagesEffectiveMetadataBehavior> logger,
    EffectiveMetadataService effectiveMetadataService,
    SpoiledGuidContext spoiledGuidContext,
    ReusableSchemaService reusableSchemaService)
    : IPipelineBehavior<MigratePagesCommand, CommandResult>
{
    public async Task<CommandResult> Handle(
        MigratePagesCommand request,
        RequestHandlerDelegate<CommandResult> next,
        CancellationToken cancellationToken)
    {
        var result = await next();

        PatchEffectiveMetadata();

        return result;
    }


    private void PatchEffectiveMetadata()
    {
        var effectiveMetadata = effectiveMetadataService.GetAll();
        if (effectiveMetadata.Count == 0)
        {
            logger.LogInformation(
                "No effective-metadata rows found; nothing to patch.");
            return;
        }

        logger.LogInformation(
            "Patching effective SEO metadata for {Count} document(s) ...", effectiveMetadata.Count);

        var itemDataInfoProviderAccessor =
            Service.Resolve<IContentItemDataInfoProviderAccessor>();

        int patched = 0;
        int skipped = 0;

        foreach (var effectiveRow in effectiveMetadata)
        {
            if (effectiveRow.EffectiveTitle is null &&
                effectiveRow.EffectiveKeywords is null &&
                effectiveRow.EffectiveDescription is null)
            {
                skipped++;
                continue;
            }

            try
            {
                PatchRow(effectiveRow, itemDataInfoProviderAccessor, ref patched, ref skipped);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to patch SEO metadata for DocumentID={DocumentID}.",
                    effectiveRow.DocumentID);
                skipped++;
            }
        }

        logger.LogInformation(
            "Effective SEO metadata patch complete. Patched={Patched}, Skipped={Skipped}.",
            patched, skipped);
    }

    private void PatchRow(
        EffectiveMetadataRecord row,
        IContentItemDataInfoProviderAccessor itemDataInfoProviderAccessor,
        ref int patched,
        ref int skipped)
    {
        var commonDataGuid = spoiledGuidContext.EnsureDocumentGuid(
            row.DocumentGUID,
            row.NodeSiteID,
            row.NodeID,
            row.DocumentID);

        var commonDataInfo = ContentItemCommonDataInfo.Provider.Get()
            .WhereEquals(
                nameof(ContentItemCommonDataInfo.ContentItemCommonDataGUID),
                commonDataGuid)
            .FirstOrDefault();

        if (commonDataInfo is null)
        {
            logger.LogDebug(
                "ContentItemCommonData not found for DocumentID={DocumentID} " +
                "(GUID={CommonDataGuid}); skipping.",
                row.DocumentID, commonDataGuid);
            skipped++;
            return;
        }

        var contentItem = ContentItemInfo.Provider.Get(
            commonDataInfo.ContentItemCommonDataContentItemID);

        if (contentItem is null)
        {
            logger.LogWarning(
                "ContentItem not found for ContentItemID={ContentItemID}; skipping.",
                commonDataInfo.ContentItemCommonDataContentItemID);
            skipped++;
            return;
        }

        var dataClassInfo = DataClassInfoProvider.GetDataClassInfo(
            contentItem.ContentItemContentTypeID);

        if (dataClassInfo is null)
        {
            logger.LogWarning(
                "DataClassInfo not found for ClassID={ClassID}; skipping.",
                contentItem.ContentItemContentTypeID);
            skipped++;
            return;
        }

        var schemaGuid = GuidHelper.CreateReusableSchemaGuid(SeoMetadataConstants.SchemaName);
        bool schemaAttached = reusableSchemaService.HasClassReusableSchema(dataClassInfo, schemaGuid);

        if (!schemaAttached)
        {
            logger.LogDebug(
                "SEO metadata schema is not attached to content type '{ClassName}'; " +
                "skipping patch for DocumentID={DocumentID}.",
                dataClassInfo.ClassName, row.DocumentID);
            skipped++;
            return;
        }

        commonDataInfo.SetValue(SeoMetadataConstants.FieldMetaTitle, row.EffectiveTitle ?? string.Empty);
        commonDataInfo.SetValue(SeoMetadataConstants.FieldMetaDescription, row.EffectiveDescription ?? string.Empty);
        commonDataInfo.SetValue(SeoMetadataConstants.FieldMetaKeywords, row.EffectiveKeywords ?? string.Empty);
        commonDataInfo.SetValue(SeoMetadataConstants.FieldMetaTitleInherited, row.TitleInherited);
        commonDataInfo.SetValue(SeoMetadataConstants.FieldMetaDescriptionInherited, row.DescriptionInherited);
        commonDataInfo.SetValue(SeoMetadataConstants.FieldMetaKeywordsInherited, row.KeywordsInherited);

        commonDataInfo.Update();

        logger.LogTrace(
            "Patched DocumentID={DocumentID}: Title='{Title}', Desc='{Desc}', Keywords='{KW}'.",
            row.DocumentID, row.EffectiveTitle, row.EffectiveDescription, row.EffectiveKeywords);

        patched++;
    }
}
