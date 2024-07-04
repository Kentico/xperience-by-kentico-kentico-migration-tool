namespace Migration.Toolkit.KXP.Api;

using CMS.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Migration.Toolkit.KXP.Api.Services.CmsClass;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseKxpApi(this IServiceCollection services, IConfiguration configuration, string? applicationPhysicalPath = null)
    {
        Service.Use<IConfiguration>(configuration);
        if (applicationPhysicalPath != null && Directory.Exists(applicationPhysicalPath))
        {
            CMS.Base.SystemContext.WebApplicationPhysicalPath = applicationPhysicalPath;
        }

        services.AddSingleton<KxpApiInitializer>();
        services.AddSingleton<FieldMigrationService>();

        services.AddSingleton<KxpClassFacade>();
        services.AddSingleton<KxpMediaFileFacade>();

        return services;
    }
}