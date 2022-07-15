using System.Diagnostics;
using CMS.DataEngine;
using CMS.FormEngine;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.Core.Services.CmsClass;

namespace Migration.Toolkit.Core.Mappers;

public class CmsClassMapper :
    // IEntityMapper<KX13.Models.CmsClass, KXO.Models.CmsClass>, 
    EntityMapperBase<KX13.Models.CmsClass, DataClassInfo>
{
    private readonly ILogger<CmsClassMapper> _logger;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly ClassService _classService;
    
    public CmsClassMapper(ILogger<CmsClassMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext, ClassService classService, IMigrationProtocol protocol) : base(logger, primaryKeyMappingContext, protocol)
    {
        _logger = logger;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _classService = classService;
    }

    protected override DataClassInfo? CreateNewInstance(KX13.Models.CmsClass source, MappingHelper mappingHelper, AddFailure addFailure) => DataClassInfo.New();

    protected override DataClassInfo MapInternal(KX13.Models.CmsClass source, DataClassInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        // if (source.ClassName != target.ClassName)
        // {
        //     // assertion failed
        //     _logger.LogTrace("Assertion failed, entity Guid mismatch.");
        //     return new ModelMappingFailedKeyMismatch<DataClassInfo>().Log(_logger);
        // }
        
        // target.ClassId = source.ClassId;
        // TODO tk: 2022-05-17: check assigned sites target.Sites
        
        target.ClassDisplayName = source.ClassDisplayName;
        target.ClassName = source.ClassName;
        target.ClassUsesVersioning = source.ClassUsesVersioning;
        target.ClassIsDocumentType = source.ClassIsDocumentType;
        target.ClassIsCoupledClass = source.ClassIsCoupledClass;
        
        // target.ClassXmlSchema = _formInfoDefinitionConvertor.ConvertToKxo(source.ClassXmlSchema);
        // target.ClassFormDefinition = _formInfoDefinitionConvertor.ConvertToKxo(source.ClassFormDefinition);
        var classStructureInfo = new ClassStructureInfo(source.ClassName, source.ClassXmlSchema, source.ClassTableName);
        var formInfo = new FormInfo(source.ClassFormDefinition);
        if (source.ClassIsCoupledClass)
        {
           
            var columnNames = formInfo.GetColumnNames();
            foreach (var columnName in columnNames)
            {
                var field = formInfo.GetFormField(columnName);
                var controlName = field.Settings["controlname"]?.ToString()?.ToLowerInvariant();
                
                if (controlName != null)
                {
                    // might be custom control
                    // Debug.Assert(ClassHelper.KnownControlNames.Contains(controlName), "ClassHelper.KnownControlNames.Contains(controlName)");
                    
                    switch (_classService.GetFormControlDefinition(controlName))
                    {
                        case { UserControlForFile: true } control: 
                        {
                            // attachment - migrate attachment from field and leave link to it
                            field.Settings.Clear();
                            field.SettingsMacroTable.Clear();
                            field.Settings["controlname"] = "Kentico.Administration.TextArea";
                            field.DataType = FieldDataType.LongText;
                            field.Size = 0;
                            formInfo.UpdateFormField(columnName, field);
                            break;
                        }
                        case { UserControlForDocAttachments: true } control:
                        {
                            // attachment - migrate attachment from field and leave link to it
                            field.Settings.Clear();
                            field.SettingsMacroTable.Clear();
                            field.Settings["controlname"] = "Kentico.Administration.TextArea";
                            field.DataType = FieldDataType.LongText;
                            field.Size = 0;
                            formInfo.UpdateFormField(columnName, field);
                            break;
                        }
                        case { UserControlForDocRelationships: true } control:
                        {
                            // relation to other document
                            field.Settings.Clear();
                            field.SettingsMacroTable.Clear();
                            field.Settings["controlname"] = "Kentico.Administration.TextArea";
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
        
        
        // Debug.Assert(columnNames.Count == classStructureInfo.ColumnsCount, "columnNames.Count == classStructureInfo.ColumnsCount");
        target.ClassXmlSchema = classStructureInfo.GetXmlSchema();
        target.ClassFormDefinition = formInfo.GetXmlDefinition();
        
        
        target.ClassNodeNameSource = source.ClassNodeNameSource;
        target.ClassTableName = source.ClassTableName;
        // TODO tk: 2022-06-07 check if convertible
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
            FormInfo mapInfo = new FormInfo(source.ClassContactMapping);
            var newMappings = new FormInfo();
            if (mapInfo.ItemsList.Count > 0)
            {
                var ffiLookup = mapInfo.ItemsList.OfType<FormFieldInfo>().ToLookup(f => f.MappedToField, f => f);

                foreach (var formFieldInfos in ffiLookup)
                {
                    if (formFieldInfos.Count() > 1 && formFieldInfos.Key != null)
                    {
                        _logger.LogWarning("Multiple mappings with same value in 'MappedToField': {Detail}", string.Join("|", formFieldInfos.Select(f => f.ToXML("FF", false))));
                    }

                    newMappings.AddFormItem(formFieldInfos.First());
                }
            }

            target.ClassContactMapping = newMappings.GetXmlDefinition();
        }
            
        target.ClassContactOverwriteEnabled = source.ClassContactOverwriteEnabled.UseKenticoDefault();
        target.ClassConnectionString = source.ClassConnectionString;
        // target.ClassIsProductSection = source.ClassIsProductSection.UseKenticoDefault();
        // TODO tk: 2022-06-07 check if convertible
        // target.ClassFormLayoutType = source.ClassFormLayoutType.AsEnum<LayoutTypeEnum>();
        // TODO tk: 2022-06-07 check if convertible
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
        
        // target.ClassInheritsFromClassID = _primaryKeyMappingContext.MapFromSource<KX13.Models.CmsClass>(c => c.ClassId, source.ClassInheritsFromClassId).UseKenticoDefault();
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

        // TODO tk: 2022-05-30  Cannot set info object, domain validation failed (Field name: ClassSearchIndexDataSource)
        // target.ClassSearchIndexDataSource = source.ClassSearchIndexDataSource.AsEnum<SearchIndexDataSourceEnum>();
        // target.ClassSearchEnabled = source.ClassSearchEnabled.UseKenticoDefault();
        // target.ClassSearchTitleColumn = source.ClassSearchTitleColumn;
        // target.ClassSearchContentColumn = source.ClassSearchContentColumn;
        // target.ClassSearchImageColumn = source.ClassSearchImageColumn;
        // target.ClassSearchCreationDateColumn = source.ClassSearchCreationDateColumn;
        // target.ClassSearchSettings = source.ClassSearchSettings;

        return target;
    }
}