using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using Microsoft.Extensions.Logging;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Extensions.CustomWidgetMigrations;

public class HeroImageWidgetMigration(ILogger<HeroImageWidgetMigration> logger) : IWidgetMigration
{
    public int Rank => 100;
    public const string SOURCE_WIDGET_IDENTIFIER = "DancingGoat.LandingPage.HeroImage";
    public const int SOURCE_PAGE_ID = 1; // Dancing goat page ID in the source instance
    public const string HERO_CONTENT_TYPE_NAME = "DancingGoatCore.Hero";
    public async Task<WidgetMigrationResult> MigrateWidget(WidgetIdentifier identifier, JToken? value, WidgetMigrationContext context)
    {
        // Recombine the properties for the desired variants
        var variants = (JArray)value!["variants"]!;
        var singleVariant = variants[0];

        var heroItemReference = await MigrateHeroWidgetProperty(singleVariant["properties"]);
        singleVariant["properties"] = new JObject
        {
            ["hero"] = heroItemReference,
            ["image"] = singleVariant["properties"]!["image"],
            ["theme"] = singleVariant["properties"]!["theme"],
            ["openInNewTab"] = JToken.FromObject(false) // default value for new property
        };

        // the property names must match the target widget props names
        // e.g., text - Text, buttonText - ButtonText, buttonTarget - ButtonTarget, theme - Theme
        // otherwise the values will not map correctly

        // we don't need special widget property migrations in this case, so we can leave the dictionary empty
        // will be explained in the guide about widget property mapping
        var propertyMigrations = new Dictionary<string, Type> { };

        return new WidgetMigrationResult(value, propertyMigrations);
    }

    public bool ShallMigrate(WidgetMigrationContext context, WidgetIdentifier identifier) =>
        string.Equals(SOURCE_WIDGET_IDENTIFIER, identifier.TypeIdentifier, StringComparison.InvariantCultureIgnoreCase)
    && SOURCE_PAGE_ID == context.SiteId;

    private async Task<JToken?> MigrateHeroWidgetProperty(JToken? value)
    {
        if (value != null)
        {
            //need to manually create the target Hero content type before migrating pages and running this migration
            var heroContentItem = await CreateHeroContentItem(value!);

            var widgetFieldValue = heroContentItem != null
                ? [heroContentItem]
                : Array.Empty<object>();

            // return the array with the new content item reference as JSON
            return JToken.FromObject(widgetFieldValue);
        }
        else
        {
            logger.LogError("Failed to parse 'Hero' json {Json}", value?.ToString() ?? "<null>");

            // leave value as it is
            return value;
        }
    }

    private async Task<ContentItemReference> CreateHeroContentItem(JToken value)
    {
        const string KENTICO_DEFAULT_WORKSPACE_NAME = "KenticoDefault"; // value retrieved from the database
        const int GLOBAL_ADMINISTRATOR_USER_ID = 53; // 53 is an ID of the Global Administrator user
        const string ENGLISH_US_LANGUAGE = "en-US";

        // Extract properties from the JSON
        var heroHeading = value["text"];
        var heroTarget = value["buttonTarget"];
        var heroCallToAction = value["buttonText"];

        var ciManager = Service.Resolve<IContentItemManagerFactory>().Create(GLOBAL_ADMINISTRATOR_USER_ID);

        var createContentItemParameters = new CreateContentItemParameters(
            contentTypeName: HERO_CONTENT_TYPE_NAME,
            name: $"MigratedHeroItem{Guid.NewGuid():N}",
            displayName: $"Hero item - {heroHeading?.ToString() ?? "<null>"}",
            languageName: ENGLISH_US_LANGUAGE,
            workspaceName: KENTICO_DEFAULT_WORKSPACE_NAME
        );

        // the property names have to match the manually created content type's field names in the administration 
        var contentItemData = new ContentItemData();
        contentItemData.SetValue("HeroHeading", heroHeading?.ToString() ?? string.Empty);
        contentItemData.SetValue("HeroTarget", heroTarget?.ToString() ?? string.Empty);
        contentItemData.SetValue("HeroCallToAction", heroCallToAction?.ToString() ?? string.Empty);

        int itemId = await ciManager.Create(createContentItemParameters, contentItemData);

        if (itemId <= 0)
        {
            throw new Exception("Unable to create content item");
        }
        if (!await ciManager.TryPublish(itemId, ENGLISH_US_LANGUAGE))
        {
            throw new Exception("Could not publish Hero item");
        }
        return new ContentItemReference { Identifier = CMS.ContentEngine.Internal.ContentItemInfo.Provider.Get(itemId).ContentItemGUID };
    }
}
