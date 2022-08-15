using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Migration.Toolkit.Common;

/// <summary>
/// Autofix enum 
/// </summary>
/// <remarks>do not update value names, they are used in json configuration</remarks>
public enum AutofixEnum
{
    DiscardData,
    AttemptFix,
    Error,
}

public class ToolkitConfiguration
{
    [JsonPropertyName(ConfigurationNames.SourceConnectionString)]
    public string SourceConnectionString { get; set; } = null!;

    [JsonPropertyName(ConfigurationNames.SourceCmsDirPath)]
    public string? SourceCmsDirPath { get; set; }
    
    [JsonPropertyName(ConfigurationNames.TargetConnectionString)]
    public string TargetConnectionString { get; set; } = null!;
    
    [JsonPropertyName(ConfigurationNames.TargetCmsDirPath)]
    public string TargetCmsDirPath { get; set; } = null!;

    [JsonPropertyName(ConfigurationNames.EntityConfigurations)]
    public EntityConfigurations EntityConfigurations { get; set; } = new();
    
    [JsonPropertyName(ConfigurationNames.MigrateOnlyMediaFileInfo)]
    public bool? MigrateOnlyMediaFileInfo { get; set; } = true;

    [JsonPropertyName(ConfigurationNames.UseOmActivityNodeRelationAutofix)]
    public AutofixEnum? UseOmActivityNodeRelationAutofix { get; set; } = AutofixEnum.Error;
    
    [JsonPropertyName(ConfigurationNames.UseOmActivitySiteRelationAutofix)]
    public AutofixEnum? UseOmActivitySiteRelationAutofix { get; set; } = AutofixEnum.Error;
    
    [JsonPropertyName(ConfigurationNames.MigrationProtocolPath)]
    public string? MigrationProtocolPath { get; set; }

    public Dictionary<int, int> RequireExplicitMapping<TEntityType>(Expression<Func<TEntityType, object>> keyNameSelector)
    {
        var memberName = keyNameSelector.GetMemberName();
        var migratedIds = EntityConfigurations?.GetEntityConfiguration<TEntityType>()?.ExplicitPrimaryKeyMapping[memberName];
        if (migratedIds == null)
        {
            throw new InvalidOperationException(string.Format(Resources.Exception_MappingIsRequired, typeof(TEntityType).Name, memberName));
        }

        return migratedIds.ToDictionary(kvp =>
        {
            if (int.TryParse(kvp.Key, out var id))
            {
                return id;    
            }

            throw new InvalidOperationException(string.Format(Resources.Exception_MappingIsRequired, typeof(TEntityType).Name, memberName));
        }, kvp =>
        {
            if (kvp.Value is { } id)
            {
                return id;
            }
            
            throw new InvalidOperationException(string.Format(Resources.Exception_MappingIsRequired, typeof(TEntityType).Name, memberName));
        });
    }

    public void AddExplicitMapping<TEntityType>(Expression<Func<TEntityType, object>> keyNameSelector, int sourceId, int targetId)
    {
        var memberName = keyNameSelector.GetMemberName();
        EntityConfigurations ??= new EntityConfigurations();
        
        var entityConfiguration = EntityConfigurations.GetEntityConfiguration<TEntityType>();
        var mapping = entityConfiguration.ExplicitPrimaryKeyMapping;
        if (!mapping.ContainsKey(memberName))
        {
            mapping.Add(memberName, new());
        }

        if (!mapping[memberName].ContainsKey(sourceId.ToString()))
        {
            mapping[memberName].Add(sourceId.ToString(), targetId);
        }
        else
        {
            mapping[memberName][sourceId.ToString()] = targetId;
        }

        EntityConfigurations.SetEntityConfiguration<TEntityType>(entityConfiguration);
    }
}