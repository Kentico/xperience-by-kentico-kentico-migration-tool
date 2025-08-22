using Migration.Tool.Common.Services.Ipc;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.KXP.Api.Services.CmsClass;

public record WidgetPropertyMigrationContext(int SiteId, EditingFormControlModel? EditingFormControlModel);
public record WidgetPropertyMigrationResult(JToken? Value, bool NeedsDeferredPatch = false, bool AllowDefaultMigrations = true);

public interface IWidgetPropertyMigration : ICustomMigration
{
    bool ShallMigrate(WidgetPropertyMigrationContext context, string propertyName);
    Task<WidgetPropertyMigrationResult> MigrateWidgetProperty(string key, JToken? value, WidgetPropertyMigrationContext context);
}
