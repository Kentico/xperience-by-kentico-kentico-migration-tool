namespace Migration.Toolkit.KXP;

using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Migration.Toolkit.Common;
using Migration.Toolkit.KXP.Context;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseKxpDbContext(this IServiceCollection services, ToolkitConfiguration toolkitConfiguration)
    {
        services.AddDbContextFactory<KxpContext>(options =>
        {
            Debug.Assert(toolkitConfiguration.XbKConnectionString != null, "toolkitConfiguration.XbKConnectionString != null");
            options.UseSqlServer(toolkitConfiguration.XbKConnectionString);
        });
        return services;
    }
}