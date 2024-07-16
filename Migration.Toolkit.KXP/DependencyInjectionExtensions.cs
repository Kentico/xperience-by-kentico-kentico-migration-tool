using System.Diagnostics;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Migration.Toolkit.Common;
using Migration.Toolkit.KXP.Context;

namespace Migration.Toolkit.KXP;

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
