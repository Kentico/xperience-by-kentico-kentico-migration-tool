using System.Text.Json.Serialization;

namespace Migration.Tool.Common;

public class CommerceConfiguration
{
    [JsonPropertyName(ConfigurationNames.OrderStatuses)]
    public Dictionary<string, string[]> OrderStatuses { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    [JsonPropertyName(ConfigurationNames.OrderFromDate)]
    public DateTime? OrderFromDate { get; set; }

    [JsonPropertyName(ConfigurationNames.OrderToDate)]
    public DateTime? OrderToDate { get; set; }

    [JsonPropertyName(ConfigurationNames.CommerceSiteName)]
    public string? CommerceSiteName { get; set; }
}

