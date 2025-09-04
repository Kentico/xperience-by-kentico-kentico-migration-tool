using Newtonsoft.Json;

namespace Migration.Tool.Source.Services.Model;

//
// Summary:
//     Represents an item for a path selector.
public class PathSelectorItem
{
    //
    // Summary:
    //     Node Alias Path of a page.
    [JsonProperty("nodeAliasPath")]
    public string? NodeAliasPath { get; set; }
}
