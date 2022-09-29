using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Migration.Toolkit.Common;
using Migration.Toolkit.KX13.Context;

namespace Migration.Toolkit.KX13;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseKx13DbContext(this IServiceCollection services, ToolkitConfiguration toolkitConfiguration)
    {
        services.AddDbContextFactory<KX13Context>(options => options.UseSqlServer(toolkitConfiguration.KxConnectionString));
        return services;
    }
}