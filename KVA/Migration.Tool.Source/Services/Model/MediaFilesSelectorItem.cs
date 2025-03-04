using Newtonsoft.Json;

namespace Migration.Tool.Source.Services.Model;

//
// Summary:
//     Represents item for media files selector.
public class MediaFilesSelectorItem
{
    //
    // Summary:
    //     Media file GUID.
    [JsonProperty("fileGuid")]
    public Guid FileGuid { get; set; }
}
