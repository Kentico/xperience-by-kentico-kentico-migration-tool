using Microsoft.Extensions.DependencyInjection;
using Migration.Tool.Extensions.CommunityMigrations;
using Migration.Tool.Extensions.CustomWidgetMigrations;
using Migration.Tool.Extensions.DefaultMigrations;
using Migration.Tool.KXP.Api.Services.CmsClass;

namespace Migration.Tool.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseCustomizations(this IServiceCollection services)
    {
        services.AddTransient<IFieldMigration, AssetMigration>();
        services.AddTransient<IFieldMigration, SampleTextMigration>();

        services.AddTransient<IWidgetPropertyMigration, WidgetFileMigration>();
        services.AddTransient<IWidgetPropertyMigration, WidgetPathSelectorMigration>();
        services.AddTransient<IWidgetPropertyMigration, WidgetPageSelectorMigration>();
        services.AddTransient<IWidgetMigration, HeroImageWidgetMigration>();
        services.AddTransient<IWidgetPropertyMigration, WidgetDataToHeroMigration>();


        // services.AddClassMergeExample();
        // services.AddClassMergeExampleAsReusable();
        // services.AddSimpleRemodelingSample();
        // services.AddReusableRemodelingSample();
        // services.AddReusableSchemaIntegrationSample();
        // services.AddReusableSchemaAutoGenerationSample();
        // services.AddTransient<ContentItemDirectorBase, SamplePageToWidgetDirector>();
        // services.AddTransient<ContentItemDirectorBase, SampleChildLinkDirector>();
        return services;
    }
}
