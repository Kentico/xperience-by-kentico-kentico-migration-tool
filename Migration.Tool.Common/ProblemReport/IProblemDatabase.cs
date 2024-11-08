using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migration.Tool.Common.Logging;
public interface IProblemDatabase
{
    ProblemDefinition Get(string problemID);
}
