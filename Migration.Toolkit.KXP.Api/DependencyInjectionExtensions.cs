namespace Migration.Toolkit.KXP.Api;

using CMS.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddSingleton<KxpClassFacade>();
        services.AddSingleton<KxpMediaFileFacade>();
        services.AddSingleton<KxpPageFacade>();
        // TODOV27 tomas.krch: 2023-09-05: obsolete registration
        //services.AddSingleton<KxpSiteFacade>();

        return services;
    }
}