using Microsoft.Extensions.Logging;
using Migration.Tool.Extensions.DefaultMigrations;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Extensions.CustomWidgetMigrations;

public class HeroBannerWidgetMigration(ILogger<HeroBannerWidgetMigration> logger) : IWidgetMigration
{
    public int Rank => 100;
    public const string SOURCE_WIDGET_IDENTIFIER = "Xperience.Widgets.HeroBannerWidget";
    public const int SOURCE_SITE_ID = 1; // Dancing goat site ID in the source instance
    public Task<WidgetMigrationResult> MigrateWidget(WidgetIdentifier identifier, JToken? value, WidgetMigrationContext context)
    {
        value!["type"] = "Xperience.Widgets.HeroBannerWidget"; //Migrate to different type of widget

        // a simple example of a widget-specific property transformation that's not intended to be reused across widgets/solution
        // as an alternative to the property migration class approach
        var ctaTargetToBool = (JToken? value) => value?.ToString() == "_blank";

        var variants = (JArray)value!["variants"]!;
        var singleVariant = variants[0];
        singleVariant["properties"] = new JObject
        {
            ["title"] = singleVariant["properties"]!["title"],
            ["content"] = singleVariant["properties"]!["content"],
            ["logo"] = singleVariant["properties"]!["logo"],
            ["image"] = singleVariant["properties"]!["image"],

            // this group of properties could be migrated to a separate reusable type (e.g., Link or CtaLink) and turned into one property with a reference - in the guide refer to the "Migrate widget data as reusable content" guide
            ["ctaText"] = singleVariant["properties"]!["ctaText"],
            ["ctaOpenInNewTab"] = ctaTargetToBool(singleVariant["properties"]!["ctaTarget"]),
            // added property for better UX - to select between internal and external link in the target instance
            ["ctaTargetType"] = singleVariant["properties"]!["ctaUrlExternal"] != null && !string.IsNullOrEmpty(singleVariant["properties"]!["ctaUrlExternal"]!.ToString())
                ? "absolute" : "page",
            ["ctaTargetPage"] = singleVariant["properties"]!["ctaUrlInternal"],
            ["ctaTargetUrl"] = singleVariant["properties"]!["ctaUrlExternal"],

            ["displayAsLink"] = singleVariant["properties"]!["displayAsLink"],

            // here we could utilize the reference to reusable Link or CtaLink again
            ["secondaryLinkText"] = singleVariant["properties"]!["cta2Text"],
            ["secondaryLinkOpenInNewTab"] = ctaTargetToBool(singleVariant["properties"]!["cta2Target"]),
            // added property for better UX - to select between internal and external link in the target instance
            ["secondaryLinkTargetType"] = singleVariant["properties"]!["ctaUrl2External"] != null && !string.IsNullOrEmpty(singleVariant["properties"]!["ctaUrl2External"]!.ToString())
                ? "absolute" : "page",
            ["secondaryLinkTargetPage"] = singleVariant["properties"]!["ctaUrl2Internal"],
            ["secondaryLinkTargetUrl"] = singleVariant["properties"]!["ctaUrl2External"],

            ["scrollText"] = singleVariant["properties"]!["scrollText"],
            ["graphicVersion"] = singleVariant["properties"]!["graphicVersion"],
            ["paddingSizeTop"] = singleVariant["properties"]!["paddingSizeTop"],
            ["paddingSizeBottom"] = singleVariant["properties"]!["paddingSizeBottom"]
        };

        //For new properties, we must explicitly define property migration classes
        var propertyMigrations = new Dictionary<string, Type>
        {
            ["logo"] = typeof(WidgetCustomSelectorMigration),
            ["image"] = typeof(WidgetCustomSelectorMigration),
            ["ctaTargetPage"] = typeof(WidgetPageSelectorToCombinedSelectorMigration),
            // we could do the same for cta2UrlInternal if needed, but in my example, I want to show the default migration to PageSelector as well
            ["secondaryLinkTargetPage"] = typeof(WidgetPageSelectorMigration)
        };

        return Task.FromResult(new WidgetMigrationResult(value, propertyMigrations));
    }

    public bool ShallMigrate(WidgetMigrationContext context, WidgetIdentifier identifier) =>
        string.Equals(SOURCE_WIDGET_IDENTIFIER, identifier.TypeIdentifier, StringComparison.InvariantCultureIgnoreCase)
    && SOURCE_SITE_ID == context.SiteId;

}
