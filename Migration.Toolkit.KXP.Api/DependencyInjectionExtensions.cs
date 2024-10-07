using CMS.Base;
using CMS.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Migration.Toolkit.KXP.Api.Services.CmsClass;

namespace Migration.Toolkit.KXP.Api;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseKxpApi(this IServiceCollection services, IConfiguration configuration, string? applicationPhysicalPath = null)
    {
        Service.Use<IConfiguration>(configuration);
        if (applicationPhysicalPath != null && Directory.Exists(applicationPhysicalPath))
        {
            SystemContext.WebApplicationPhysicalPath = applicationPhysicalPath;
        }


        services.AddSingleton<IFieldMigrationService, FieldMigrationService>();
        services.AddSingleton<KxpApiInitializer>();
        services.AddSingleton(s => (s.GetService<IFieldMigrationService>() as FieldMigrationService)!);

        services.AddSingleton<KxpClassFacade>();
        services.AddSingleton<KxpMediaFileFacade>();

        return services;
    }
}
