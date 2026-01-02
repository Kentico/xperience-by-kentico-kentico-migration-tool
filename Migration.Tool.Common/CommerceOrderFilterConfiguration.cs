using System.Text.Json.Serialization;

namespace Migration.Tool.Common;
/// <summary>
/// Configuration for filtering orders from KX13.
/// </summary>
public class CommerceOrderFilterConfiguration
{
    [JsonPropertyName(ConfigurationNames.OrderFromDate)]
    public DateTime? OrderFromDate { get; set; }

    [JsonPropertyName(ConfigurationNames.OrderToDate)]
    public DateTime? OrderToDate { get; set; }

    [JsonPropertyName(ConfigurationNames.OrderStatusCodeNames)]
    public List<string> OrderStatusCodeNames { get; set; } = [];
}
