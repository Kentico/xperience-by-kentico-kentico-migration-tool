
namespace Migration.Tool.Common.Logging;
public class TieredDescription
{
    public required string PlainText { get; set; }
    public string? HTML { get; set; }

    public TieredDescription FillValues(Dictionary<string, object>? values)
        => new()
        {
            PlainText = FillValues(PlainText, values),
            HTML = HTML is not null ? FillValues(HTML, values) : null
        };

    private static string FillValues(string input, Dictionary<string, object>? values)
        => values is not null
            ? values.Aggregate(input, (string agg, KeyValuePair<string, object> namedValue) => agg.Replace($"{{{{{namedValue.Key}}}}}", namedValue.Value.ToString()))
            : input;
}
