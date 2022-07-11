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
    [JsonPropertyName("SourceConnectionString")]
    public string? SourceConnectionString { get; set; }
    
    [JsonPropertyName("SourceCmsDirPath")]
    public string? SourceCmsDirPath { get; set; }
    
    [JsonPropertyName("TargetConnectionString")]
    public string? TargetConnectionString { get; set; }
    
    [JsonPropertyName("TargetCmsDirPath")]
    public string? TargetCmsDirPath { get; set; }
    
    [JsonPropertyName("EntityConfigurations")]
    public EntityConfigurations? EntityConfigurations { get; set; }
    
    [JsonPropertyName("TargetAttachmentMediaLibraryName")]
    public string? TargetAttachmentMediaLibraryName { get; set; }
    
    [JsonPropertyName("MigrateOnlyMediaFileInfo")]
    public bool? MigrateOnlyMediaFileInfo { get; set; } = true;

    [JsonPropertyName("UseOmActivityNodeRelationAutofix")]
    public AutofixEnum? UseOmActivityNodeRelationAutofix { get; set; } = AutofixEnum.Error;
    
    [JsonPropertyName("UseOmActivitySiteRelationAutofix")]
    public AutofixEnum? UseOmActivitySiteRelationAutofix { get; set; } = AutofixEnum.Error;

    public Dictionary<int?, int?> RequireExplicitMapping<TEntityType>(Expression<Func<TEntityType, object>> keyNameSelector)
    {
        var memberName = keyNameSelector.GetMemberName();
        var migratedSiteIds = EntityConfigurations?.GetEntityConfiguration<TEntityType>()?.ExplicitPrimaryKeyMapping[memberName];
        if (migratedSiteIds == null)
        {
            throw new InvalidOperationException($"{typeof(TEntityType).Name} ExplicitPrimaryKeyMapping of {memberName} is required.");
        }

        return migratedSiteIds.ToDictionary(kvp => (int?)int.Parse(kvp.Key), kvp => kvp.Value);
    }

    public void AddExplicitMapping<TEntityType>(Expression<Func<TEntityType, object>> keyNameSelector, int sourceId, int targetId)
    {
        var memberName = keyNameSelector.GetMemberName();
        if (EntityConfigurations == null) throw new InvalidOperationException();
        var entityConfiguration = EntityConfigurations.GetEntityConfiguration<TEntityType>();
        var explicitPrimaryKeyMapping = entityConfiguration.ExplicitPrimaryKeyMapping;
        if (!explicitPrimaryKeyMapping.ContainsKey(memberName))
        {
            explicitPrimaryKeyMapping.Add(memberName, new());
        }

        if (!explicitPrimaryKeyMapping[memberName].ContainsKey(sourceId.ToString()))
        {
            explicitPrimaryKeyMapping[memberName].Add(sourceId.ToString(), targetId);
        }
        else
        {
            explicitPrimaryKeyMapping[memberName][sourceId.ToString()] = targetId;
        }

        EntityConfigurations.SetEntityConfiguration<TEntityType>(entityConfiguration);
    }
}