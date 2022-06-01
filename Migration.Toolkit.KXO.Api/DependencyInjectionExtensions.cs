using CMS.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Migration.Toolkit.KXO.Api;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseKxoApi(this IServiceCollection services, IConfiguration configuration, string? applicationPhysicalPath = null)
    {
        Service.Use<IConfiguration>(configuration);
        if (applicationPhysicalPath != null && File.Exists(applicationPhysicalPath))
        {
            CMS.Base.SystemContext.WebApplicationPhysicalPath = applicationPhysicalPath;    
        }

        services.AddSingleton<KxoApiInitializer>();

        services.AddSingleton<KxoClassFacade>();
        // ConnectionHelper.ConnectionString = _configuration.TargetConnectionString;
        
        return services;
    }
}