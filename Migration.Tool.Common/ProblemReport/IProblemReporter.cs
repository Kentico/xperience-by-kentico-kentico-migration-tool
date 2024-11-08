using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migration.Tool.Common.Logging;
public interface IProblemReporter<T>
{
    void Report(string id, Dictionary<string, object>? parameters = null);
}
