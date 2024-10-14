using Microsoft.Extensions.DependencyInjection;
using Migration.Tool.Extensions.CommunityMigrations;
using Migration.Tool.Extensions.DefaultMigrations;
using Migration.Tool.KXP.Api.Services.CmsClass;

namespace Migration.Tool.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseCustomizations(this IServiceCollection services)
    {
        services.AddTransient<IFieldMigration, AssetMigration>();
        services.AddTransient<IFieldMigration, SampleTextMigration>();

        // services.AddClassMergeExample();
        // services.AddSimpleRemodelingSample();
        // services.AddReusableSchemaIntegrationSample();
        return services;
    }
}
