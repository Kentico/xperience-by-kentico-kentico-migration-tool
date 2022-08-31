using CMS.DataEngine;
using CMS.FormEngine;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.Core.Services.CmsClass;
using FcText = Migration.Toolkit.KX13.Auxiliary.Kx13FormControls.UserControlForText;
using FcLongText = Migration.Toolkit.KX13.Auxiliary.Kx13FormControls.UserControlForLongText;

namespace Migration.Toolkit.Core.Mappers;

public class CmsClassMapper : EntityMapperBase<KX13.Models.CmsClass, DataClassInfo>
{
    private const string CLASS_FIELD_CONTROL_NAME = "controlname";

    private readonly ILogger<CmsClassMapper> _logger;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;

    public CmsClassMapper(ILogger<CmsClassMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext, IProtocol protocol) : base(logger, primaryKeyMappingContext, protocol)
    {
        _logger = logger;
        _primaryKeyMappingContext = primaryKeyMappingContext;
    }

    protected override DataClassInfo? CreateNewInstance(KX13.Models.CmsClass source, MappingHelper mappingHelper, AddFailure addFailure) =>
        DataClassInfo.New();

    protected override DataClassInfo MapInternal(KX13.Models.CmsClass source, DataClassInfo target, bool newInstance, MappingHelper mappingHelper,
        AddFailure addFailure)
    {
        target.ClassDisplayName = source.ClassDisplayName;
        target.ClassName = source.ClassName;
        target.ClassUsesVersioning = source.ClassUsesVersioning;
        target.ClassIsDocumentType = source.ClassIsDocumentType;
        target.ClassIsCoupledClass = source.ClassIsCoupledClass;

        if (source.ClassIsDocumentType)
        {
            MapClassDocumentTypeFields(source, target);
        }

        if (source.ClassIsForm ?? false)
        {
            MapClassFormFields(source, target);
        }

        target.ClassNodeNameSource = source.ClassNodeNameSource;
        target.ClassTableName = source.ClassTableName;
        // target.ClassFormLayout = source.ClassFormLayout;
        target.ClassShowAsSystemTable = source.ClassShowAsSystemTable.UseKenticoDefault();
        target.ClassUsePublishFromTo = source.ClassUsePublishFromTo.UseKenticoDefault();
        target.ClassShowTemplateSelection = source.ClassShowTemplateSelection.UseKenticoDefault();
        // target.ClassIsMenuItemType = source.ClassIsMenuItemType.UseKenticoDefault();
        target.ClassNodeAliasSource = source.ClassNodeAliasSource;
        target.ClassLastModified = source.ClassLastModified;
        target.ClassGUID = source.ClassGuid;
        // target.ClassIsProduct = source.ClassIsProduct.UseKenticoDefault();
        target.ClassIsCustomTable = source.ClassIsCustomTable;
        target.ClassShowColumns = source.ClassShowColumns;

        // target.ClassContactMapping = source.ClassContactMapping;
        if (source.ClassContactMapping != null)
        {
            var mapInfo = new FormInfo(source.ClassContactMapping);
            var newMappings = new FormInfo();
            if (mapInfo.ItemsList.Count > 0)
            {
                var ffiLookup = mapInfo.ItemsList.OfType<FormFieldInfo>().ToLookup(f => f.MappedToField, f => f);

                foreach (var formFieldInfos in ffiLookup)
                {
                    if (formFieldInfos.Count() > 1 && formFieldInfos.Key != null)
                    {
                        _logger.LogWarning("Multiple mappings with same value in 'MappedToField': {Detail}",
                            string.Join("|", formFieldInfos.Select(f => f.ToXML("FormFields", false))));
                    }

                    newMappings.AddFormItem(formFieldInfos.First());
                }
            }

            target.ClassContactMapping = newMappings.GetXmlDefinition();
        }

        target.ClassContactOverwriteEnabled = source.ClassContactOverwriteEnabled.UseKenticoDefault();
        target.ClassConnectionString = source.ClassConnectionString;
        // target.ClassIsProductSection = source.ClassIsProductSection.UseKenticoDefault();
        // target.ClassFormLayoutType = source.ClassFormLayoutType.AsEnum<LayoutTypeEnum>();
        // target.ClassVersionGUID = source.ClassVersionGuid;
        target.ClassDefaultObjectType = source.ClassDefaultObjectType;
        target.ClassIsForm = source.ClassIsForm.UseKenticoDefault();
        target.ClassCustomizedColumns = source.ClassCustomizedColumns;
        target.ClassCodeGenerationSettings = source.ClassCodeGenerationSettings;
        target.ClassIconClass = source.ClassIconClass;
        target.ClassURLPattern = source.ClassUrlpattern;
        target.ClassUsesPageBuilder = source.ClassUsesPageBuilder;
        // target.ClassIsNavigationItem = source.ClassIsNavigationItem;
        target.ClassHasURL = source.ClassHasUrl;
        target.ClassHasMetadata = source.ClassHasMetadata;

        // target.ClassSkumappings = source.ClassSkumappings;
        // target.ClassCreateSku = source.ClassCreateSku;
        // target.ClassSkudefaultDepartmentName = source.ClassSkudefaultDepartmentName;
        // target.ClassSKUDefaultDepartmentID = source.ClassSkudefaultDepartmentId;
        // target.ClassSKUDefaultProductType = source.ClassSkudefaultProductType;

        if (mappingHelper.TranslateId<KX13.Models.CmsClass>(c => c.ClassId, source.ClassInheritsFromClassId.NullIfZero(), out var classId))
        {
            target.ClassInheritsFromClassID = classId.UseKenticoDefault();
        }

        target.ClassResourceID = _primaryKeyMappingContext.MapFromSource<KX13.Models.CmsResource>(c => c.ResourceId, source.ClassResourceId)
            .UseKenticoDefault();
        if (mappingHelper.TryTranslateId<KX13.Models.CmsResource>(c => c.ResourceId, source.ClassResourceId, out var resourceId))
        {
            if (resourceId.HasValue)
            {
                target.ClassResourceID = resourceId.Value;
            }
        }

        // TODO tk: 2022-05-30 domain validation failed (Field name: ClassSearchIndexDataSource)
        // target.ClassSearchIndexDataSource = source.ClassSearchIndexDataSource.AsEnum<SearchIndexDataSourceEnum>();
        // target.ClassSearchEnabled = source.ClassSearchEnabled.UseKenticoDefault();
        // target.ClassSearchTitleColumn = source.ClassSearchTitleColumn;
        // target.ClassSearchContentColumn = source.ClassSearchContentColumn;
        // target.ClassSearchImageColumn = source.ClassSearchImageColumn;
        // target.ClassSearchCreationDateColumn = source.ClassSearchCreationDateColumn;
        // target.ClassSearchSettings = source.ClassSearchSettings;

        return target;
    }

    private void MapClassFormFields(KX13M.CmsClass source, DataClassInfo target)
    {
        var classStructureInfo = new ClassStructureInfo(source.ClassName, source.ClassXmlSchema, source.ClassTableName);
        var formInfo = new FormInfo(source.ClassFormDefinition);
        if (source.ClassIsCoupledClass)
        {
            var columnNames = formInfo.GetColumnNames();
            foreach (var columnName in columnNames)
            {
                var field = formInfo.GetFormField(columnName);
                ConvertSingleField(field, formInfo, columnName, FieldMappingInstance.Default);
            }
        }

        target.ClassXmlSchema = classStructureInfo.GetXmlSchema();
        target.ClassFormDefinition = formInfo.GetXmlDefinition();
    }

    private void MapClassDocumentTypeFields(KX13M.CmsClass source, DataClassInfo target)
    {
        var classStructureInfo = new ClassStructureInfo(source.ClassName, source.ClassXmlSchema, source.ClassTableName);
        var formInfo = new FormInfo(source.ClassFormDefinition);
        if (source.ClassIsCoupledClass)
        {
            var columnNames = formInfo.GetColumnNames();
            foreach (var columnName in columnNames)
            {
                var field = formInfo.GetFormField(columnName);
                // var controlName = field.Settings[CLASS_FIELD_CONTROL_NAME]?.ToString()?.ToLowerInvariant();
                ConvertSingleField(field, formInfo, columnName, FieldMappingInstance.Default);
            }
        }

        target.ClassXmlSchema = classStructureInfo.GetXmlSchema();
        target.ClassFormDefinition = formInfo.GetXmlDefinition();
    }

    private static void ConvertSingleField(FormFieldInfo field, FormInfo formInfo, string columnName, Dictionary<string, DataTypeModel> dataTypeModels)
    {
        var columnType = field.DataType;
        var controlName = field.Settings[CLASS_FIELD_CONTROL_NAME]?.ToString()?.ToLowerInvariant();

        static void PerformActionsOnField(FormFieldInfo field, string[] actions)
        {
            foreach (var action in actions)
            {
                switch (action)
                {
                    case TcaDirective.ClearSettings:
                    {
                        field.Settings.Clear();
                        break;
                    }
                    case TcaDirective.ClearMacroTable:
                    {
                        field.SettingsMacroTable.Clear();
                        break;
                    }
                    case TcaDirective.ConvertToAsset:
                    {
                        // field.DataType = FieldDataType.Assets;
                        field.Settings["MaximumAssets"] = "99"; // setting is missing in source instance, target instance requires it
                        break;
                    }
                    case TcaDirective.ConvertToPages:
                    {
                        field.Settings["MaximumPages"] = "99"; // setting is missing in source instance, target instance requires it
                        field.Settings["RootPath"] = "/"; // TODO tk: 2022-08-31 describe why?
                        field.Size = 0; // TODO tk: 2022-08-31 describe why?
                        // field.DataType = FieldDataType.Pages;
                        break;
                    }
                }
            }
        }

        if (dataTypeModels.TryGetValue(columnType, out var typeMapping))
        {
            field.DataType = typeMapping.TargetDataType;
            if (controlName != null &&
                typeMapping is { FormComponents: { } } &&
                (typeMapping.FormComponents.TryGetValue(controlName, out var targetControlMapping) ||
                 typeMapping.FormComponents.TryGetValue("#any-not-matched-by-control-mapping-list#", out targetControlMapping)))
            {
                var (targetFormComponent, actions) = targetControlMapping;
                if (targetControlMapping.TargetFormComponent == "#copy-source-control#")
                {
                    field.Settings[CLASS_FIELD_CONTROL_NAME] = controlName;
                    PerformActionsOnField(field, actions);
                }
                else if (targetFormComponent == "#clear-field-control#")
                {
                    field.Settings.Remove(CLASS_FIELD_CONTROL_NAME);
                }
                else
                {
                    field.Settings[CLASS_FIELD_CONTROL_NAME] = targetFormComponent;
                    PerformActionsOnField(field, actions);
                }

                formInfo.UpdateFormField(columnName, field);
            }
        }
    }
}