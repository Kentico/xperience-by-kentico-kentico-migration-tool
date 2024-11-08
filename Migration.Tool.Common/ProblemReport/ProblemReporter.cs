using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migration.Tool.Common.Logging;
public class ProblemReporter<T>(IProblemDatabase problemDatabase, IEnumerable<IProblemReportOutputAdapter> outputAdapters) : IProblemReporter<T>
{
    public void Report(string id, Dictionary<string, object>? parameters = null)
    {
        var problem = problemDatabase.Get(id).FillValues(parameters);
        foreach (var adapter in outputAdapters)
        {
            adapter.Push(problem);
        }
    }
}
