using System.Xml.Linq;
using System.Xml.XPath;

using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;

namespace Migration.Toolkit.KXP.Api.Services.CmsClass;

public class FormDefinitionPatcher
{
    private const string CategoryElem = "category";
    private const string CategoryAttrName = FieldAttrName;
    private const string FieldAttrColumn = "column";
    private const string FieldAttrColumntype = "columntype";
    private const string FieldAttrEnabled = "enabled";
    private const string FieldAttrGuid = "guid";
    private const string FieldAttrIspk = "isPK";
    private const string FieldAttrName = "name";
    private const string FieldAttrSize = "size";
    private const int FieldAttrSizeZero = 0;
    private const string FieldAttrSystem = "system";
    private const string FieldAttrVisible = "visible";
    private const string FieldElem = "field";
    private const string FieldElemProperties = "properties";
    private const string FieldElemSettings = "settings";
    private const string PropertiesElemDefaultvalue = "defaultvalue";
    private const string SettingsElemControlname = "controlname";
    private const string SettingsMaximumassets = "MaximumAssets";
    private const string SettingsMaximumassetsFallback = "99";
    private const string SettingsMaximumpages = "MaximumPages";
    private const string SettingsMaximumpagesFallback = "99";
    private const string SettingsRootpath = "RootPath";
    private const string SettingsRootpathFallback = "/";

    private readonly IReadOnlySet<string> allowedFieldAttributes = new HashSet<string>([
        // taken from FormFieldInfo.GetAttributes() method
        "column",
        "visible",
        "enabled",
        "columntype",
        "allowempty",
        "isPK",
        "system",
        "columnsize",
        "columnprecision",
        "guid",
        "external",
        "isinherited",
        "mappedtofield",
        "dummy",
        "isunique",
        "refobjtype",
        "reftype",
        "resolvedefaultvalue"
    ], StringComparer.InvariantCultureIgnoreCase);

    private readonly bool altForm;
    private readonly bool classIsCustom;
    private readonly bool classIsDocumentType;
    private readonly bool classIsForm;
    private readonly bool discardSysFields;
    private readonly FieldMigrationService fieldMigrationService;
    private readonly string formDefinitionXml;

    private readonly ILogger logger;
    private readonly XDocument xDoc;

    public FormDefinitionPatcher(ILogger logger,
        string formDefinitionXml,
        FieldMigrationService fieldMigrationService,
        bool classIsForm,
        bool classIsDocumentType,
        bool discardSysFields,
        bool classIsCustom,
        bool altForm = false)
    {
        this.logger = logger;
        this.formDefinitionXml = formDefinitionXml;
        this.fieldMigrationService = fieldMigrationService;
        this.classIsForm = classIsForm;
        this.classIsDocumentType = classIsDocumentType;
        this.discardSysFields = discardSysFields;
        this.classIsCustom = classIsCustom;
        this.altForm = altForm;
        xDoc = XDocument.Parse(this.formDefinitionXml);
    }

    public IEnumerable<string?> GetFieldNames() => xDoc.XPathSelectElements($"//{FieldElem}").Select(x => x.Attribute(FieldAttrColumn)?.Value);

    public void RemoveCategories()
    {
        var categories = (xDoc.Root?.XPathSelectElements($"//{CategoryElem}") ?? Enumerable.Empty<XElement>()).ToList();
        foreach (var xElement in categories)
        {
            string elementDescriptor = xElement.ToString();
            if (xElement.Attribute(FieldAttrName)?.Value is { } name)
            {
                elementDescriptor = name;
            }

            logger.LogDebug("Removing category '{CategoryDescriptor}'", elementDescriptor);
            xElement.Remove();
        }
    }

    public void RemoveFields(string diffAgainstDefinition)
    {
        var otherDoc = XDocument.Parse(diffAgainstDefinition);

        if (otherDoc.Root?.Elements() is { } elements)
        {
            var elementList = elements.ToList();
            foreach (var field in elementList)
            {
                if (field.Attribute(FieldAttrColumn)?.Value is { } fieldToRemoveName)
                {
                    if (xDoc.XPathSelectElements($"//{FieldElem}[@column={fieldToRemoveName}]") is { } fieldToRemove)
                    {
                        logger.LogDebug("Field {FieldName} removed from definition", fieldToRemoveName);
                        fieldToRemove.Remove();
                    }
                    else
                    {
                        logger.LogDebug("Field {FieldName} not found, cannot remove from definition", fieldToRemoveName);
                    }
                }
            }
        }
        else
        {
            logger.LogError("Unable to parse form definition: {FormDefinition}", diffAgainstDefinition);
        }
    }

    public void PatchFields()
    {
        if (xDoc.Root?.Elements() is { } elements)
        {
            var elementList = elements.ToList();
            foreach (var fieldOrCategory in elementList)
            {
                if (fieldOrCategory.Name == FieldElem)
                {
                    PatchField(fieldOrCategory);
                }
                else if (fieldOrCategory.Name == CategoryElem)
                {
                    logger.LogDebug("Category '{Category}' skipped", fieldOrCategory.Attribute(CategoryAttrName)?.Value ?? "<no category name>");
                }
                else
                {
                    logger.LogWarning("Unknown element '{Element}'", fieldOrCategory.Name);
                }
            }
        }
        else
        {
            logger.LogError("Unable to parse form definition: {FormDefinition}", formDefinitionXml);
        }
    }

    public string? GetPatched() => xDoc.Root?.ToString();

    private void PatchField(XElement field)
    {
        var columnAttr = field.Attribute(FieldAttrColumn);
        var systemAttr = field.Attribute(FieldAttrSystem);
        var isPkAttr = field.Attribute(FieldAttrIspk);
        var columnTypeAttr = field.Attribute(FieldAttrColumntype);
        var visibleAttr = field.Attribute(FieldAttrVisible);
        var enabledAttr = field.Attribute(FieldAttrEnabled);
        var guidAttr = field.Attribute(FieldAttrGuid);

        bool isPk = bool.TryParse(isPkAttr?.Value, out bool isPkParsed) && isPkParsed;
        bool system = bool.TryParse(systemAttr?.Value, out bool sysParsed) && sysParsed;

        string fieldDescriptor = (columnAttr ?? guidAttr)?.Value ?? "<no guid or column>";

        // cleanup of no longer supported fields
        foreach (var a in field.Attributes())
        {
            string an = a.Name.ToString();
            if (!allowedFieldAttributes.Contains(an))
            {
                a.Remove();
                logger.LogTrace("Removing attribute '{AttributeName}'='{Value}' from field with column '{ColumnName}'", an, a.Value, columnAttr?.Value);
            }
        }

        if (discardSysFields && (system || isPk))
        {
            logger.LogDebug("Discard sys filed == true => Removing field sys '{Field}'", fieldDescriptor);
            field.Remove();
            return;
        }

        string? columnType = columnTypeAttr?.Value;
        if (columnType == null)
        {
            if (isPk)
            {
                return;
            }

            logger.LogError("Field ('{Field}') 'columnType' attribute is required", fieldDescriptor);
            return;
        }


        var controlNameElem = field.XPathSelectElement($"{FieldElemSettings}/{SettingsElemControlname}");
        string? controlName = controlNameElem?.Value;

        if (fieldMigrationService.GetFieldMigration(columnType, controlName, columnAttr?.Value) is var (_, targetDataType, _, targetFormComponent, actions, _))
        {
            logger.LogDebug("Field {FieldDescriptor} DataType: {SourceDataType} => {TargetDataType}", fieldDescriptor, columnType, targetDataType);
            columnTypeAttr?.SetValue(targetDataType);
            switch (targetFormComponent)
            {
                case TfcDirective.DoNothing:
                    logger.LogDebug("Field {FieldDescriptor} ControlName: Tca:{TcaDirective}", fieldDescriptor, targetFormComponent);
                    PerformActionsOnField(field, fieldDescriptor, actions);
                    break;
                case TfcDirective.Clear:
                    logger.LogDebug("Field {FieldDescriptor} ControlName: Tca:{TcaDirective}", fieldDescriptor, targetFormComponent);
                    field.RemoveNodes();
                    visibleAttr?.SetValue(false);
                    break;
                case TfcDirective.CopySourceControl:
                    // TODO tk: 2022-10-06 support only for custom controls
                    logger.LogDebug("Field {FieldDescriptor} ControlName: Tca:{TcaDirective} => {ControlName}", fieldDescriptor, targetFormComponent, controlName);
                    controlNameElem?.SetValue(controlName);
                    PerformActionsOnField(field, fieldDescriptor, actions);
                    break;
                default:
                {
                    logger.LogDebug("Field {FieldDescriptor} ControlName: Tca:NONE => from control '{ControlName}' => {TargetFormComponent}", fieldDescriptor, controlName, targetFormComponent);
                    controlNameElem?.SetValue(targetFormComponent);
                    PerformActionsOnField(field, fieldDescriptor, actions);
                    break;
                }
            }
        }


        if (!classIsForm && !classIsDocumentType)
        {
            bool hasVisibleAttribute = visibleAttr != null;
            if (enabledAttr is { } enabled)
            {
                enabled.Remove();
                logger.LogDebug("Removing field '{Field}' attribute '{Attribute}'", fieldDescriptor, FieldAttrEnabled);
            }

            if (system && classIsCustom)
            {
                systemAttr?.Remove();
                logger.LogDebug("Removing field '{Field}' attribute '{Attribute}'", fieldDescriptor, systemAttr?.Name);
            }

            if (hasVisibleAttribute && visibleAttr?.Value is { } visibleValue)
            {
                field.Add(new XAttribute(FieldAttrEnabled, visibleValue));
                logger.LogDebug("Set field '{Field}' attribute '{Attribute}' to value '{Value}' from attribute '{SourceAttribute}'", fieldDescriptor, FieldAttrEnabled, visibleValue, FieldAttrVisible);
            }

            if (!altForm)
            {
                if (hasVisibleAttribute)
                {
                    visibleAttr?.Remove();
                    logger.LogDebug("Removing field '{Field}' attribute '{Attribute}'", fieldDescriptor, FieldAttrVisible);
                }
            }

            foreach (var fieldChildNode in field.Elements().ToList())
            {
                logger.LogDebug("Patching filed child '{FieldChildName}'", fieldChildNode.Name);
                switch (fieldChildNode.Name.ToString())
                {
                    case FieldElemProperties:
                    {
                        PatchProperties(fieldChildNode);
                        break;
                    }
                    case FieldElemSettings:
                    {
                        if (altForm)
                        {
                            PatchSettings(fieldChildNode);
                        }
                        else
                        {
                            // XbK Resource / Module class no longer supports visual representation
                            ClearSettings(fieldChildNode);
                        }

                        break;
                    }
                    default:
                    {
                        logger.LogDebug("Removing field element '{ElementName}'", fieldChildNode.Name);
                        fieldChildNode.Remove();
                        break;
                    }
                }
            }
        }

        if (classIsForm || classIsDocumentType)
        {
            if (field.Attribute(FieldAttrVisible) is { } visible)
            {
                field.Add(new XAttribute(FieldAttrEnabled, visible.Value));
                logger.LogDebug("Set field '{Field}' attribute '{Attribute}' to value '{Value}' from attribute '{SourceAttribute}'", fieldDescriptor, FieldAttrEnabled, visible, FieldAttrVisible);
            }
        }
    }

    private void ClearSettings(XElement settingsElem)
    {
        var elementsToRemove = settingsElem.Elements().ToList();
        foreach (var element in elementsToRemove)
        {
            logger.LogDebug("Removing settings element '{ElementName}'", element.Name);
            element.Remove();
        }

        if (!settingsElem.Elements().Any())
        {
            settingsElem.Remove();
        }
    }

    private void PatchSettings(XElement settingsElem)
    {
        var elementsToRemove = settingsElem.Elements()
            .Where(element => element.Name != SettingsElemControlname)
            .ToList();

        foreach (var element in elementsToRemove)
        {
            logger.LogDebug("Removing settings element '{ElementName}'", element.Name);
            element.Remove();
        }

        if (!settingsElem.Elements().Any())
        {
            settingsElem.Remove();
        }
    }


    private void PatchProperties(XElement properties)
    {
        var elementsToRemove = properties.Elements()
            .Where(element => element.Name != PropertiesElemDefaultvalue)
            .ToList();

        foreach (var element in elementsToRemove)
        {
            logger.LogDebug("Removing properties element '{ElementName}'", element.Name);
            element.Remove();
        }

        if (!properties.Elements().Any())
        {
            logger.LogDebug("Properties element is empty => removing");
            properties.Remove();
        }
    }

    private void PerformActionsOnField(XElement field, string fieldDescriptor, string[]? actions)
    {
        if (actions == null)
        {
            return;
        }

        foreach (string action in actions)
        {
            logger.LogDebug("Field {FieldDescriptor} Action: {Action}", fieldDescriptor, action);
            switch (action)
            {
                case TcaDirective.ClearSettings:
                {
                    field.Element(FieldElemSettings)?.Remove();
                    break;
                }
                case TcaDirective.ClearMacroTable:
                {
                    // TODO tk: 2022-10-11 really needed?
                    break;
                }
                case TcaDirective.ConvertToAsset:
                {
                    field
                        .EnsureElement(FieldElemSettings)
                        .EnsureElement(SettingsMaximumassets, maxAssets => maxAssets.Value = SettingsMaximumassetsFallback);
                    break;
                }
                case TcaDirective.ConvertToPages:
                {
                    field
                        .EnsureElement(FieldElemSettings, settings =>
                        {
                            settings.EnsureElement(SettingsMaximumpages, maxAssets => maxAssets.Value = SettingsMaximumpagesFallback);
                            settings.EnsureElement(SettingsRootpath, maxAssets => maxAssets.Value = SettingsRootpathFallback); // TODO tk: 2022-08-31 describe why?
                        });

                    field.SetAttributeValue(FieldAttrSize, FieldAttrSizeZero); // TODO tk: 2022-08-31 describe why?

                    var settings = field.EnsureElement(FieldElemSettings);
                    settings.EnsureElement("TreePath", element => element.Value = settings.Element("RootPath")?.Value ?? "");
                    settings.EnsureElement("RootPath").Remove();

                    break;
                }

                default:
                    break;
            }
        }
    }
}
