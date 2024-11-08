using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migration.Tool.Common.Logging;
public class ProblemDefinition
{
    public required string ID { get; set; }
    public required TieredDescription Cause { get; set; }
    public IEnumerable<TieredDescription> Suggestions { get; set; } = [];

    public ProblemDefinition FillValues(Dictionary<string, object>? values)
        => new()
        {
            ID = ID,
            Cause = Cause.FillValues(values),
            Suggestions = Suggestions.Select(x => x.FillValues(values))
        };
}
