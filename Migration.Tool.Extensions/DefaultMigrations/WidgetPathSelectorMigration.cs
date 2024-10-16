using Microsoft.Extensions.Logging;
using Migration.Tool.Common.Enumerations;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source.Services.Model;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Extensions.DefaultMigrations;

public class WidgetPathSelectorMigration(ILogger<WidgetPathSelectorMigration> logger) : IWidgetPropertyMigration
{
    private const string MigratedComponent = Kx13FormComponents.Kentico_PathSelector;

    public int Rank => 100_001;
    public bool ShallMigrate(WidgetPropertyMigrationContext context, string propertyName)
        => MigratedComponent.Equals(context.EditingFormControlModel?.FormComponentIdentifier, StringComparison.InvariantCultureIgnoreCase);

    public Task<WidgetPropertyMigrationResult> MigrateWidgetProperty(string key, JToken? value, WidgetPropertyMigrationContext context)
    {
        if (value?.ToObject<List<PathSelectorItem>>() is { Count: > 0 } items)
        {
            var result = items.Select(x => new Kentico.Components.Web.Mvc.FormComponents.PathSelectorItem { TreePath = x.NodeAliasPath }).ToList();
            var resultAsJToken = JToken.FromObject(result);
            return Task.FromResult(new WidgetPropertyMigrationResult(resultAsJToken));
        }
        else
        {
            logger.LogError("Failed to parse '{ComponentName}' json {Json}", MigratedComponent, value?.ToString() ?? "<null>");

            // leave value as it is
            return Task.FromResult(new WidgetPropertyMigrationResult(value));
        }
    }
}
