using System.Text.Json.Serialization;

namespace Migration.Tool.Common;

public class CommerceConfiguration
{
    [JsonPropertyName(ConfigurationNames.CommerceSiteNames)]
    public List<string>? CommerceSiteNames { get; set; }

    [JsonPropertyName(ConfigurationNames.IncludeCustomerSystemFields)]
    public string? IncludeCustomerSystemFields { get; set; }

    [JsonPropertyName(ConfigurationNames.IncludeAddressSystemFields)]
    public string? IncludeAddressSystemFields { get; set; }

    [JsonPropertyName(ConfigurationNames.SystemFieldPrefix)]
    public string? SystemFieldPrefix { get; set; }
}
