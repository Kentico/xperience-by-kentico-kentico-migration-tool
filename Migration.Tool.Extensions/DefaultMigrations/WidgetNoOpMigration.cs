using Migration.Tool.KXP.Api.Services.CmsClass;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Extensions.DefaultMigrations;

public class WidgetNoOpMigration : IWidgetPropertyMigration
{
    public int Rank => 1_000_000;

    public bool ShallMigrate(WidgetPropertyMigrationContext context, string propertyName) => false; // used only when explicitly stated in custom widget migration, ShallMigrate isn't used

    public Task<WidgetPropertyMigrationResult> MigrateWidgetProperty(string key, JToken? value, WidgetPropertyMigrationContext context) => Task.FromResult(new WidgetPropertyMigrationResult(value));
}
