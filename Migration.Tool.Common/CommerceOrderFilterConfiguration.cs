using System.Text.Json.Serialization;

namespace Migration.Tool.Common;
/// <summary>
/// Configuration for filtering orders from KX13.
/// </summary>
public class CommerceOrderFilterConfiguration
{
    [JsonPropertyName(ConfigurationNames.OrdersFromDate)]
    public DateTime? OrdersFromDate { get; set; }

    [JsonPropertyName(ConfigurationNames.OrdersToDate)]
    public DateTime? OrdersToDate { get; set; }

    [JsonPropertyName(ConfigurationNames.OrdersStatusCodeNames)]
    public List<string> OrdersStatusCodeNames { get; set; } = [];
}
