using Newtonsoft.Json;

namespace Migration.Tool.Source.Services.Model;

/// <summary>Represents an item for the attachment selector.</summary>
public class AttachmentSelectorItem
{
    /// <summary>Attachment GUID.</summary>
    [JsonProperty("fileGuid")]
    public Guid FileGuid { get; set; }
}
