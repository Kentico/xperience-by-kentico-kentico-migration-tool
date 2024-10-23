using Migration.Tool.Common.Services.Ipc;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.KXP.Api.Services.CmsClass;

public record WidgetPropertyMigrationContext(int SiteId, EditingFormControlModel? EditingFormControlModel);
public record WidgetPropertyMigrationResult(JToken? Value, bool NeedsDeferredPatch = false, bool AllowDefaultMigrations = true);

public interface IWidgetPropertyMigration
{
    /// <summary>
    /// custom migrations are sorted by this number, first encountered migration wins. Values higher than 100 000 are set to default migrations, set number bellow 100 000 for custom migrations
    /// </summary>
    int Rank { get; }

    bool ShallMigrate(WidgetPropertyMigrationContext context, string propertyName);
    Task<WidgetPropertyMigrationResult> MigrateWidgetProperty(string key, JToken? value, WidgetPropertyMigrationContext context);
}
