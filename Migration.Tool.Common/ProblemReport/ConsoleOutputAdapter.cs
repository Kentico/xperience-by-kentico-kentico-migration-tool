using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migration.Tool.Common.Logging;
public class ConsoleOutputAdapter : IProblemReportOutputAdapter
{
    public void Push(ProblemDefinition problem)
    {
        Console.WriteLine(problem.Cause.PlainText);
        Console.WriteLine("Suggestions:");
        foreach (var suggestion in problem.Suggestions)
        {
            Console.WriteLine($"  - {suggestion.PlainText}");
        }
    }
}
