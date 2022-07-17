using CMS.DataEngine;
using CMS.FormEngine;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.Core.Services.CmsClass;

namespace Migration.Toolkit.Core.Mappers;

using Migration.Toolkit.KX13.Models;

public class CmsClassMapper : EntityMapperBase<KX13.Models.CmsClass, DataClassInfo>
{
    private const string CLASS_FIELD_CONTROL_NAME = "controlname";
    private const string KENTICO_ADMINISTRATION_TEXTAREA = "Kentico.Administration.TextArea";
    
    private readonly ILogger<CmsClassMapper> _logger;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly ClassService _classService;

    public CmsClassMapper(ILogger<CmsClassMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext, ClassService classService,
        IMigrationProtocol protocol) : base(logger, primaryKeyMappingContext, protocol)
    {
        _logger = logger;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _classService = classService;
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

        MapClassFields(source, target);

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
                            string.Join("|", formFieldInfos.Select(f => f.ToXML("FF", false))));
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

        target.ClassResourceID = _primaryKeyMappingContext.MapFromSource<KX13.Models.CmsResource>(c => c.ResourceId, source.ClassResourceId).UseKenticoDefault();
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

    private void MapClassFields(CmsClass source, DataClassInfo target)
    {
        var classStructureInfo = new ClassStructureInfo(source.ClassName, source.ClassXmlSchema, source.ClassTableName);
        var formInfo = new FormInfo(source.ClassFormDefinition);
        if (source.ClassIsCoupledClass)
        {
            var columnNames = formInfo.GetColumnNames();
            foreach (var columnName in columnNames)
            {
                var field = formInfo.GetFormField(columnName);
                var controlName = field.Settings[CLASS_FIELD_CONTROL_NAME]?.ToString()?.ToLowerInvariant();

                if (controlName != null)
                {
                    // might be custom control

                    switch (_classService.GetFormControlDefinition(controlName))
                    {
                        case { UserControlForFile: true }:
                        {
                            // attachment - migrate attachment from field and leave link to it
                            field.Settings.Clear();
                            field.SettingsMacroTable.Clear();
                            field.Settings[CLASS_FIELD_CONTROL_NAME] = KENTICO_ADMINISTRATION_TEXTAREA;
                            field.DataType = FieldDataType.LongText;
                            field.Size = 0;
                            formInfo.UpdateFormField(columnName, field);
                            break;
                        }
                        case { UserControlForDocAttachments: true }:
                        {
                            // attachment - migrate attachment from field and leave link to it
                            field.Settings.Clear();
                            field.SettingsMacroTable.Clear();
                            field.Settings[CLASS_FIELD_CONTROL_NAME] = KENTICO_ADMINISTRATION_TEXTAREA;
                            field.DataType = FieldDataType.LongText;
                            field.Size = 0;
                            formInfo.UpdateFormField(columnName, field);
                            break;
                        }
                        case { UserControlForDocRelationships: true }:
                        {
                            // relation to other document
                            field.Settings.Clear();
                            field.SettingsMacroTable.Clear();
                            field.Settings[CLASS_FIELD_CONTROL_NAME] = KENTICO_ADMINISTRATION_TEXTAREA;
                            field.DataType = FieldDataType.LongText;
                            field.Size = 0;
                            formInfo.UpdateFormField(columnName, field);

                            Protocol.Append(HandbookReferences
                                .NotCurrentlySupportedSkip<FormFieldInfo>()
                                .NeedsManualAction()
                                .WithMessage("Class field type is not supported right now")
                                .WithData(new
                                {
                                    field.Name,
                                    source.ClassName,
                                    source.ClassGuid
                                })
                            );

                            break;
                        }
                        default:
                            // leave as is
                            break;
                    }
                }
            }
        }

        target.ClassXmlSchema = classStructureInfo.GetXmlSchema();
        target.ClassFormDefinition = formInfo.GetXmlDefinition();
    }
}