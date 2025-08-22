using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using CMS.DataEngine;
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
        tableName ??= ReflectionHelper<TModel>.GetStaticPropertyOrNull("TableName") as string;

        if (tableName is null)
        {
            // If table is not specified by annotation at the type, try deriving the table name from OBJECT_TYPE static field (works for ProviderInfos)
            object? objectType = ReflectionHelper<TModel>.GetStaticFieldOrNull(nameof(DataClassInfo.OBJECT_TYPE));
            if (objectType is not null)
            {
                tableName = ((string)objectType).Replace('.', '_');
            }
        }
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
