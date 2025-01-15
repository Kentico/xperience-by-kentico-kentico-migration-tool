using System.Diagnostics;
using System.Xml;
using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Enumerations;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Helpers;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Mappers;

public class CmsClassMapper(
    ILogger<CmsClassMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    FieldMigrationService fieldMigrationService,
    ModelFacade modelFacade,
    ToolConfiguration configuration
)
    : EntityMapperBase<ICmsClass, DataClassInfo>(logger,
        primaryKeyMappingContext, protocol)
{
    private const string REQUIRED_RULE_IDENTIFIER = "Kentico.Administration.RequiredValue";

    protected override DataClassInfo? CreateNewInstance(ICmsClass source, MappingHelper mappingHelper, AddFailure addFailure) =>
        DataClassInfo.New();

    protected override DataClassInfo MapInternal(ICmsClass source, DataClassInfo target, bool newInstance, MappingHelper mappingHelper,
        AddFailure addFailure)
    {
        target.ClassDisplayName = source.ClassDisplayName;
        target.ClassName = source.ClassName;

        bool isCustomizableSystemClass = false;
        bool classIsCustom = true;
        var classResource = modelFacade.SelectById<ICmsResource>(source.ClassResourceID);
        if (classResource?.ResourceName is { } resourceName)
        {
            isCustomizableSystemClass = source.ClassShowAsSystemTable.GetValueOrDefault(false) &&
                                        K12SystemResource.All.Contains(resourceName);

            classIsCustom = !K12SystemResource.All.Contains(resourceName);

            logger.LogDebug("{ClassName} is {@Properties}", source.ClassName, new { isCustomizableSystemClass, classIsCustom, source.ClassResourceID, classResource?.ResourceName });
        }

        var existingFieldGUIDs = new FormInfo(target.ClassFormDefinition).ItemsList.OfType<FormFieldInfo>().ToDictionary(x => x.Name, x => x.Guid);

        FormDefinitionHelper.MapFormDefinitionFields(logger, fieldMigrationService, source, target, isCustomizableSystemClass, classIsCustom);

        target.ClassHasUnmanagedDbSchema = false;
        if (!string.IsNullOrWhiteSpace(source.ClassTableName))
        {
            target.ClassTableName = source.ClassTableName;
        }

        target.ClassShowTemplateSelection = source.ClassShowTemplateSelection.UseKenticoDefault();
        target.ClassLastModified = source.ClassLastModified;
        target.ClassGUID = source.ClassGUID;

        if (source.ClassContactMapping != null)
        {
            var mapInfo = new FormInfo(source.ClassContactMapping);
            var newMappings = new FormInfo();
            if (mapInfo.ItemsList.Count > 0)
            {
                var ffiLookup = mapInfo.ItemsList.OfType<FormFieldInfo>().ToLookup(f => f.MappedToField, f => f);

                foreach (var formFieldInfos in ffiLookup)
                {
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
        if (source.ClassIsDocumentType)
        {
            target.ClassWebPageHasUrl = source switch
            {
                CmsClassK13 { ClassHasURL: false } => false,
                _ => true
            };
        }

        if (mappingHelper.TranslateIdAllowNulls<ICmsResource>(c => c.ResourceID, source.ClassResourceID, out int? resourceId))
        {
            if (resourceId.HasValue)
            {
                target.ClassResourceID = resourceId.Value;
            }
        }

        switch (source)
        {
            // Special
            case { ClassName: { } className } when
                className.Equals("cms.site", StringComparison.InvariantCultureIgnoreCase) ||
                className.Equals("cms.root", StringComparison.InvariantCultureIgnoreCase)
                :
            {
                throw new Exception("Unable to map obsolete dataclass");
                return target;
            }
            // Target Other,null
            // Target System,null
            case not null when target is { ClassType: ClassType.OTHER or ClassType.SYSTEM_TABLE }:
            {
                break;
            }

            default:
                break;
        }

        switch (source)
        {
            // Custom class from custom resource?
            case
            {
                ClassIsForm: false or null,
                ClassIsDocumentType: false,
                ClassResourceID: { } classResourceId
            }:
            {
                target.ClassType = ClassType.OTHER;
                target.ClassContentTypeType = null;

                target = PatchDataClassInfo(target, existingFieldGUIDs, out string? oldPrimaryKeyName, out string? documentNameField);

                break;
            }

            // Target Form,null
            case
            {
                ClassIsDocumentType: false,
                ClassIsCoupledClass: true,
                ClassIsForm: true
                // ClassIsPage: false
            }
                :
            {
                target.ClassType = ClassType.FORM;
                target.ClassContentTypeType = "";

                break;
            }

            // Target Content,Reusable
            case
            {
                ClassIsDocumentType: false,
                ClassIsForm: false or null,
                ClassShowAsSystemTable: false,
                ClassIsCustomTable: false
                // ClassIsPage: false
            }:
            {
                target.ClassType = ClassType.CONTENT_TYPE;
                target.ClassContentTypeType = ClassContentTypeType.REUSABLE;

                target = PatchDataClassInfo(target, existingFieldGUIDs, out string? oldPrimaryKeyName, out string? documentNameField);
                break;
            }

            // Target Content,Website
            case { ClassName: { } className } when className.Equals("cms.folder", StringComparison.InvariantCultureIgnoreCase):
            case
            {
                ClassIsDocumentType: true,
                ClassIsForm: false or null
                // ClassIsPage: true
            }:
            {
                target.ClassType = ClassType.CONTENT_TYPE;
                target.ClassContentTypeType = configuration.ClassNamesConvertToContentHub.Contains(target.ClassName)
                    ? ClassContentTypeType.REUSABLE
                    : ClassContentTypeType.WEBSITE;

                target = PatchDataClassInfo(target, existingFieldGUIDs, out string? oldPrimaryKeyName, out string? documentNameField);
                break;
            }

            default:
                break;
        }

        return target;
    }

    public static DataClassInfo PatchDataClassInfo(DataClassInfo dataClass, Dictionary<string, Guid> existingFieldGUIDs, out string? oldPrimaryKeyName, out string? documentNameField)
    {
        oldPrimaryKeyName = null;
        documentNameField = null;
        if (dataClass.ClassType is ClassType.CONTENT_TYPE)
        {
            var fi = new FormInfo(dataClass.ClassFormDefinition);
            string tableName = dataClass.ClassTableName;
            var contentTypeManager = Service.Resolve<IContentTypeManager>();

            contentTypeManager.Initialize(dataClass);

            if (!string.IsNullOrWhiteSpace(tableName))
            {
                dataClass.ClassTableName = tableName;
            }

            var nfi = new FormInfo(dataClass.ClassFormDefinition);
            foreach (var item in nfi.ItemsList.OfType<FormFieldInfo>())
            {
                if (existingFieldGUIDs.TryGetValue(item.Name, out var existingGUID))
                {
                    item.Guid = existingGUID;
                }
            }

            foreach (var dataDefinitionItem in fi.GetFormElements(true, true) ?? [])
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

            AppendDocumentNameField(nfi, dataClass.ClassName, out documentNameField);

            dataClass.ClassFormDefinition = nfi.GetXmlDefinition();

            return dataClass;
        }

        return dataClass;
    }

    public static string? GetLegacyDocumentName(FormInfo nfi, string className)
    {
        if (nfi.GetFields(true, true).FirstOrDefault(f => GuidHelper.CreateFieldGuid($"documentname|{className}").Equals(f.Guid)) is { } foundField)
        {
            return foundField.Name;
        }

        return null;
    }

    private static void AppendDocumentNameField(FormInfo nfi, string newClassName, out string documentNameField)
    {
        if (GetLegacyDocumentName(nfi, newClassName) is { } fieldName)
        {
            documentNameField = fieldName;
            return;
        }

        // no DocumentName in v27, we supply one in migration
        documentNameField = "DocumentName";
        int i = 0;
        while (nfi.GetFormField(documentNameField) is not null)
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
            Guid = GuidHelper.CreateFieldGuid($"documentname|{newClassName}"),
            System = false, // no longer system field, system doesn't rely on this field anymore
            Settings = { { "controlname", "Kentico.Administration.TextInput" } }
        });
    }

    private static string AppendRequiredValidationRule(string rulesXml)
    {
        if (string.IsNullOrWhiteSpace(rulesXml))
        {
            return
                @"<validationrulesdata><ValidationRuleConfiguration><ValidationRuleIdentifier>Kentico.Administration.RequiredValue</ValidationRuleIdentifier><RuleValues /></ValidationRuleConfiguration></validationrulesdata>";
        }

        var document = new XmlDocument();
        document.LoadXml($"<root>{rulesXml}</root>");
        var mbIdentifierNodes = document.SelectNodes("//ValidationRuleIdentifier");

        if (mbIdentifierNodes != null)
        {
            if (mbIdentifierNodes.Cast<XmlElement>().Any(identifierNode => identifierNode is { InnerText: REQUIRED_RULE_IDENTIFIER }))
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
