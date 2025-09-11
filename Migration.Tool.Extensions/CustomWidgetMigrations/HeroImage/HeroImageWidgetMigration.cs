using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using Microsoft.Extensions.Logging;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Extensions.CustomWidgetMigrations;

public class HeroImageWidgetMigration : IWidgetMigration
{
    public int Rank => 1;
    public const string SOURCE_WIDGET_IDENTIFIER = "DancingGoat.LandingPage.HeroImage";
    public const int SOURCE_PAGE_ID = 44; // Coffee samples page
    public Task<WidgetMigrationResult> MigrateWidget(WidgetIdentifier identifier, JToken? value, WidgetMigrationContext context)
    {
        //Recombine the properties
        var variants = (JArray)value!["variants"]!;
        var singleVariant = variants[0];
        singleVariant["properties"] = new JObject
        {
            ["hero"] = singleVariant["properties"],
            ["image"] = singleVariant["properties"]!["image"],
            ["theme"] = singleVariant["properties"]!["theme"]
        };

        // the property names must match the target widget props names
        // e.g., text - Text, buttonText - ButtonText, buttonTarget - ButtonTarget, theme - Theme
        // otherwise the value will not map correctly

        //For new properties, we must explicitly define property migration classes
        var propertyMigrations = new Dictionary<string, Type>
        {
            ["hero"] = typeof(WidgetDataToHeroMigration), // newly created Hero content item will be referenced by this property
        };

        return Task.FromResult(new WidgetMigrationResult(value, propertyMigrations));
    }

    public bool ShallMigrate(WidgetMigrationContext context, WidgetIdentifier identifier) =>
        string.Equals(SOURCE_WIDGET_IDENTIFIER, identifier.TypeIdentifier, StringComparison.InvariantCultureIgnoreCase);
    // && SOURCE_PAGE_ID == context.SiteId;
}


public class WidgetDataToHeroMigration(ILogger<WidgetDataToHeroMigration> logger) : IWidgetPropertyMigration
{
    public const string HERO_CONTENT_TYPE_NAME = "DancingGoatCore.Hero";

    public int Rank => 100_000;

    public bool ShallMigrate(WidgetPropertyMigrationContext context, string propertyName)
        => string.Equals("hero", propertyName, StringComparison.InvariantCultureIgnoreCase);

    public async Task<WidgetPropertyMigrationResult> MigrateWidgetProperty(string key, JToken? value, WidgetPropertyMigrationContext context)
    {
        if (value != null)
        {
            logger.LogInformation("Migrating 'hero' property with value: {Json}", value?.ToString() ?? "<null>");

            //need to manually create the desired content type in the target instance running this migration
            var heroContentItem = await CreateHeroContentItem(value!);

            logger.LogInformation("Created Hero content item with GUID: {Guid}", heroContentItem?.Identifier.ToString() ?? "<null>");

            var widgetFieldValue = heroContentItem != null
                ? [heroContentItem]
                : Array.Empty<object>();

            logger.LogInformation("Widget property 'hero' migrated to reference new content item: {Json}", JToken.FromObject(widgetFieldValue).ToString());
            // return the array with the new content item reference as JSON
            return new WidgetPropertyMigrationResult(JToken.FromObject(widgetFieldValue));
        }
        else
        {
            logger.LogError("Failed to parse 'Hero' json {Json}", value?.ToString() ?? "<null>");

            // leave value as it is
            return new WidgetPropertyMigrationResult(value);
        }
    }

    private async Task<ContentItemReference> CreateHeroContentItem(JToken value)
    {
        const string KENTICO_DEFAULT_WORKSPACE_NAME = "KenticoDefault"; // from DB
        const int GLOBAL_ADMINISTRATOR_USER_ID = 53; // 53 is an ID of the Global Administrator user

        // var heroImage = value["image"];
        var heroHeading = value["text"] ?? "text";
        var heroTarget = value["buttonTarget"] ?? "buttonTarget";
        var heroCallToAction = value["buttonText"] ?? "buttonText";

        logger.LogInformation("Creating Hero content item with Heading: {Heading}, Target: {Target}, CallToAction: {CallToAction}",
            heroHeading?.ToString() ?? "<null>",
            heroTarget?.ToString() ?? "<null>",
            heroCallToAction?.ToString() ?? "<null>"
            );

        var ciManager = Service.Resolve<IContentItemManagerFactory>().Create(GLOBAL_ADMINISTRATOR_USER_ID);

        var createContentItemParameters = new CreateContentItemParameters(
            contentTypeName: HERO_CONTENT_TYPE_NAME,
            name: "MigratedHeroItem" + Guid.NewGuid().ToString("N"),
            displayName: "Hero item",
            languageName: "en-US",
            workspaceName: KENTICO_DEFAULT_WORKSPACE_NAME
        );
        var contentItemData = new ContentItemData();
        contentItemData.SetValue("HeroHeading", heroHeading?.ToString() ?? "text");
        contentItemData.SetValue("HeroTarget", heroTarget?.ToString() ?? "buttonTarget");
        contentItemData.SetValue("HeroCallToAction", heroCallToAction?.ToString() ?? "buttonText");

        logger.LogInformation("[Hero] createContentItemParameters: {}", JToken.FromObject(createContentItemParameters).ToString());
        logger.LogInformation("[Hero] contentItemData HeroHeading: {}", contentItemData.TryGetValue<object>("HeroHeading", out var heading) ? heading : "<null>");
        logger.LogInformation("[Hero] contentItemData HeroTarget: {}", contentItemData.TryGetValue<object>("HeroTarget", out var target) ? target : "<null>");
        logger.LogInformation("[Hero] contentItemData HeroCallToAction: {}", contentItemData.TryGetValue<object>("HeroCallToAction", out var callToAction) ? callToAction : "<null>");

        int itemId = await ciManager.Create(createContentItemParameters, contentItemData);

        logger.LogInformation("Content item creation returned ID: {Id}", itemId);

        if (itemId <= 0)
        {
            throw new Exception("Unable to create content item");
        }
        if (!await ciManager.TryPublish(itemId, "en-US"))
        {
            throw new Exception("Could not publish Hero item");
        }
        return new ContentItemReference { Identifier = CMS.ContentEngine.Internal.ContentItemInfo.Provider.Get(itemId).ContentItemGUID };
    }
}
