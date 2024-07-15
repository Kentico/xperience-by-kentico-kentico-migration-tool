namespace Migration.Toolkit.Source.Mappers;

using CMS.DataEngine;
using CMS.FormEngine;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Enumerations;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KXP.Api.Services.CmsClass;
using Migration.Toolkit.Source.Contexts;
using Migration.Toolkit.Source.Model;
using Migration.Toolkit.Source.Services;

public record AlternativeFormMapperSource(ICmsAlternativeForm AlternativeForm, DataClassInfo XbkFormClass);

public class AlternativeFormMapper(
    ILogger<AlternativeFormMapper> logger,
    PrimaryKeyMappingContext pkContext,
    IProtocol protocol,
    FieldMigrationService fieldMigrationService,
    ModelFacade modelFacade
    )
    : EntityMapperBase<AlternativeFormMapperSource, AlternativeFormInfo>(logger, pkContext, protocol)
{
    protected override AlternativeFormInfo? CreateNewInstance(AlternativeFormMapperSource source, MappingHelper mappingHelper, AddFailure addFailure)
        => AlternativeFormInfo.New();

    protected override AlternativeFormInfo MapInternal(AlternativeFormMapperSource sourceObj, AlternativeFormInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var (source, xbkFormClass) = sourceObj;

        target.FormClassID = mappingHelper.TranslateIdAllowNulls<ICmsClass>(c => c.ClassID, source.FormClassID, out var classId)
            ? classId ?? 0
            : 0;
        target.FormCoupledClassID = mappingHelper.TranslateIdAllowNulls<ICmsClass>(c => c.ClassID, source.FormCoupledClassID, out var coupledClassId)
            ? coupledClassId ?? 0
            : 0;

        var formCoupledClass = modelFacade.SelectById<ICmsClass>(source.FormCoupledClassID);
        var formClass = modelFacade.SelectById<ICmsClass>(source.FormClassID);

        var coupledClassIsDeprecated =
            formCoupledClass?.ClassName is { } coupledClassName &&
            K12SystemClass.NoLongerSupported.Contains(coupledClassName);

        var classIsSysInternal = K12SystemClass.All.Contains(formClass.ClassName);

        var mergedDefinition = formClass.ClassFormDefinition;
        if (formCoupledClass != null)
        {
            logger.LogDebug("Merging coupled class ('{FormCoupledClassName}') form definition with form definition ('{FormClassName}')", formCoupledClass.ClassName, formClass.ClassName);
            mergedDefinition = FormHelper.MergeFormDefinitions(mergedDefinition, formCoupledClass.ClassFormDefinition);
        }
        mergedDefinition = FormHelper.MergeFormDefinitions(mergedDefinition, source.FormDefinition);

        var patcher = new FormDefinitionPatcher(
            logger,
            mergedDefinition,
            fieldMigrationService,
            formClass.ClassIsForm.GetValueOrDefault(false),
            formClass.ClassIsDocumentType,
            false,
            !classIsSysInternal,
            true
        );

        var fieldNames = patcher.GetFieldNames().ToList();
        logger.LogDebug("Fields ({Count}) before patch: {Fields}", fieldNames.Count, string.Join(",", fieldNames));

        patcher.PatchFields();

        var fieldNamesAfterPatch = patcher.GetFieldNames().ToList();
        logger.LogDebug("Fields ({Count}) after patch: {Fields}", fieldNamesAfterPatch.Count, string.Join(",", fieldNamesAfterPatch));

        if (coupledClassIsDeprecated && formCoupledClass != null)
        {
            logger.LogDebug("Form coupled class ('{FormCoupledClassName}') is deprecated, removing fields", formCoupledClass.ClassName);
            patcher.RemoveFields(formCoupledClass.ClassFormDefinition);

            var fileNamesAfterDeprecatedRemoval = patcher.GetFieldNames().ToList();
            logger.LogDebug("Fields ({Count}) after deprecated removal: {Fields}", fileNamesAfterDeprecatedRemoval.Count, string.Join(",", fileNamesAfterDeprecatedRemoval));
        }

        var result = new FormInfo(patcher.GetPatched()).GetXmlDefinition();

        var formDefinitionDifference = FormHelper.GetFormDefinitionDifference(xbkFormClass.ClassFormDefinition, result, true);

        target.FormDefinition = formDefinitionDifference;

        target.FormDisplayName = source.FormDisplayName;
        target.FormGUID = source.FormGUID;
        target.FormIsCustom = source.FormIsCustom.GetValueOrDefault(false);
        target.FormLastModified = source.FormLastModified;
        target.FormName = source.FormName;

        return target;
    }
}