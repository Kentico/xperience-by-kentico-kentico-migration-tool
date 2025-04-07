using Migration.Tool.Extensions.DefaultMigrations;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Extensions.CommunityMigrations;
public class SampleWidgetMigration : IWidgetMigration
{
    public int Rank => 1;

    public Task<WidgetMigrationResult> MigrateWidget(WidgetIdentifier identifier, JToken? value, WidgetMigrationContext context)
    {
        value!["type"] = "DancingGoat.HeroWidget"; //Migrate to different type of widget

        //Recombine the properties
        var variants = (JArray)value!["variants"]!;
        var singleVariant = variants[0];
        singleVariant["properties"] = new JObject
        {
            ["teaser"] = singleVariant["properties"]!["image"],
            ["text"] = singleVariant["properties"]!["text"]
        };

        //For new properties, we must explicitly define property migration classes
        var propertyMigrations = new Dictionary<string, Type>
        {
            ["teaser"] = typeof(WidgetFileMigration)
            //["text"] ... this is an unchanged property from the original widget => default widget property migrations will handle it
        };

        return Task.FromResult(new WidgetMigrationResult(value, propertyMigrations));
    }

    public bool ShallMigrate(WidgetMigrationContext context, WidgetIdentifier identifier) => string.Equals("DancingGoat.HomePage.BannerWidget", identifier.TypeIdentifier, StringComparison.InvariantCultureIgnoreCase);
}
