using Migration.Tool.Common.Builders;

namespace Migration.Tool.Source.Providers;

public class ClassMappingProvider(IEnumerable<IClassMapping> classMappings)
{
    private readonly Dictionary<string, IClassMapping> mappingsByClassName = classMappings.Aggregate(new Dictionary<string, IClassMapping>(StringComparer.InvariantCultureIgnoreCase),
        (current, sourceClassMapping) =>
        {
            foreach (string s2Cl in sourceClassMapping.SourceClassNames)
            {
                if (!current.TryAdd(s2Cl, sourceClassMapping))
                {
                    throw new InvalidOperationException($"Incorrectly defined class mapping - duplicate found for class '{s2Cl}'. Fix mapping before proceeding with migration.");
                }
            }

            return current;
        });

    public IClassMapping? GetMapping(string className) => mappingsByClassName.GetValueOrDefault(className);
}
