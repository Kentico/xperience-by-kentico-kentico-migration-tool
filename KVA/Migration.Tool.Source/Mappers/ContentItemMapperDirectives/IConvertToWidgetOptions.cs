using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;

public interface IConvertToWidgetOptions
{
    IWidgetLocationOptions Location { get; }
    IWidgetPropertiesOptions Properties { get; }
}

public interface IWidgetPropertiesOptions
{
    /// <param name="wrapInReusableItem">Transforms the source node to a reusable content item, which is then passed to the properties mapping delegate (and can be linked there)</param>
    /// <param name="itemToWidgetPropertiesMapping">Delegate to fill in widget properties based on the original node properties and/or child nodes</param>
    void Fill(bool wrapInReusableItem, MapWidgetPropertiesDelegate itemToWidgetPropertiesMapping);
}

public interface IWidgetLocationOptions
{
    /// <param name="parentLevel">Relative level of the page to host the widget on. -1 is direct parent, -2 is grandparent, etc.</param>
    IWidgetOnPageOptions OnAncestorPage(int parentLevel = -1);
}

public interface IWidgetOnPageOptions
{
    /// <summary>
    /// Specifies the editable area in which to create the widget. Creates or reuses editable area on the target page.
    /// </summary>
    /// <param name="sectionType">The identifier argument in RegisterSection attribute of the target instance. Mandatory if section is about to be created, otherwise ignored</param>
    /// <param name="sectionGuid">GUID of the section to find or create</param>
    IWidgetInEditableAreaOptions InEditableArea(string areaIdentifier);
}

public interface IWidgetInEditableAreaOptions
{
    /// <summary>
    /// Specifies the section in which to create the widget. Creates a section if no sectionGuid is specified or if the specified GUID is not found. Otherwise uses the found section.
    /// </summary>
    /// <param name="sectionType">The identifier argument in RegisterSection attribute of the target instance. Mandatory if section is about to be created, otherwise ignored</param>
    /// <param name="sectionGuid">GUID of the section to find or create</param>
    IWidgetInSectionOptions InSection(string sectionType, Guid? sectionGuid = null);
}

public interface IWidgetInSectionOptions
{
    /// <summary>
    /// Uses GUID to specify the widget zone in which to create the widget. If a zone with specified GUID already exists, it is used, otherwise a zone with this GUID is created.
    /// </summary>
    /// <param name="zoneGuid">GUID of the zone to find or create</param>
    void InZone(Guid zoneGuid);

    /// <summary>
    /// Uses zone name to specify the widget zone in which to create the widget. If a zone with specified name already exists, it is used, otherwise a zone with this name (and random GUID) is created.
    /// </summary>
    /// <param name="zoneName">Name of the zone to find or create</param>
    void InZone(string zoneName);

    /// <summary>
    /// Specifies that the widget should be created in first widget zone of the section. If no zone is found, one will be created.
    /// </summary>
    void InFirstZone();
}
