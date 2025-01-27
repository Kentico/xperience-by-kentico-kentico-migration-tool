using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
internal class ConvertToWidgetDirective(string WidgetType, Guid? WidgetGuid, Guid? WidgetVariantGuid) : ContentItemDirectiveBase, IWidgetLocationOptions, IWidgetPropertiesOptions, IWidgetOnPageOptions, IConvertToWidgetOptions, IWidgetInEditableAreaOptions, IWidgetInSectionOptions
{
    internal string WidgetType { get; } = WidgetType;
    internal Guid? WidgetGuid { get; } = WidgetGuid;
    internal Guid? WidgetVariantGuid { get; } = WidgetVariantGuid;

    internal string EditableAreaIdentifier { get; set; }
    public IWidgetInEditableAreaOptions InEditableArea(string areaIdentifier)
    {
        EditableAreaIdentifier = areaIdentifier;
        return this;
    }

    internal string SectionType { get; private set; }
    internal Guid? SectionGuid { get; private set; }
    public IWidgetInSectionOptions InSection(string sectionType, Guid? sectionGuid = null)
    {
        SectionType = sectionType;
        SectionGuid = sectionGuid;
        return this;
    }

    internal bool ZoneFirst { get; private set; }
    internal Guid ZoneGuid { get; private set; }
    internal string ZoneName { get; private set; }

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
    internal MapWidgetPropertiesDelegate ItemToWidgetPropertiesMapping { get; private set; }
    public void Fill(bool wrapInReusableItem, MapWidgetPropertiesDelegate itemToWidgetPropertiesMapping)
    {
        WrapInReusableItem = wrapInReusableItem;
        ItemToWidgetPropertiesMapping = itemToWidgetPropertiesMapping;
    }
}

public delegate JObject MapWidgetPropertiesDelegate(Dictionary<string, object?> itemProperties, Guid? reusableItemGuid, ICollection<Guid> childItems);
