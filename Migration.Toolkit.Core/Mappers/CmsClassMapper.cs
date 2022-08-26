using CMS.DataEngine;
using CMS.FormEngine;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.Core.Services.CmsClass;

namespace Migration.Toolkit.Core.Mappers;

using CMS.DocumentEngine;
using Kentico.Components.Web.Mvc.FormComponents;
using Migration.Toolkit.KX13.Auxiliary;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXP.Api.Auxiliary;

public class CmsClassMapper : EntityMapperBase<KX13.Models.CmsClass, DataClassInfo>
{
    private const string CLASS_FIELD_CONTROL_NAME = "controlname";
    private const string KENTICO_ADMINISTRATION_TEXTAREA = "Kentico.Administration.TextArea";

    private readonly ILogger<CmsClassMapper> _logger;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly ClassService _classService;

    public CmsClassMapper(ILogger<CmsClassMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext, ClassService classService,
        IProtocol protocol) : base(logger, primaryKeyMappingContext, protocol)
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

    private void MapClassFormFields(CmsClass source, DataClassInfo target)
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
                var columnType = field.DataType;

                switch (columnType)
                {
                    case Kx13FieldDataType.ALL:
                    {
                        field.DataType = FieldDataType.ALL;
                        break;
                    }

                    case Kx13FieldDataType.Unknown:
                    {
                        field.DataType = FieldDataType.Unknown;
                        break;
                    }

                    case Kx13FieldDataType.Text:
                    {
                        // TODO tomas.krch: 2022-08-24 size, etc..
                        field.DataType = FieldDataType.Text;
                        break;
                    }

                    case Kx13FieldDataType.LongText:
                    {
                        field.DataType = FieldDataType.LongText;
                        break;
                    }

                    case Kx13FieldDataType.Integer:
                    {
                        field.DataType = FieldDataType.Integer;
                        break;
                    }

                    case Kx13FieldDataType.LongInteger:
                    {
                        field.DataType = FieldDataType.LongInteger;
                        break;
                    }

                    case Kx13FieldDataType.Double:
                    {
                        field.DataType = FieldDataType.Double;
                        break;
                    }

                    case Kx13FieldDataType.DateTime:
                    {
                        field.DataType = FieldDataType.DateTime;
                        break;
                    }

                    case Kx13FieldDataType.Boolean:
                    {
                        field.DataType = FieldDataType.Boolean;
                        break;
                    }

                    case Kx13FieldDataType.DocAttachments:
                    case Kx13FieldDataType.File:
                    {
                        field.Settings.Clear();
                        field.SettingsMacroTable.Clear();
                        field.DataType = FieldDataType.Assets;
                        field.Settings[CLASS_FIELD_CONTROL_NAME] = FormComponents.AdminAssetSelectorComponent;
                        field.Settings["MaximumAssets"] = "99";
                        formInfo.UpdateFormField(columnName, field);
                        break;
                    }

                    case Kx13FieldDataType.Guid:
                    {
                        field.DataType = FieldDataType.Guid;
                        break;
                    }

                    case Kx13FieldDataType.Binary:
                    {
                        field.DataType = FieldDataType.Binary;
                        break;
                    }

                    case Kx13FieldDataType.Xml:
                    {
                        field.DataType = FieldDataType.Xml;
                        break;
                    }

                    case Kx13FieldDataType.Decimal:
                    {
                        field.DataType = FieldDataType.Decimal;
                        break;
                    }

                    case Kx13FieldDataType.TimeSpan:
                    {
                        field.DataType = FieldDataType.TimeSpan;
                        break;
                    }

                    case Kx13FieldDataType.Date:
                    {
                        field.DataType = FieldDataType.Date;
                        break;
                    }

                    case Kx13FieldDataType.DocRelationships:
                    {
                        field.Settings.Clear();
                        field.SettingsMacroTable.Clear();
                        field.Settings[CLASS_FIELD_CONTROL_NAME] = FormComponents.AdminPageSelectorComponent;
                        field.Settings["MaximumPages"] = "99";
                        field.Settings["RootPath"] = "/";
                        field.DataType = FieldDataType.Pages;
                        field.Size = 0;
                        formInfo.UpdateFormField(columnName, field);
                        break;
                    }
                }
            }
        }

        target.ClassXmlSchema = classStructureInfo.GetXmlSchema();
        target.ClassFormDefinition = formInfo.GetXmlDefinition();
    }
    
    private void MapClassDocumentTypeFields(CmsClass source, DataClassInfo target)
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
                var columnType = field.DataType;

                switch (columnType)
                {
                    case Kx13FieldDataType.ALL:
                    {
                        field.DataType = FieldDataType.ALL;
                        break;
                    }

                    case Kx13FieldDataType.Unknown:
                    {
                        field.DataType = FieldDataType.Unknown;
                        break;
                    }

                    case Kx13FieldDataType.Text:
                    {
                        // TODO tomas.krch: 2022-08-24 size, etc..
                        field.DataType = FieldDataType.Text;
                        break;
                    }

                    case Kx13FieldDataType.LongText:
                    {
                        field.DataType = FieldDataType.LongText;
                        break;
                    }

                    case Kx13FieldDataType.Integer:
                    {
                        field.DataType = FieldDataType.Integer;
                        break;
                    }

                    case Kx13FieldDataType.LongInteger:
                    {
                        field.DataType = FieldDataType.LongInteger;
                        break;
                    }

                    case Kx13FieldDataType.Double:
                    {
                        field.DataType = FieldDataType.Double;
                        break;
                    }

                    case Kx13FieldDataType.DateTime:
                    {
                        field.DataType = FieldDataType.DateTime;
                        break;
                    }

                    case Kx13FieldDataType.Boolean:
                    {
                        field.DataType = FieldDataType.Boolean;
                        break;
                    }

                    case Kx13FieldDataType.DocAttachments:
                    case Kx13FieldDataType.File:
                    {
                        field.Settings.Clear();
                        field.SettingsMacroTable.Clear();
                        field.DataType = FieldDataType.Assets;
                        field.Settings[CLASS_FIELD_CONTROL_NAME] = FormComponents.AdminAssetSelectorComponent;
                        field.Settings["MaximumAssets"] = "99";
                        formInfo.UpdateFormField(columnName, field);
                        break;
                    }

                    case Kx13FieldDataType.Guid:
                    {
                        field.DataType = FieldDataType.Guid;
                        break;
                    }

                    case Kx13FieldDataType.Binary:
                    {
                        field.DataType = FieldDataType.Binary;
                        break;
                    }

                    case Kx13FieldDataType.Xml:
                    {
                        field.DataType = FieldDataType.Xml;
                        break;
                    }

                    case Kx13FieldDataType.Decimal:
                    {
                        field.DataType = FieldDataType.Decimal;
                        break;
                    }

                    case Kx13FieldDataType.TimeSpan:
                    {
                        field.DataType = FieldDataType.TimeSpan;
                        break;
                    }

                    case Kx13FieldDataType.Date:
                    {
                        field.DataType = FieldDataType.Date;
                        break;
                    }

                    case Kx13FieldDataType.DocRelationships:
                    {
                        field.Settings.Clear();
                        field.SettingsMacroTable.Clear();
                        field.Settings[CLASS_FIELD_CONTROL_NAME] = FormComponents.AdminPageSelectorComponent;
                        field.Settings["MaximumPages"] = "99";
                        field.Settings["RootPath"] = "/";
                        field.DataType = FieldDataType.Pages;
                        field.Size = 0;
                        formInfo.UpdateFormField(columnName, field);
                        break;
                    }
                }
            }
        }

        target.ClassXmlSchema = classStructureInfo.GetXmlSchema();
        target.ClassFormDefinition = formInfo.GetXmlDefinition();
    }
}