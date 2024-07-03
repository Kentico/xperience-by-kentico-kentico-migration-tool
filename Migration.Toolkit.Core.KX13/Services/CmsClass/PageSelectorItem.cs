namespace Migration.Toolkit.Core.KX13.Services.CmsClass;

using Newtonsoft.Json;

/// <summary>Represents an item for a page selector.</summary>
public class PageSelectorItem
{
    /// <summary>Node Guid of a page.</summary>
    [JsonProperty("nodeGuid")]
#error "NodeGuid may not be unique, use other means of searching for node!"
    public Guid NodeGuid { get; set; }
}