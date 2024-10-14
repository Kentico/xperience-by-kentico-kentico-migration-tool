using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Migration.Tool.Common.Helpers;

namespace Migration.Tool.Common;

public class EntityConfigurations : Dictionary<string, EntityConfiguration>
{
    public EntityConfigurations() : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    public EntityConfiguration GetEntityConfiguration(string? tableName)
    {
        if (TryGetValue(tableName ?? "*", out var result))
        {
            return result;
        }

        return TryGetValue("*", out var @default) ? @default : new EntityConfiguration();
    }

    public EntityConfiguration GetEntityConfiguration<TModel>()
    {
        string? tableName = ReflectionHelper<TModel>.GetFirstAttributeOrNull<TableAttribute>()?.Name;
        return GetEntityConfiguration(tableName);
    }

    public void SetEntityConfiguration<TModel>(EntityConfiguration config)
    {
        string? tableName = ReflectionHelper<TModel>.GetFirstAttributeOrNull<TableAttribute>()?.Name;
        if (ContainsKey(tableName ?? throw new InvalidOperationException("Table name not found for entity, possibly class is not decorated with [TableAttribute]?")))
        {
            this[tableName] = config;
        }
        else
        {
            Add(tableName, config);
        }
    }
}

public class EntityConfiguration
{
    [JsonPropertyName(ConfigurationNames.ExcludeCodeNames)]
    public string[] ExcludeCodeNames { get; set; } = Array.Empty<string>();

    [JsonPropertyName(ConfigurationNames.ExplicitPrimaryKeyMapping)]
    public Dictionary<string, Dictionary<string, int?>> ExplicitPrimaryKeyMapping { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
