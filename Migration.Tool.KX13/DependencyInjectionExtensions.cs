using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Migration.Tool.Common;
using Migration.Tool.KX13.Context;

namespace Migration.Tool.KX13;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseKx13DbContext(this IServiceCollection services, ToolConfiguration toolConfiguration)
    {
        services.AddDbContextFactory<KX13Context>(options => options.UseSqlServer(toolConfiguration.KxConnectionString));
        return services;
    }
}
