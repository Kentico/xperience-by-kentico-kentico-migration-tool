using CMS.DataEngine;
using CMS.FormEngine;

using MediatR;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Helpers;
using Migration.Tool.KXP.Api;
using Migration.Tool.KXP.Api.Auxiliary;
using Migration.Tool.Source.Services;

namespace Migration.Tool.Extensions.MetadataSchema;

/// <summary>
/// MediatR pipeline behavior that runs around <c>MigratePageTypesCommand</c>.
/// <para>
/// <b>Post-handler</b>: creates the <c>Migration.SEOMetadata</c> reusable
/// field schema and attaches it to every XbyK content type whose source class
/// had <c>ClassHasMetadata = 1</c>.
/// </para>
/// </summary>
public class PageTypesSeoSchemaBehavior(
    ILogger<PageTypesSeoSchemaBehavior> logger,
    EffectiveMetadataService effectiveMetadataService,
    ReusableSchemaService reusableSchemaService,
    KxpClassFacade kxpClassFacade)
    : IPipelineBehavior<MigratePageTypesCommand, CommandResult>
{
    public async Task<CommandResult> Handle(
        MigratePageTypesCommand request,
        RequestHandlerDelegate<CommandResult> next,
        CancellationToken cancellationToken)
    {
        var result = await next();

        EnsureSeoMetadataSchema();
        AttachSchemaToApplicableContentTypes();

        return result;
    }

    private void EnsureSeoMetadataSchema()
    {
        logger.LogInformation(
            "Ensuring reusable field schema '{SchemaName}' exists ...",
            SeoMetadataConstants.SchemaName);

        reusableSchemaService.EnsureReusableFieldSchema(
            SeoMetadataConstants.SchemaName,
            SeoMetadataConstants.SchemaDisplayName,
            SeoMetadataConstants.SchemaDescription,
            BuildTextField(SeoMetadataConstants.FieldMetaTitle, "Meta Title",
                "Effective SEO page title (resolved through page-tree inheritance)."),
            BuildTextField(SeoMetadataConstants.FieldMetaDescription, "Meta Description",
                "Effective SEO page description (resolved through page-tree inheritance)."),
            BuildTextField(SeoMetadataConstants.FieldMetaKeywords, "Meta Keywords",
                "Effective SEO page keywords (resolved through page-tree inheritance)."),
            BuildBoolField(SeoMetadataConstants.FieldMetaTitleInherited, "Meta Title Inherited",
                "True when the meta title was not set on this page and was inherited from an ancestor."),
            BuildBoolField(SeoMetadataConstants.FieldMetaDescriptionInherited, "Meta Description Inherited",
                "True when the meta description was not set on this page and was inherited from an ancestor."),
            BuildBoolField(SeoMetadataConstants.FieldMetaKeywordsInherited, "Meta Keywords Inherited",
                "True when the meta keywords were not set on this page and were inherited from an ancestor.")
        );

        logger.LogInformation(
            "Reusable field schema '{SchemaName}' is ready.",
            SeoMetadataConstants.SchemaName);
    }

    private static FormFieldInfo BuildTextField(string fieldName, string caption, string description)
    {
        var ffi = new FormFieldInfo
        {
            Name = fieldName,
            Guid = GuidHelper.CreateFieldGuid($"{SeoMetadataConstants.SchemaName}|{fieldName}"),
            DataType = FieldDataType.LongText,
            AllowEmpty = true,
            Visible = true,
            Enabled = true,
            Settings =
            {
                ["controlname"] = FormComponents.AdminTextAreaComponent
            }
        };
        ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, caption);
        ffi.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, description);
        return ffi;
    }

    private static FormFieldInfo BuildBoolField(string fieldName, string caption, string description)
    {
        var ffi = new FormFieldInfo
        {
            Name = fieldName,
            Guid = GuidHelper.CreateFieldGuid($"{SeoMetadataConstants.SchemaName}|{fieldName}"),
            DataType = FieldDataType.Boolean,
            AllowEmpty = false,
            DefaultValue = "false",
            Visible = true,
            Enabled = true,
            Settings =
            {
                ["controlname"] = FormComponents.AdminCheckBoxComponent
            }
        };
        ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, caption);
        ffi.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, description);
        return ffi;
    }


    private void AttachSchemaToApplicableContentTypes()
    {
        var sourceClassNames = effectiveMetadataService
            .GetDistinctClassNames()
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (sourceClassNames.Count == 0)
        {
            logger.LogInformation(
                "No source classes with ClassHasMetadata = 1 found; skipping schema attachment.");
            return;
        }

        logger.LogInformation(
            "Attaching SEO metadata schema to content types for {Count} source class name(s) ...",
            sourceClassNames.Count);

        int attached = 0;

        foreach (string sourceClassName in sourceClassNames)
        {
            var dataClassInfo = DataClassInfoProvider.GetDataClassInfo(sourceClassName);
            if (dataClassInfo is null)
            {
                logger.LogWarning(
                    "Target DataClassInfo not found for source class '{ClassName}'. " +
                    "Skipping SEO metadata schema attachment for this class. " +
                    "If a custom class mapping renamed this class, add the schema attachment manually.",
                    sourceClassName);
                continue;
            }

            if (dataClassInfo.ClassType is not ClassType.CONTENT_TYPE)
            {
                logger.LogDebug(
                    "Class '{ClassName}' is not a CONTENT_TYPE; skipping schema attachment.",
                    sourceClassName);
                continue;
            }

            // Avoid attaching the schema twice (idempotent re-runs).
            var schemaGuid = GuidHelper.CreateReusableSchemaGuid(SeoMetadataConstants.SchemaName);
            if (reusableSchemaService.HasClassReusableSchema(dataClassInfo, schemaGuid))
            {
                logger.LogDebug(
                    "Schema '{SchemaName}' already attached to '{ClassName}'; skipping.",
                    SeoMetadataConstants.SchemaName, sourceClassName);
                continue;
            }

            reusableSchemaService.AddReusableSchemaToDataClass(dataClassInfo, schemaGuid);
            kxpClassFacade.SetClass(dataClassInfo);

            logger.LogInformation(
                "Attached SEO metadata schema to content type '{ClassName}'.",
                sourceClassName);
            attached++;
        }

        logger.LogInformation(
            "SEO metadata schema attached to {Count} content type(s).",
            attached);
    }
}
