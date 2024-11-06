using System;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Migration.Tool.KXP.Api.Services.CmsClass;

public class WidgetMigrationService
{
    private readonly List<IWidgetPropertyMigration> widgetPropertyMigrations;
    private readonly List<IWidgetMigration> widgetMigrations;

    public WidgetMigrationService(IServiceProvider serviceProvider)
    {
        widgetPropertyMigrations = LoadRegisteredMigrations<IWidgetPropertyMigration>(serviceProvider);
        widgetMigrations = LoadRegisteredMigrations<IWidgetMigration>(serviceProvider);
    }

    private List<T> LoadRegisteredMigrations<T>(IServiceProvider serviceProvider) where T : ICustomMigration
    {
        var registeredMigrations = serviceProvider.GetService<IEnumerable<T>>();
        return registeredMigrations == null
            ? []
            : registeredMigrations.OrderBy(wpm => wpm.Rank).ToList();
    }

    public IWidgetPropertyMigration? GetWidgetPropertyMigration(WidgetPropertyMigrationContext context, string key)
        => widgetPropertyMigrations.FirstOrDefault(wpm => wpm.ShallMigrate(context, key));

    public IWidgetPropertyMigration ResolveWidgetPropertyMigration(Type type)
        => widgetPropertyMigrations.FirstOrDefault(x => x.GetType() == type) ?? throw new ArgumentException($"No migration of type {type} registered", nameof(type));

    public IWidgetMigration? GetWidgetMigration(WidgetMigrationContext context, WidgetIdentifier identifier)
        => widgetMigrations.FirstOrDefault(wpm => wpm.ShallMigrate(context, identifier));
}
