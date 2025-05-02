using System.Diagnostics;
using System.Xml;
using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
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

                target = PatchDataClassInfo(target, existingFieldGUIDs, modelFacade.SelectVersion(), [], configuration.IncludeExtendedMetadata.GetValueOrDefault(false), out string? oldPrimaryKeyName, out string? documentNameField);

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
                target = PatchFormDataClassInfo(target);
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

                target = PatchDataClassInfo(target, existingFieldGUIDs, modelFacade.SelectVersion(), [], configuration.IncludeExtendedMetadata.GetValueOrDefault(false), out string? oldPrimaryKeyName, out string? documentNameField);
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

                target = PatchDataClassInfo(target, existingFieldGUIDs, modelFacade.SelectVersion(), [], configuration.IncludeExtendedMetadata.GetValueOrDefault(false), out string? oldPrimaryKeyName, out string? documentNameField);
                break;
            }

            default:
                break;
        }

        return target;
    }

    private DataClassInfo PatchFormDataClassInfo(DataClassInfo target)
    {
        var fi = new FormInfo(target.ClassFormDefinition);
        bool legacyFormat = false;
        foreach (var item in fi.ItemsList.OfType<FormFieldInfo>())
        {
            if (!item.Settings.ContainsKey("componentidentifier"))
            {
                legacyFormat = true;
                item.Settings["componentidentifier"] = Kx13FormComponents.Kentico_TextInput;
                if (item.Settings.ContainsKey("controlname"))
                {
                    item.Settings.Remove("controlname");
                }
            }
        }
        target.ClassFormDefinition = fi.GetXmlDefinition();

        if (legacyFormat)
        {
            logger.LogWarning("Some fields of Form {FormName} have legacy format ClassFormDefinition and were converted to text input. Manual adjustments might be necessary", target.ClassName);
        }
        return target;
    }

    public static DataClassInfo PatchDataClassInfo(DataClassInfo dataClass, Dictionary<string, Guid> existingFieldGUIDs, SemanticVersion version, Dictionary<Guid, string> reusableSchemaNames, bool includeExtendedMetadata, out string? oldPrimaryKeyName, out string? mappedLegacyField)
    {
        oldPrimaryKeyName = null;
        mappedLegacyField = null;
        if (dataClass.ClassType is ClassType.CONTENT_TYPE)
        {
            var fi = new FormInfo(dataClass.ClassFormDefinition);
            string tableName = dataClass.ClassTableName;
            var contentTypeManager = Service.Resolve<IContentTypeManager>();

            if (!fi.ItemsList.OfType<FormFieldInfo>().Any(x => x.Name == nameof(ContentItemDataInfo.ContentItemDataGUID)))
            {
                contentTypeManager.Initialize(dataClass);
            }

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
                if (!nfi.ItemsList.Any(x => IsSameFormElement(reusableSchemaNames, dataDefinitionItem, x)))
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
            }

            Debug.WriteLineIf(oldPrimaryKeyName == null, $"WARN: old PK is null for class '{dataClass.ClassName}'");

            foreach (var field in GetLegacyMetadataFields(version, includeExtendedMetadata))
            {
                AppendLegacyMetadataField(nfi, dataClass.ClassName, field, reusableSchemaNames, out mappedLegacyField);
            }
            dataClass.ClassFormDefinition = nfi.GetXmlDefinition();

            return dataClass;
        }

        return dataClass;
    }

    public static IEnumerable<LegacyDocumentMetadataFieldMapping> GetLegacyMetadataFields(SemanticVersion version, bool includeExtended)
    {
        List<LegacyDocumentMetadataFieldMapping> fields = [new("DocumentName", "Page Name", 100, false, x => x.DocumentName)];
        if (includeExtended && version is { Major: 12 or 13 })
        {
            fields.AddRange([
                new(nameof(ICmsDocument.DocumentPageTitle), "Page Title", -1, true, x => x.DocumentPageTitle),
                new(nameof(ICmsDocument.DocumentPageDescription), "Page Description", -1, true, x => x.DocumentPageDescription),
                new(nameof(ICmsDocument.DocumentPageKeyWords), "Page Keywords", -1, true, x => x.DocumentPageKeyWords),
            ]);
        }
        return fields;
    }
    public static string? GetMappedLegacyField(FormInfo nfi, string className, string legacyFieldName)
    {
        if (nfi.GetFields(true, true).FirstOrDefault(f => GuidHelper.CreateFieldGuid($"{legacyFieldName.ToLower()}|{className}").Equals(f.Guid)) is { } foundField)
        {
            return foundField.Name;
        }

        return null;
    }

    private static void AppendLegacyMetadataField(FormInfo nfi, string newClassName, LegacyDocumentMetadataFieldMapping mapping, Dictionary<Guid, string> reusableSchemaNames, out string targetFieldName)
    {
        if (GetMappedLegacyField(nfi, newClassName, mapping.LegacyFieldName) is { } fieldName)
        {
            targetFieldName = fieldName;
            return;
        }

        targetFieldName = mapping.LegacyFieldName;
        int i = 0;
        while (nfi.GetFormField(targetFieldName) is not null)
        {
            targetFieldName = $"{mapping.LegacyFieldName}{++i}";
        }

        var formFieldInfo = new FormFieldInfo
        {
            Caption = mapping.TargetCaption,
            Name = targetFieldName,
            AllowEmpty = mapping.AllowEmpty,
            DataType = mapping.TargetSize switch { -1 => "longtext", _ => "text" },
            Size = mapping.TargetSize switch { -1 => 0, _ => mapping.TargetSize },
            Precision = 0,
            DefaultValue = null,
            Guid = GuidHelper.CreateFieldGuid($"{mapping.LegacyFieldName.ToLower()}|{newClassName}"),
            System = false,
            Settings = { { "controlname", "Kentico.Administration.TextInput" } }
        };

        if (!nfi.ItemsList.Any(x => IsSameFormElement(reusableSchemaNames, formFieldInfo, x)))
        {
            nfi.AddFormItem(formFieldInfo);
        }
    }

    private static bool IsSameFormElement(Dictionary<Guid, string> reusableSchemaNames, IDataDefinitionItem element1, IDataDefinitionItem element2)
    {
        if (element1 is FormFieldInfo ffi1 && element2 is FormFieldInfo ffi2)
        {
            return ffi1.Name.Equals(ffi2.Name, StringComparison.InvariantCultureIgnoreCase);
        }
        else if (element1 is FormSchemaInfo fsi1 && element2 is FormSchemaInfo fsi2)
        {
            return reusableSchemaNames.TryGetValue(fsi1.Guid, out string? schemaName1) &&
                reusableSchemaNames.TryGetValue(fsi2.Guid, out string? schemaName2) &&
                schemaName1.Equals(schemaName2, StringComparison.InvariantCultureIgnoreCase);
        }
        else
        {
            return false;
        }
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
