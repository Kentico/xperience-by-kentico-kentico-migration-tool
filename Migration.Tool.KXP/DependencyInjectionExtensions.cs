using System.Diagnostics;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Migration.Tool.Common;
using Migration.Tool.KXP.Context;

namespace Migration.Tool.KXP;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseKxpDbContext(this IServiceCollection services, ToolConfiguration toolConfiguration)
    {
        services.AddDbContextFactory<KxpContext>(options =>
        {
            Debug.Assert(toolConfiguration.XbKConnectionString != null, "toolConfiguration.XbKConnectionString != null");
            options.UseSqlServer(toolConfiguration.XbKConnectionString);
        });
        return services;
    }
}
