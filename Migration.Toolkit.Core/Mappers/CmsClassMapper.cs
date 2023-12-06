using CMS.DataEngine;
using CMS.FormEngine;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Services.CmsClass;

namespace Migration.Toolkit.Core.Mappers;

using System.Diagnostics;
using System.Xml;
using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine.Query;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Enumerations;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.Services;
using Migration.Toolkit.KX13.Models;

public class CmsClassMapper : EntityMapperBase<KX13.Models.CmsClass, DataClassInfo>
{
    private readonly ILogger<CmsClassMapper> _logger;
    private readonly FieldMigrationService _fieldMigrationService;
    private readonly KeyMappingContext _keyMappingContext;

    public CmsClassMapper(
        ILogger<CmsClassMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol,
        FieldMigrationService fieldMigrationService,
        KeyMappingContext keyMappingContext
        ) : base(logger,
        primaryKeyMappingContext, protocol)
    {
        _logger = logger;
        _fieldMigrationService = fieldMigrationService;
        _keyMappingContext = keyMappingContext;
    }

    protected override DataClassInfo? CreateNewInstance(KX13.Models.CmsClass source, MappingHelper mappingHelper, AddFailure addFailure) =>
        DataClassInfo.New();

    protected override DataClassInfo MapInternal(KX13.Models.CmsClass source, DataClassInfo target, bool newInstance, MappingHelper mappingHelper,
        AddFailure addFailure)
    {
        // if (source.ClassName.Equals("cms.folder", StringComparison.InvariantCultureIgnoreCase))
        // {
        //
        // }
        // else
        // {
        //
        // }

        target.ClassDisplayName = source.ClassDisplayName;
        target.ClassName = source.ClassName;
        // TODO tomas.krch: 2023-10-30 removed in v27 => this needs to be replaced by 'ClassType' and 'ClassContentTypeType'
        // target.ClassIsDocumentType = source.ClassIsDocumentType;

        // TODOV27 tomas.krch: 2023-09-05: class is coupled class removed, replacement?
        // target.ClassIsCoupledClass = source.ClassIsCoupledClass;


        var isCustomizableSystemClass = false;
        var classIsCustom = true;
        if (source.ClassResource?.ResourceName is { } resourceName)
        {
            isCustomizableSystemClass = source.ClassShowAsSystemTable.GetValueOrDefault(false) &&
                                        Kx13SystemResource.All.Contains(resourceName);

            classIsCustom = !Kx13SystemResource.All.Contains(resourceName);

            _logger.LogDebug("{ClassName} is {@Properties}", source.ClassName, new { isCustomizableSystemClass, classIsCustom, source.ClassResourceId, source.ClassResource?.ResourceName });
        }

        MapFormDefinitionFields(source, target, isCustomizableSystemClass, classIsCustom);


        target.ClassHasUnmanagedDbSchema = false;


        target.ClassTableName = source.ClassTableName;
        target.ClassShowTemplateSelection = source.ClassShowTemplateSelection.UseKenticoDefault();
        target.ClassLastModified = source.ClassLastModified;
        target.ClassGUID = source.ClassGuid;


        if (source.ClassContactMapping != null)
        {
            var mapInfo = new FormInfo(source.ClassContactMapping);
            var newMappings = new FormInfo();
            if (mapInfo.ItemsList.Count > 0)
            {
                var ffiLookup = mapInfo.ItemsList.OfType<FormFieldInfo>().ToLookup(f => f.MappedToField, f => f);

                foreach (var formFieldInfos in ffiLookup)
                {
                    // if (formFieldInfos.Count() > 1 && formFieldInfos.Key != null)
                    // {
                    //     _logger.LogWarning("Multiple mappings with same value in 'MappedToField': {Detail}",
                    //         string.Join("|", formFieldInfos.Select(f => f.ToXML("FormFields", false))));
                    // }

                    newMappings.AddFormItem(formFieldInfos.First());
                }
            }

            target.ClassContactMapping = newMappings.GetXmlDefinition();
        }

        target.ClassContactOverwriteEnabled = source.ClassContactOverwriteEnabled.UseKenticoDefault();
        target.ClassConnectionString = source.ClassConnectionString;
        target.ClassDefaultObjectType = source.ClassDefaultObjectType;
        target.ClassCodeGenerationSettings = source.ClassCodeGenerationSettings;
        target.ClassIconClass = source.ClassIconClass;
        target.ClassWebPageHasUrl = source.ClassHasUrl;

        // true for page content type
        // TODOV27 tomas.krch: 2023-09-05: obsolete dataclass properties
        // target.ClassIsPage = source.ClassIsDocumentType;
        // target.ClassNodeNameSource = source.ClassNodeNameSource;
        // target.ClassShowAsSystemTable = source.ClassShowAsSystemTable.UseKenticoDefault();
        // target.ClassUsePublishFromTo = source.ClassUsePublishFromTo.UseKenticoDefault();
        // target.ClassNodeAliasSource = source.ClassNodeAliasSource;
        // target.ClassShowColumns = source.ClassShowColumns;
        // target.ClassURLPattern = source.ClassUrlpattern;
        // target.ClassUsesPageBuilder = source.ClassUsesPageBuilder;
        // target.ClassHasMetadata = source.ClassHasMetadata;
        // target.ClassIsForm = source.ClassIsForm.UseKenticoDefault();
        // target.ClassCustomizedColumns = source.ClassCustomizedColumns;

        // TODOV27 tomas.krch: 2023-09-05: broken class inheritance
        // if (mappingHelper.TranslateIdAllowNulls<KX13.Models.CmsClass>(c => c.ClassId, source.ClassInheritsFromClassId.NullIfZero(), out var classId))
        // {
        //     target.ClassInheritsFromClassID = classId.UseKenticoDefault();
        // }

        if (mappingHelper.TranslateIdAllowNulls<KX13.Models.CmsResource>(c => c.ResourceId, source.ClassResourceId, out var resourceId))
        {
            if (resourceId.HasValue)
            {
                target.ClassResourceID = resourceId.Value;
            }
        }

        // OBSOLETE
        // target.ClassFormLayout = source.ClassFormLayout;
        // target.ClassIsMenuItemType = source.ClassIsMenuItemType.UseKenticoDefault();
        // target.ClassIsProduct = source.ClassIsProduct.UseKenticoDefault();
        // target.ClassUsesVersioning = source.ClassUsesVersioning;
        // target.ClassSkumappings = source.ClassSkumappings;
        // target.ClassCreateSku = source.ClassCreateSku;
        // target.ClassSkudefaultDepartmentName = source.ClassSkudefaultDepartmentName;
        // target.ClassSKUDefaultDepartmentID = source.ClassSkudefaultDepartmentId;
        // target.ClassSKUDefaultProductType = source.ClassSkudefaultProductType;
        // target.ClassIsProductSection = source.ClassIsProductSection.UseKenticoDefault();
        // target.ClassFormLayoutType = source.ClassFormLayoutType.AsEnum<LayoutTypeEnum>();
        // target.ClassVersionGUID = source.ClassVersionGuid;
        // target.ClassIsNavigationItem = source.ClassIsNavigationItem;
        // TODO tk: 2022-05-30 domain validation failed (Field name: ClassSearchIndexDataSource)
        // target.ClassSearchIndexDataSource = source.ClassSearchIndexDataSource.AsEnum<SearchIndexDataSourceEnum>();
        // target.ClassSearchEnabled = source.ClassSearchEnabled.UseKenticoDefault();
        // target.ClassSearchTitleColumn = source.ClassSearchTitleColumn;
        // target.ClassSearchContentColumn = source.ClassSearchContentColumn;
        // target.ClassSearchImageColumn = source.ClassSearchImageColumn;
        // target.ClassSearchCreationDateColumn = source.ClassSearchCreationDateColumn;
        // target.ClassSearchSettings = source.ClassSearchSettings;

        switch (source)
        {
            // Special
            case { ClassName: { } className } when
                className.Equals("cms.site", StringComparison.InvariantCultureIgnoreCase) ||
                className.Equals("cms.root", StringComparison.InvariantCultureIgnoreCase)
                :
            {
                // Debug.WriteLine($"'{source.ClassName}' SKIPPED");
                // return;
                // TODO tomas.krch: 2023-11-08 correct fail - this needs to be excluded before mapping
                // addFailure($"'{source.ClassName}' SKIPPED");
                throw new Exception("Unable to map obsolete dataclass");
                return target;
            }
            // Target Other,null
            // Target System,null
            case not null when target is { ClassType: ClassType.OTHER or ClassType.SYSTEM_TABLE }:
            {
                // Debug.WriteLine($"'{dataClass.ClassName}' => OTHER or SYSTEM => IGNORED");
                break;
            }
        }

        // if (target == null)
        // {
        //     // Debug.WriteLine($"'{source.ClassName}' not found in migrated db");
        //     target = MapTempClassToDataClass(source);
        // }

        switch (source)
        {
            // Custom class from custom resource?
            case
            {
                ClassIsForm: false or null,
                ClassIsDocumentType: false,
                ClassResourceId: { } classResourceId
            }:
            {
                target.ClassType = ClassType.OTHER;
                target.ClassContentTypeType = null;

                target = PatchDataClassInfo(target, out var oldPrimaryKeyName, out var documentNameField);

                break;
            }

            // Target Form,null
            case
                {
                    ClassIsDocumentType: false,
                    ClassIsCoupledClass: true,
                    ClassIsForm: true,
                    // ClassIsPage: false
                }
                :
            {
                target.ClassType = ClassType.FORM;
                target.ClassContentTypeType = "";
                // remove when ClassIsDocumentType is completely obsolete
                // if(target.ContainsColumn("ClassIsDocumentType")) target.SetValue("ClassIsDocumentType", false);
                // Debug.WriteLine($"'{target.ClassName}' => CT='{target.ClassType}' CCTT='{target.ClassContentTypeType}'");
                //DataClassInfoProvider.ProviderObject.Set(target);

                break;
            }

            // Target Content,Reusable
            case
            {
                ClassIsDocumentType: false,
                ClassIsForm: false or null,
                ClassShowAsSystemTable: false,
                ClassIsCustomTable: false,
                // ClassIsPage: false
            }:
            {
                target.ClassType = ClassType.CONTENT_TYPE;
                target.ClassContentTypeType = ClassContentTypeType.REUSABLE;

                target = PatchDataClassInfo(target, out var oldPrimaryKeyName, out var documentNameField);

                // Debug.WriteLine($"'{target.ClassName}' => CT='{target.ClassType}' CCTT='{target.ClassContentTypeType}'");
                // DataClassInfoProvider.ProviderObject.Set(target);
                // mMigrated.DataClass(source.ClassName, source.ClassID, new Migrated.SimplifiedClassInfo(oldPrimaryKeyName, target.ClassTableName, documentNameField, source.ClassFormDefinition), target);
                break;
            }

            // Target Content,Website
            case { ClassName: { } className } when className.Equals("cms.folder", StringComparison.InvariantCultureIgnoreCase):
            case
            {
                ClassIsDocumentType: true,
                ClassIsForm: false or null,
                // ClassIsPage: true
            }:
            {
                target.ClassType = ClassType.CONTENT_TYPE;
                target.ClassContentTypeType = ClassContentTypeType.WEBSITE;

                target = PatchDataClassInfo(target, out var oldPrimaryKeyName, out var documentNameField);

                // Debug.WriteLine($"'{target.ClassName}' => CT='{target.ClassType}' CCTT='{target.ClassContentTypeType}'");
                // DataClassInfoProvider.ProviderObject.Set(target);
                // mMigrated.DataClass(source.ClassName, source.ClassID, new Migrated.SimplifiedClassInfo(oldPrimaryKeyName, target.ClassTableName, documentNameField, source.ClassFormDefinition), target);
                break;
            }
        }

        return target;
    }

    private void MapFormDefinitionFields(CmsClass source, DataClassInfo target, bool isCustomizableSystemClass, bool classIsCustom)
    {
        // var classStructureInfo = new ClassStructureInfo(source.ClassName, source.ClassXmlSchema, source.ClassTableName);

        if (!string.IsNullOrWhiteSpace(source.ClassFormDefinition))
        {
            var patcher = new FormDefinitionPatcher(
                _logger,
                source.ClassFormDefinition,
                _fieldMigrationService,
                source.ClassIsForm.GetValueOrDefault(false),
                source.ClassIsDocumentType,
                isCustomizableSystemClass,
                classIsCustom
            );

            patcher.PatchFields();
            patcher.RemoveCategories(); // TODO tk: 2022-10-11 remove when supported

            var result = patcher.GetPatched();
            if (isCustomizableSystemClass)
            {
                result = FormHelper.MergeFormDefinitions(target.ClassFormDefinition, result);
            }

            var formInfo = new FormInfo(result); //(source.ClassFormDefinition);

            // temporary fix until system category is supported

            // var columnNames = formInfo.GetColumnNames();
            //
            // foreach (var columnName in columnNames)
            // {
            //     var field = formInfo.GetFormField(columnName);
            //     ConvertSingleField(field, formInfo, columnName, FieldMappingInstance.Default.DataTypeMappings);
            // }

            // TODO tomas.krch: 2023-10-30 check if XML Schema gets set automatically with API kentico api
            // target.ClassXmlSchema = classStructureInfo.GetXmlSchema();
            target.ClassFormDefinition = formInfo.GetXmlDefinition();
        }
        else
        {
            target.ClassFormDefinition = new FormInfo().GetXmlDefinition();
        }
    }

    private DataClassInfo PatchDataClassInfo(DataClassInfo dataClass, out string? oldPrimaryKeyName, out string? documentNameField)
    {
        oldPrimaryKeyName = null;
        documentNameField = null;
        if (dataClass.ClassType is ClassType.CONTENT_TYPE)
        {
            var fi = new FormInfo(dataClass.ClassFormDefinition);
            var tableName = dataClass.ClassTableName;
            var contentTypeManager = Service.Resolve<IContentTypeManager>();
            contentTypeManager.Initialize(dataClass);
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                dataClass.ClassTableName = tableName;
            }
            var nfi = new FormInfo(dataClass.ClassFormDefinition);
            AppendDocumentNameField(nfi, out documentNameField);

            foreach (var dataDefinitionItem in fi.GetFormElements(true, true) ?? new())
            {
                if (dataDefinitionItem is FormFieldInfo ffi)
                {
                    if (ffi is { PrimaryKey: true })
                    {
                        oldPrimaryKeyName = ffi.Name;
                        continue;
                    }

                    if (ffi.DataType.Equals("contentitems", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ffi.DataType = "contentitemreference";
                        if (!ffi.AllowEmpty)
                        {
                            ffi.ValidationRuleConfigurationsXmlData = AppendRequiredValidationRule(ffi.ValidationRuleConfigurationsXmlData);
                        }
                    }

                    if (ffi.DataType.Equals("pages", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ffi.DataType = "webpages";
                    }

                    if (string.IsNullOrWhiteSpace(ffi.Settings["controlname"] as string))
                    {
                        ffi.Visible = false;
                    }
                }

                nfi.AddFormItem(dataDefinitionItem);
            }

            Debug.WriteLineIf(oldPrimaryKeyName == null, $"WARN: old PK is null for class '{dataClass.ClassName}'");

            dataClass.ClassFormDefinition = nfi.GetXmlDefinition();
            return dataClass;
        }

        return dataClass;
    }

    private static void AppendDocumentNameField(FormInfo nfi, out string documentNameField)
    {
        // no DocumentName in v27, we supply one in migration
        documentNameField = "DocumentName";
        var i = 0;
        while (nfi.GetFormField(documentNameField) is { })
        {
            documentNameField = $"DocumentName{++i}";
        }

        nfi.AddFormItem(new FormFieldInfo
        {
            Caption = "Page name", // as in v26.x.x
            Name = documentNameField,
            AllowEmpty = false,
            DataType = "text",
            Size = 100,
            Precision = 0,
            DefaultValue = null,
            Guid = Guid.NewGuid(),
            System = false, // no longer system field, system doesn't rely on this field anymore
            Settings = { { "controlname", "Kentico.Administration.TextInput" } }
        });
    }

    private const string RequiredRuleIdentifier = "Kentico.Administration.RequiredValue";

    private string AppendRequiredValidationRule(string rulesXml)
    {
        if (string.IsNullOrWhiteSpace(rulesXml))
        {
            return
                @"<validationrulesdata><ValidationRuleConfiguration><ValidationRuleIdentifier>Kentico.Administration.RequiredValue</ValidationRuleIdentifier><RuleValues /></ValidationRuleConfiguration></validationrulesdata>";
        }

        XmlDocument document = new XmlDocument();
        document.LoadXml($"<root>{rulesXml}</root>");
        var mbIdentifierNodes = document.SelectNodes("//ValidationRuleIdentifier");

        if (mbIdentifierNodes != null)
        {
            if (mbIdentifierNodes.Cast<XmlElement>().Any(identifierNode => identifierNode is { InnerText: RequiredRuleIdentifier }))
            {
                return rulesXml;
            }
        }

        const string RULE_PROTOTYPE =
            @"<ValidationRuleConfiguration><ValidationRuleIdentifier>Kentico.Administration.RequiredValue</ValidationRuleIdentifier><RuleValues /></ValidationRuleConfiguration>";
        var prototype = new XmlDocument();
        prototype.LoadXml(RULE_PROTOTYPE);

        Debug.Assert(prototype.FirstChild != null, "prototype.FirstChild != null");
        var oNode = prototype.FirstChild.CloneNode(true);

        Debug.Assert(document.FirstChild != null, "document.FirstChild != null");
        document.FirstChild.AppendChild(document.ImportNode(oNode, true));
        return document.FirstChild.InnerXml;
    }
}