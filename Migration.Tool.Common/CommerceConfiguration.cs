using System.Text.Json.Serialization;

namespace Migration.Tool.Common;

/// <summary>
/// Configuration for commerce data migration.
/// </summary>
public class CommerceConfiguration
{
    [JsonPropertyName(ConfigurationNames.CommerceSiteNames)]
    public List<string>? CommerceSiteNames { get; set; }

    [JsonPropertyName(ConfigurationNames.IncludeCustomerSystemFields)]
    public List<string>? IncludeCustomerSystemFields { get; set; }

    [JsonPropertyName(ConfigurationNames.IncludeAddressSystemFields)]
    public List<string>? IncludeAddressSystemFields { get; set; }

    [JsonPropertyName(ConfigurationNames.SystemFieldPrefix)]
    public string? SystemFieldPrefix { get; set; }

    [JsonPropertyName(ConfigurationNames.IncludeOrderSystemFields)]
    public List<string>? IncludeOrderSystemFields { get; set; }

    [JsonPropertyName(ConfigurationNames.IncludeOrderItemsSystemFields)]
    public List<string>? IncludeOrderItemsSystemFields { get; set; }

    [JsonPropertyName(ConfigurationNames.IncludeOrderAddressSystemFields)]
    public List<string>? IncludeOrderAddressSystemFields { get; set; }

    [JsonPropertyName(ConfigurationNames.OrderStatuses)]
    public Dictionary<string, string[]> OrderStatuses { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    [JsonPropertyName(ConfigurationNames.KX13OrderFilter)]
    public CommerceOrderFilterConfiguration? KX13OrderFilter { get; set; }
}