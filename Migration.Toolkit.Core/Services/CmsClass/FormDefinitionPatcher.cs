namespace Migration.Toolkit.Core.Services.CmsClass;

using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.XPath;
using CMS.DataEngine.Query;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.KX13.Auxiliary;

// TODO tomas.krch: 2023-11-02 update patcher to incorporate V27 migration

public class FormDefinitionPatcher
{
    private const string CATEGORY_ELEM                   = "category";
    private const string CATEGORY_ATTR_NAME              = FIELD_ATTR_NAME;
    private const string FIELD_ATTR_COLUMN               = "column";
    private const string FIELD_ATTR_COLUMNTYPE           = "columntype";
    private const string FIELD_ATTR_ENABLED              = "enabled";
    private const string FIELD_ATTR_GUID                 = "guid";
    private const string FIELD_ATTR_ISPK                 = "isPK";
    private const string FIELD_ATTR_NAME                 = "name";
    private const string FIELD_ATTR_SIZE                 = "size";
    private const int    FIELD_ATTR_SIZE_ZERO            = 0;
    private const string FIELD_ATTR_SYSTEM               = "system";
    private const string FIELD_ATTR_VISIBLE              = "visible";
    private const string FIELD_ELEM                      = "field";
    private const string FIELD_ELEM_PROPERTIES           = "properties";
    private const string FIELD_ELEM_SETTINGS             = "settings";
    private const string PROPERTIES_ELEM_DEFAULTVALUE    = "defaultvalue";
    private const string SETTINGS_ELEM_CONTROLNAME       = "controlname";
    private const string SETTINGS_MAXIMUMASSETS          = "MaximumAssets";
    private const string SETTINGS_MAXIMUMASSETS_FALLBACK = "99";
    private const string SETTINGS_MAXIMUMPAGES           = "MaximumPages";
    private const string SETTINGS_MAXIMUMPAGES_FALLBACK  = "99";
    private const string SETTINGS_ROOTPATH               = "RootPath";
    private const string SETTINGS_ROOTPATH_FALLBACK      = "/";

    private readonly ILogger _logger;
    private readonly string _formDefinitionXml;
    private readonly FieldMigrationService _fieldMigrationService;
    private readonly bool _classIsForm;
    private readonly bool _classIsDocumentType;
    private readonly bool _discardSysFields;
    private readonly bool _classIsCustom;
    private readonly bool _altForm;
    private readonly XDocument _xDoc;

    public FormDefinitionPatcher(ILogger logger,
        string formDefinitionXml,
        FieldMigrationService fieldMigrationService,
        bool classIsForm,
        bool classIsDocumentType,
        bool discardSysFields,
        bool classIsCustom,
        bool altForm = false)
    {
        _logger = logger;
        _formDefinitionXml = formDefinitionXml;
        _fieldMigrationService = fieldMigrationService;
        _classIsForm = classIsForm;
        _classIsDocumentType = classIsDocumentType;
        _discardSysFields = discardSysFields;
        _classIsCustom = classIsCustom;
        _altForm = altForm;
        _xDoc = XDocument.Parse(_formDefinitionXml);
    }

    public IEnumerable<string?> GetFieldNames()
    {
        return _xDoc.XPathSelectElements($"//{FIELD_ELEM}").Select(x => x.Attribute(FIELD_ATTR_COLUMN)?.Value);
    }

    public void RemoveCategories()
    {
        var categories = (_xDoc.Root?.XPathSelectElements($"//{CATEGORY_ELEM}") ?? Enumerable.Empty<XElement>()).ToList();
        foreach (var xElement in categories)
        {
            var elementDescriptor = xElement.ToString();
            if (xElement.Attribute(FIELD_ATTR_NAME)?.Value is { } name)
            {
                elementDescriptor = name;
            }

            _logger.LogDebug("Removing category '{CategoryDescriptor}'", elementDescriptor);
            xElement.Remove();
        }
    }

    public void RemoveFields(string diffAgainstDefinition)
    {
        var otherDoc = XDocument.Parse(diffAgainstDefinition);

        if (otherDoc.Root?.Elements() is { } elements)
        {
            var elementList = elements.ToList();
            foreach (XElement field in elementList)
            {
                if (field.Attribute(FIELD_ATTR_COLUMN)?.Value is { } fieldToRemoveName)
                {
                    if (_xDoc.XPathSelectElements($"//{FIELD_ELEM}[@column={fieldToRemoveName}]") is { } fieldToRemove)
                    {
                        _logger.LogDebug("Field {FieldName} removed from definition", fieldToRemoveName);
                        fieldToRemove.Remove();
                    }
                    else
                    {
                        _logger.LogDebug("Field {FieldName} not found, cannot remove from definition", fieldToRemoveName);
                    }
                }
            }
        }
        else
        {
            _logger.LogError("Unable to parse form definition: {FormDefinition}", diffAgainstDefinition);
        }
    }

    public void PatchFields()
    {
        if (_xDoc.Root?.Elements() is { } elements)
        {
            var elementList = elements.ToList();
            foreach (var fieldOrCategory in elementList)
            {
                if (fieldOrCategory.Name == FIELD_ELEM)
                {
                    PatchField(fieldOrCategory);
                }
                else if (fieldOrCategory.Name == CATEGORY_ELEM)
                {
                    _logger.LogDebug("Category '{Category}' skipped", fieldOrCategory.Attribute(CATEGORY_ATTR_NAME)?.Value ?? "<no category name>");
                }
                else
                {
                    _logger.LogWarning("Unknown element '{Element}'", fieldOrCategory.Name);
                }
            }
        }
        else
        {
            _logger.LogError("Unable to parse form definition: {FormDefinition}", _formDefinitionXml);
        }
    }

    public string? GetPatched()
    {
        return _xDoc.Root?.ToString();
    }

    private void PatchField(XElement field)
    {
        var columnAttr = field.Attribute(FIELD_ATTR_COLUMN);
        var systemAttr = field.Attribute(FIELD_ATTR_SYSTEM);
        var isPkAttr = field.Attribute(FIELD_ATTR_ISPK);
        var columnTypeAttr = field.Attribute(FIELD_ATTR_COLUMNTYPE);
        var visibleAttr = field.Attribute(FIELD_ATTR_VISIBLE);
        var enabledAttr = field.Attribute(FIELD_ATTR_ENABLED);
        var guidAttr = field.Attribute(FIELD_ATTR_GUID);

        var isPk = bool.TryParse(isPkAttr?.Value, out var isPkParsed) && isPkParsed;
        var system = bool.TryParse(systemAttr?.Value, out var sysParsed) && sysParsed;

        var fieldDescriptor = (columnAttr ?? guidAttr)?.Value ?? "<no guid or column>";

        if (_discardSysFields && (system || isPk))
        {
            _logger.LogDebug("Discard sys filed == true => Removing field sys '{Field}'", fieldDescriptor);
            field.Remove();
            return;
        }

        var columnType = columnTypeAttr?.Value;
        if (columnType == null)
        {
            if (isPk) return;

            _logger.LogError("Field ('{Field}') 'columnType' attribute is required", fieldDescriptor);
            return;
        }


        var controlNameElem = field.XPathSelectElement($"{FIELD_ELEM_SETTINGS}/{SETTINGS_ELEM_CONTROLNAME}");
        var controlName = controlNameElem?.Value;

        if (_fieldMigrationService.GetFieldMigration(columnType, controlName, columnAttr?.Value) is var (sourceDataType, targetDataType, sourceFormControl, targetFormComponent, actions, fieldNameRegex))
        {
            _logger.LogDebug("Field {FieldDescriptor} DataType: {SourceDataType} => {TargetDataType}", fieldDescriptor, columnType, targetDataType);
            columnTypeAttr?.SetValue(targetDataType);
            switch (targetFormComponent)
            {
                case TfcDirective.DoNothing:
                    _logger.LogDebug("Field {FieldDescriptor} ControlName: Tca:{TcaDirective}", fieldDescriptor, targetFormComponent);
                    PerformActionsOnField(field, fieldDescriptor, actions);
                    break;
                case TfcDirective.Clear:
                    _logger.LogDebug("Field {FieldDescriptor} ControlName: Tca:{TcaDirective}", fieldDescriptor, targetFormComponent);
                    field.RemoveNodes();
                    visibleAttr?.SetValue(false);
                    break;
                case TfcDirective.CopySourceControl:
                    // TODO tk: 2022-10-06 support only for custom controls
                    _logger.LogDebug("Field {FieldDescriptor} ControlName: Tca:{TcaDirective} => {ControlName}", fieldDescriptor, targetFormComponent, controlName);
                    controlNameElem?.SetValue(controlName);
                    PerformActionsOnField(field, fieldDescriptor, actions);
                    break;
                default:
                {
                    _logger.LogDebug("Field {FieldDescriptor} ControlName: Tca:NONE => from control '{ControlName}' => {TargetFormComponent}", fieldDescriptor, controlName, targetFormComponent);
                    controlNameElem?.SetValue(targetFormComponent);
                    PerformActionsOnField(field, fieldDescriptor, actions);
                    break;
                }
            }
        }


        if (!_classIsForm && !_classIsDocumentType)
        {
            var hasVisibleAttribute = visibleAttr != null;
            if (enabledAttr is { } enabled)
            {
                enabled.Remove();
                _logger.LogDebug("Removing field '{Field}' attribute '{Attribute}'", fieldDescriptor, FIELD_ATTR_ENABLED);
            }

            if (system && _classIsCustom)
            {
                systemAttr?.Remove();
                _logger.LogDebug("Removing field '{Field}' attribute '{Attribute}'", fieldDescriptor, systemAttr?.Name);
            }

            if (hasVisibleAttribute && visibleAttr?.Value is { } visibleValue)
            {
                field.Add(new XAttribute(FIELD_ATTR_ENABLED, visibleValue));
                _logger.LogDebug("Set field '{Field}' attribute '{Attribute}' to value '{Value}' from attribute '{SourceAttribute}'", fieldDescriptor, FIELD_ATTR_ENABLED, visibleValue, FIELD_ATTR_VISIBLE);
            }

            if (!_altForm)
            {
                if (hasVisibleAttribute)
                {
                    visibleAttr?.Remove();
                    _logger.LogDebug("Removing field '{Field}' attribute '{Attribute}'", fieldDescriptor, FIELD_ATTR_VISIBLE);
                }
            }

            foreach (var fieldChildNode in field.Elements().ToList())
            {
                _logger.LogDebug("Patching filed child '{FieldChildName}'", fieldChildNode.Name);
                switch (fieldChildNode.Name.ToString())
                {
                    case FIELD_ELEM_PROPERTIES:
                    {
                        PatchProperties(fieldChildNode);
                        break;
                    }
                    case FIELD_ELEM_SETTINGS:
                    {
                        if (_altForm)
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
                        _logger.LogDebug("Removing field element '{ElementName}'", fieldChildNode.Name);
                        fieldChildNode.Remove();
                        break;
                    }
                }
            }
        }

        if (_classIsForm || _classIsDocumentType)
        {
            if (field.Attribute(FIELD_ATTR_VISIBLE) is { } visible)
            {
                field.Add(new XAttribute(FIELD_ATTR_ENABLED, visible.Value));
                _logger.LogDebug("Set field '{Field}' attribute '{Attribute}' to value '{Value}' from attribute '{SourceAttribute}'", fieldDescriptor, FIELD_ATTR_ENABLED, visible, FIELD_ATTR_VISIBLE);
            }
        }
    }

    private void ClearSettings(XElement settingsElem)
    {
        var elementsToRemove = settingsElem.Elements().ToList();
        foreach (var element in elementsToRemove)
        {
            _logger.LogDebug("Removing settings element '{ElementName}'", element.Name);
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
            .Where(element => element.Name != SETTINGS_ELEM_CONTROLNAME)
            .ToList();

        foreach (var element in elementsToRemove)
        {
            _logger.LogDebug("Removing settings element '{ElementName}'", element.Name);
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
            .Where(element => element.Name != PROPERTIES_ELEM_DEFAULTVALUE)
            .ToList();

        foreach (var element in elementsToRemove)
        {
            _logger.LogDebug("Removing properties element '{ElementName}'", element.Name);
            element.Remove();
        }

        if (!properties.Elements().Any())
        {
            _logger.LogDebug("Properties element is empty => removing");
            properties.Remove();
        }
    }

    private void PerformActionsOnField(XElement field, string fieldDescriptor, string[]? actions)
    {
        if (actions == null) return;

        foreach (var action in actions)
        {
            _logger.LogDebug("Field {FieldDescriptor} Action: {Action}", fieldDescriptor, action);
            switch (action)
            {
                case TcaDirective.ClearSettings:
                {
                    field.Element(FIELD_ELEM_SETTINGS)?.Remove();
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
                        .EnsureElement(FIELD_ELEM_SETTINGS)
                        .EnsureElement(SETTINGS_MAXIMUMASSETS, maxAssets => maxAssets.Value = SETTINGS_MAXIMUMASSETS_FALLBACK);
                    break;
                }
                case TcaDirective.ConvertToPages:
                {
                    field
                        .EnsureElement(FIELD_ELEM_SETTINGS, settings =>
                        {
                            settings.EnsureElement(SETTINGS_MAXIMUMPAGES, maxAssets => maxAssets.Value = SETTINGS_MAXIMUMPAGES_FALLBACK);
                            settings.EnsureElement(SETTINGS_ROOTPATH, maxAssets => maxAssets.Value = SETTINGS_ROOTPATH_FALLBACK); // TODO tk: 2022-08-31 describe why?
                        });

                    field.SetAttributeValue(FIELD_ATTR_SIZE, FIELD_ATTR_SIZE_ZERO); // TODO tk: 2022-08-31 describe why?

                    var settings = field.EnsureElement(FIELD_ELEM_SETTINGS);
                    settings.EnsureElement("TreePath", element => element.Value = settings.Element("RootPath")?.Value ?? "");
                    settings.EnsureElement("RootPath").Remove();

                    break;
                }
            }
        }
    }
}