using CMS.Base;
using CMS.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.KXP.Api.Services.CmsResource;

namespace Migration.Tool.KXP.Api;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseKxpApi(this IServiceCollection services, IConfiguration configuration, string? applicationPhysicalPath = null)
    {
        Service.Use<IConfiguration>(configuration);
        if (applicationPhysicalPath != null && Directory.Exists(applicationPhysicalPath))
        {
            SystemContext.WebApplicationPhysicalPath = applicationPhysicalPath;
        }

        services.AddTransient<IFieldMigrationService, FieldMigrationService>();
        services.AddTransient<WidgetMigrationService>();

        services.AddSingleton<KxpApiInitializer>();
        services.AddSingleton(s => (s.GetService<IFieldMigrationService>() as FieldMigrationService)!);

        services.AddSingleton<KxpClassFacade>();
        services.AddSingleton<KxpMediaFileFacade>();
        services.AddSingleton<SqlResourceProvider>();

        return services;
    }
}
