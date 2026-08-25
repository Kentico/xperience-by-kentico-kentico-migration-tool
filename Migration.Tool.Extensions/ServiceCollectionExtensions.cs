using Microsoft.Extensions.DependencyInjection;
using Migration.Tool.Extensions.CommunityMigrations;
using Migration.Tool.Extensions.DefaultMigrations;
using Migration.Tool.KXP.Api.Services.CmsClass;

namespace Migration.Tool.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseCustomizations(this IServiceCollection services)
    {
        // Default built-in migrations
        services.AddTransient<IFieldMigration, AssetMigration>();
        services.AddTransient<IWidgetPropertyMigration, WidgetFileMigration>();
        services.AddTransient<IWidgetPropertyMigration, WidgetPathSelectorMigration>();
        services.AddTransient<IWidgetPropertyMigration, WidgetPageSelectorMigration>();

        // Community migration samples (uncomment to enable)
        services.AddTransient<IFieldMigration, SampleTextMigration>();
        // services.AddTransient<IFieldMigration, SelectDocumentMigration>();
        // services.AddTransient<IWidgetMigration, SampleWidgetMigration>();
        // services.AddTransient<ContentItemDirectorBase, SamplePageToWidgetDirector>();
        // services.AddTransient<ContentItemDirectorBase, SampleChildLinkDirector>();
        // services.AddTransient<ContentItemDirectorBase, SampleLinkedPageDirector>();
        // services.AddTransient<ContentItemDirectorBase>(sp => new JsonBasedTypeRemapDirector("migration-mapping.json"));

        // Class mapping samples (uncomment to enable)
        // services.AddClassMergeExample();
        // services.AddClassMergeExampleAsReusable();
        // services.AddSimpleRemodelingSample();
        // services.AddReusableRemodelingSample();
        // services.AddReusableSchemaIntegrationSample();
        // services.AddReusableSchemaAutoGenerationSample();
        // services.AddCustomContactFieldMappingSample();

        // Routing content items to prefabricated content types (i.e., types not created by Migration Tool --page-types CLI argument)
        //
        // The following two methods may be combined, but one particular content type should be covered by only one of them.
        //
        //   1. Content item director method is applicable if each target field has a matching source field that has the same name.
        //      You may use JsonBasedTypeRemapDirector or drive the mapping directly from your own director using IContentItemActionProvider.OverrideTargetType method.
        //
        //      services.AddTransient<ContentItemDirectorBase>(sp => new JsonBasedTypeRemapDirector("migration-mapping.json"));
        //
        //   2. Custom mapping method gives you the highest flexibility if the prefabricated type doesn't match the source type exactly.
        //
        //      services.AddMappingToPrefabricatedContentTypeSample();

        return services;
    }
}
