namespace Migration.Toolkit.Core.Configuration;

public class EntityConfigurations : Dictionary<string, EntityConfiguration>
{
    public EntityConfiguration GetEntityConfiguration(string? tableName)
    {
        if (this.TryGetValue(tableName ?? "*", out var result))
        {
            return result;
        }

        return this.TryGetValue("*", out var @default) ? @default : new EntityConfiguration(false);
    }
}
public record EntityConfiguration(bool DeleteTargetIfSourceNotExists);