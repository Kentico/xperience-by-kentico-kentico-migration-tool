using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Common.Model;

#region Copied from Kentico assembly

[DataContract(Name = "Configuration", Namespace = "")]
public sealed class EditableAreasConfiguration
{
    /// <summary>
    ///     Creates an instance of <see cref="T:Kentico.PageBuilder.Web.Mvc.EditableAreasConfiguration" /> class.
    /// </summary>
    public EditableAreasConfiguration() => EditableAreas = [];

    /// <summary>Editable areas within the page.</summary>
    [DataMember]
    [JsonProperty("editableAreas")]
    public List<EditableAreaConfiguration> EditableAreas { get; private set; }
}

/// <summary>
///     Represents configuration of editable area within the <see cref="T:CMS.DocumentEngine.TreeNode" /> instance.
/// </summary>
[DataContract(Name = "EditableArea", Namespace = "")]
public sealed class EditableAreaConfiguration
{
    /// <summary>
    ///     Creates an instance of <see cref="T:Kentico.PageBuilder.Web.Mvc.EditableAreasConfiguration" /> class.
    /// </summary>
    public EditableAreaConfiguration() => Sections = [];

    /// <summary>Identifier of the editable area.</summary>
    [DataMember]
    [JsonProperty("identifier")]
    public string Identifier { get; set; } = null!;

    /// <summary>Sections within editable area.</summary>
    [DataMember]
    [JsonProperty("sections")]
    public List<SectionConfiguration> Sections { get; private set; }

    /// <summary>
    ///     A flag indicating whether the output of the individual widgets within the editable area can be cached. The default
    ///     value is <c>false</c>.
    /// </summary>
    public bool AllowWidgetOutputCache { get; set; }

    /// <summary>
    ///     An absolute expiration date for the cached output of the individual widgets.
    /// </summary>
    public DateTimeOffset? WidgetOutputCacheExpiresOn { get; set; }

    /// <summary>
    ///     The length of time from the first request to cache the output of the individual widgets.
    /// </summary>
    public TimeSpan? WidgetOutputCacheExpiresAfter { get; set; }

    /// <summary>
    ///     The time after which the cached output of the individual widgets should be evicted if it has not been accessed.
    /// </summary>
    public TimeSpan? WidgetOutputCacheExpiresSliding { get; set; }
}

/// <summary>
///     Represents configuration of section within the
///     <see cref="T:Kentico.PageBuilder.Web.Mvc.EditableAreaConfiguration" /> instance.
/// </summary>
[DataContract(Name = "Section", Namespace = "")]
public sealed class SectionConfiguration
{
    /// <summary>
    ///     Creates an instance of <see cref="T:Kentico.PageBuilder.Web.Mvc.EditableAreasConfiguration" /> class.
    /// </summary>
    public SectionConfiguration() => Zones = [];

    /// <summary>Identifier of the section.</summary>
    [DataMember]
    [JsonProperty("identifier")]
    public Guid Identifier { get; set; }

    /// <summary>Type section identifier.</summary>
    [DataMember]
    [JsonProperty("type")]
    public string TypeIdentifier { get; set; } = null!;

    /// <summary>Section properties.</summary>
    [DataMember]
    [JsonProperty("properties")]
    // public ISectionProperties Properties { get; set; }
    public JObject? Properties { get; set; }

    /// <summary>Zones within the section.</summary>
    [DataMember]
    [JsonProperty("zones")]
    public List<ZoneConfiguration> Zones { get; private set; }
}

/// <summary>
///     Represents the zone within the <see cref="T:Kentico.PageBuilder.Web.Mvc.EditableAreasConfiguration" />
///     configuration class.
/// </summary>
[DataContract(Name = "Zone", Namespace = "")]
public sealed class ZoneConfiguration
{
    /// <summary>
    ///     Creates an instance of <see cref="T:Kentico.PageBuilder.Web.Mvc.ZoneConfiguration" /> class.
    /// </summary>
    public ZoneConfiguration() => Widgets = [];

    /// <summary>Identifier of the widget zone.</summary>
    [DataMember]
    [JsonProperty("identifier")]
    public Guid Identifier { get; set; }

    /// <summary>Name of the widget zone.</summary>
    [DataMember]
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    /// <summary>List of widgets within the zone.</summary>
    [DataMember]
    [JsonProperty("widgets")]
    public List<WidgetConfiguration> Widgets { get; private set; }
}

/// <summary>
///     Represents the configuration of a widget within the
///     <see cref="P:Kentico.PageBuilder.Web.Mvc.ZoneConfiguration.Widgets" /> list.
/// </summary>
[DataContract(Name = "Widget", Namespace = "")]
public sealed class WidgetConfiguration
{
    /// <summary>
    ///     Creates an instance of <see cref="T:Kentico.PageBuilder.Web.Mvc.WidgetConfiguration" /> class.
    /// </summary>
    public WidgetConfiguration() => Variants = [];

    /// <summary>Identifier of the widget instance.</summary>
    [DataMember]
    [JsonProperty("identifier")]
    public Guid Identifier { get; set; }

    /// <summary>Type widget identifier.</summary>
    [DataMember]
    [JsonProperty("type")]
    public string TypeIdentifier { get; set; } = null!;

    /// <summary>Personalization condition type identifier.</summary>
    [DataMember]
    [JsonProperty("conditionType")]
    public string PersonalizationConditionTypeIdentifier { get; set; } = null!;

    /// <summary>List of widget variants.</summary>
    [DataMember]
    [JsonProperty("variants")]
    public List<WidgetVariantConfiguration?> Variants { get; set; }
}

/// <summary>
///     Represents the configuration variant of a widget within the
///     <see cref="P:Kentico.PageBuilder.Web.Mvc.WidgetConfiguration.Variants" /> list.
/// </summary>
[DataContract(Name = "Variant", Namespace = "")]
public sealed class WidgetVariantConfiguration
{
    /// <summary>Identifier of the variant instance.</summary>
    [DataMember]
    [JsonProperty("identifier")]
    public Guid Identifier { get; set; }

    /// <summary>Widget variant name.</summary>
    [DataMember]
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    /// <summary>Widget variant properties.</summary>
    [DataMember]
    [JsonProperty("properties")]
    // public IWidgetProperties Properties { get; set; }
    public JObject Properties { get; set; } = null!;

    /// <summary>Widget variant personalization condition type.</summary>
    /// <remarks>Only personalization condition type parameters are serialized to JSON.</remarks>
    [DataMember]
    [JsonProperty("conditionTypeParameters")]
    public JObject PersonalizationConditionType { get; set; } = null!;
}

#endregion
