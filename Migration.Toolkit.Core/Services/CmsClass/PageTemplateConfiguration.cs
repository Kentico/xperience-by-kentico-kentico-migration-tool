namespace Migration.Toolkit.Core.Services.CmsClass;

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Page template configuration for the <see cref="T:CMS.DocumentEngine.TreeNode" /> instance.
/// </summary>
[DataContract(Name = "PageTemplate", Namespace = "")]
public class PageTemplateConfiguration
{
    /// <summary>Identifier of the page template.</summary>
    [DataMember]
    [JsonProperty("identifier")]
    public string Identifier { get; set; }

    /// <summary>
    /// Identifier of the page template configuration based on which the page was created.
    /// </summary>
    [DataMember]
    [JsonProperty("configurationIdentifier")]
    public Guid ConfigurationIdentifier { get; set; }

    /// <summary>Page template properties.</summary>
    [DataMember]
    [JsonProperty("properties")]
    public JObject Properties { get; set; }
}