using Newtonsoft.Json.Linq;

namespace Migration.Tool.KXP.Api.Services.CmsClass;

public record WidgetIdentifier(string TypeIdentifier, Guid InstanceIdentifier);
public record WidgetMigrationContext(int SiteId);
public record WidgetMigrationResult(JToken? Value, IReadOnlyDictionary<string, Type> PropertyMigrations, bool NeedsDeferredPatch = false);

public interface IWidgetMigration : ICustomMigration
{
    bool ShallMigrate(WidgetMigrationContext context, WidgetIdentifier identifier);
    Task<WidgetMigrationResult> MigrateWidget(WidgetIdentifier identifier, JToken? value, WidgetMigrationContext context);
}
