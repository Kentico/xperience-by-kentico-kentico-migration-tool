using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
internal class ConvertToWidgetDirective(string widgetType, Guid? widgetGuid, Guid? widgetVariantGuid) : ContentItemDirectiveBase, IWidgetLocationOptions, IWidgetPropertiesOptions, IWidgetOnPageOptions, IConvertToWidgetOptions, IWidgetInEditableAreaOptions, IWidgetInSectionOptions
{
    internal string WidgetType { get; } = widgetType;
    internal Guid? WidgetGuid { get; } = widgetGuid;
    internal Guid? WidgetVariantGuid { get; } = widgetVariantGuid;

    internal string? EditableAreaIdentifier { get; set; }
    public IWidgetInEditableAreaOptions InEditableArea(string areaIdentifier)
    {
        EditableAreaIdentifier = areaIdentifier;
        return this;
    }

    internal string? SectionType { get; private set; }
    internal Guid? SectionGuid { get; private set; }
    public IWidgetInSectionOptions InSection(string sectionType, Guid? sectionGuid = null)
    {
        SectionType = sectionType;
        SectionGuid = sectionGuid;
        return this;
    }

    internal bool ZoneFirst { get; private set; }
    internal Guid ZoneGuid { get; private set; }
    internal string? ZoneName { get; private set; }

    public IWidgetLocationOptions Location => this;

    public IWidgetPropertiesOptions Properties => this;

    public void InFirstZone() => ZoneFirst = true;
    public void InZone(Guid zoneGuid) => ZoneGuid = zoneGuid;
    public void InZone(string zoneName) => ZoneName = zoneName;

    internal int ParentLevel { get; private set; }
    public IWidgetOnPageOptions OnAncestorPage(int parentLevel = -1)
    {
        ParentLevel = parentLevel;
        return this;
    }

    internal bool WrapInReusableItem { get; private set; }
    internal MapWidgetPropertiesDelegate? ItemToWidgetPropertiesMapping { get; private set; }
    public void Fill(bool wrapInReusableItem, MapWidgetPropertiesDelegate itemToWidgetPropertiesMapping)
    {
        WrapInReusableItem = wrapInReusableItem;
        ItemToWidgetPropertiesMapping = itemToWidgetPropertiesMapping;
    }
}

public delegate JObject MapWidgetPropertiesDelegate(Dictionary<string, object?> itemProperties, Guid? reusableItemGuid, ICollection<Guid> childItems);
