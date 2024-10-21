using Microsoft.Extensions.DependencyInjection;

namespace Migration.Tool.KXP.Api.Services.CmsClass;

public class WidgetMigrationService
{
    private readonly List<IWidgetPropertyMigration> widgetPropertyMigrations;

    public WidgetMigrationService(IServiceProvider serviceProvider)
    {
        var migrations = serviceProvider.GetService<IEnumerable<IWidgetPropertyMigration>>();
        widgetPropertyMigrations = migrations == null
            ? []
            : migrations.OrderBy(wpm => wpm.Rank).ToList();
    }

    public IWidgetPropertyMigration? GetWidgetPropertyMigrations(WidgetPropertyMigrationContext context, string key)
        => widgetPropertyMigrations.FirstOrDefault(wpm => wpm.ShallMigrate(context, key));
}
