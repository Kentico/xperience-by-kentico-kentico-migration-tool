using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Common.Logging;
public class ProblemDatabase : IProblemDatabase
{
    public ProblemDatabase() => LoadDefinitions();

    private Dictionary<string, ProblemDefinition> db;

    private void LoadDefinitions()
    {
        db = [];

        foreach (string filePath in Directory.GetFiles(Path.Combine(Assembly.GetEntryAssembly().Location, "ProblemDefinitions"), "*.json"))
        {
            var fileDict = JObject.Parse(File.ReadAllText(filePath));
            foreach (var prop in fileDict.Properties())
            {
                if (db.ContainsKey(prop.Name))
                {
                    throw new Exception($"Multiple definitions for problem {prop.Name}");
                }
                var def = fileDict[prop.Name]?.ToObject<ProblemDefinition>();
                if (def is not null)
                {
                    def.ID = prop.Name;
                }
                db[prop.Name] = def is not null ? def : throw new Exception($"Invalid definition of problem {prop.Name} in file {filePath}");
            }
        }
    }

    public ProblemDefinition? Get(string problemID) => db.GetValueOrDefault(problemID);
}
