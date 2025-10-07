using CMS.ContentEngine;
using CMS.Core;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common.Enumerations;
using Migration.Tool.Common.Services;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source.Services.Model;
using Newtonsoft.Json.Linq;

public class WidgetPageSelectorToCombinedSelectorMigration(
    ISpoiledGuidContext spoiledGuidContext,
    ILogger<WidgetPageSelectorToCombinedSelectorMigration> logger) : IWidgetPropertyMigration
{
    private const string MigratedComponent = Kx13FormComponents.Kentico_PageSelector;

    public int Rank => 100_002;
    public bool ShallMigrate(WidgetPropertyMigrationContext context, string propertyName)
        => MigratedComponent.Equals(context.EditingFormControlModel?.FormComponentIdentifier, StringComparison.InvariantCultureIgnoreCase);

    public Task<WidgetPropertyMigrationResult> MigrateWidgetProperty(
        string key, JToken? value, WidgetPropertyMigrationContext context)
    {
        (int siteId, _) = context;

        var refsToMedia = new List<object>();
        if (value?.ToObject<List<PageSelectorItem>>() is { Count: > 0 } items)
        {
            var result = items.Select(pageSelectorItem => new ContentItemReference
            {
                Identifier = spoiledGuidContext.EnsureNodeGuid(pageSelectorItem.NodeGuid, siteId)
            }).ToList();
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