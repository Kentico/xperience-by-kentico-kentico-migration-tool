using Newtonsoft.Json;

namespace Migration.Tool.Source.Services.Model;

/// <summary>Represents an item for a page selector.</summary>
public class PageSelectorItem
{
    /// <summary>Node Guid of a page.</summary>
    [JsonProperty("nodeGuid")]
    public Guid NodeGuid { get; set; }
}
