using CMS.DataEngine;
using CMS.FormEngine;
using Microsoft.Extensions.Logging;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Helpers;

public static class FormDefinitionHelper
{
    public static void MapFormDefinitionFields(ILogger logger, IFieldMigrationService fieldMigrationService, ICmsClass source, DataClassInfo target, bool isCustomizableSystemClass, bool classIsCustom)
    {
        if (!string.IsNullOrWhiteSpace(source.ClassFormDefinition))
        {
            var patcher = new FormDefinitionPatcher(
                logger,
                source.ClassFormDefinition,
                fieldMigrationService,
                source.ClassIsForm.GetValueOrDefault(false),
                source.ClassIsDocumentType,
                isCustomizableSystemClass,
                classIsCustom
            );

            patcher.PatchFields();
            patcher.RemoveCategories(); // TODO tk: 2022-10-11 remove when supported

            string? result = patcher.GetPatched();
            if (isCustomizableSystemClass)
            {
                result = FormHelper.MergeFormDefinitions(target.ClassFormDefinition, result);
            }

            var formInfo = new FormInfo(result);
            target.ClassFormDefinition = formInfo.GetXmlDefinition();
        }
        else
        {
            target.ClassFormDefinition = new FormInfo().GetXmlDefinition();
        }
    }

    public static void MapFormDefinitionFields(ILogger logger, IFieldMigrationService fieldMigrationService,
        string sourceClassDefinition, bool? classIsForm, bool classIsDocumentType,
        DataClassInfo target, bool isCustomizableSystemClass, bool classIsCustom, IEnumerable<string> excludedFields)
    {
        if (!string.IsNullOrWhiteSpace(sourceClassDefinition))
        {
            var patcher = new FormDefinitionPatcher(
                logger,
                sourceClassDefinition,
                fieldMigrationService,
                classIsForm.GetValueOrDefault(false),
                classIsDocumentType,
                isCustomizableSystemClass,
                classIsCustom
            );

            patcher.PatchFields(excludedFields);
            patcher.RemoveCategories(); // TODO tk: 2022-10-11 remove when supported

            string? result = patcher.GetPatched();
            if (isCustomizableSystemClass)
            {
                result = FormHelper.MergeFormDefinitions(target.ClassFormDefinition, result);
            }

            var formInfo = new FormInfo(result);
            target.ClassFormDefinition = formInfo.GetXmlDefinition();
        }
        else
        {
            target.ClassFormDefinition = new FormInfo().GetXmlDefinition();
        }
    }
}
