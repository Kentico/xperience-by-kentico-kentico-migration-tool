namespace Migration.Toolkit.Core.K11.Mappers;

using CMS.DataEngine;
using CMS.FormEngine;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Enumerations;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.Core.K11.Services;
using Migration.Toolkit.Core.K11.Services.CmsClass;
using Migration.Toolkit.K11.Models;

public record AlternativeFormMapperSource(CmsAlternativeForm AlternativeForm, DataClassInfo XbkFormClass);

public class AlternativeFormMapper(ILogger<AlternativeFormMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol, FieldMigrationService fieldMigrationService)
    : EntityMapperBase<AlternativeFormMapperSource, AlternativeFormInfo>(logger, pkContext, protocol)
{
    protected override AlternativeFormInfo? CreateNewInstance(AlternativeFormMapperSource source, MappingHelper mappingHelper, AddFailure addFailure)
        => AlternativeFormInfo.New();

    protected override AlternativeFormInfo MapInternal(AlternativeFormMapperSource sourceObj, AlternativeFormInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var (source, xbkFormClass) = sourceObj;

        target.FormClassID = mappingHelper.TranslateIdAllowNulls<CmsClass>(c => c.ClassId, source.FormClassId, out var classId)
            ? classId ?? 0
            : 0;
        target.FormCoupledClassID = mappingHelper.TranslateIdAllowNulls<CmsClass>(c => c.ClassId, source.FormCoupledClassId, out var coupledClassId)
            ? coupledClassId ?? 0
            : 0;

        var coupledClassIsDeprecated =
            source.FormCoupledClass?.ClassName is { } coupledClassName &&
            K12SystemClass.NoLongerSupported.Contains(coupledClassName);

        var classIsSysInternal = K12SystemClass.All.Contains(source.FormClass.ClassName);

        var mergedDefinition = source.FormClass.ClassFormDefinition;
        if (source.FormCoupledClass != null)
        {
            logger.LogDebug("Merging coupled class ('{FormCoupledClassName}') form definition with form definition ('{FormClassName}')", source.FormCoupledClass.ClassName, source.FormClass.ClassName);
            mergedDefinition = FormHelper.MergeFormDefinitions(mergedDefinition, source.FormCoupledClass.ClassFormDefinition);
        }
        mergedDefinition = FormHelper.MergeFormDefinitions(mergedDefinition, source.FormDefinition);

        var patcher = new FormDefinitionPatcher(
            logger,
            mergedDefinition,
            fieldMigrationService,
            source.FormClass.ClassIsForm.GetValueOrDefault(false),
            source.FormClass.ClassIsDocumentType,
            false,
            !classIsSysInternal,
            true
        );

        var fieldNames = patcher.GetFieldNames().ToList();
        logger.LogDebug("Fields ({Count}) before patch: {Fields}", fieldNames.Count, string.Join(",", fieldNames));

        patcher.PatchFields();

        var fieldNamesAfterPatch = patcher.GetFieldNames().ToList();
        logger.LogDebug("Fields ({Count}) after patch: {Fields}", fieldNamesAfterPatch.Count, string.Join(",", fieldNamesAfterPatch));

        if (coupledClassIsDeprecated && source.FormCoupledClass != null)
        {
            logger.LogDebug("Form coupled class ('{FormCoupledClassName}') is deprecated, removing fields", source.FormCoupledClass.ClassName);
            patcher.RemoveFields(source.FormCoupledClass.ClassFormDefinition);

            var fileNamesAfterDeprecatedRemoval = patcher.GetFieldNames().ToList();
            logger.LogDebug("Fields ({Count}) after deprecated removal: {Fields}", fileNamesAfterDeprecatedRemoval.Count, string.Join(",", fileNamesAfterDeprecatedRemoval));
        }

        var result = new FormInfo(patcher.GetPatched()).GetXmlDefinition();

        var formDefinitionDifference = FormHelper.GetFormDefinitionDifference(xbkFormClass.ClassFormDefinition, result, true);

        target.FormDefinition = formDefinitionDifference;

        target.FormDisplayName = source.FormDisplayName;
        target.FormGUID = source.FormGuid;
        target.FormIsCustom = source.FormIsCustom.GetValueOrDefault(false);
        target.FormLastModified = source.FormLastModified;
        target.FormName = source.FormName;

        return target;
    }
}