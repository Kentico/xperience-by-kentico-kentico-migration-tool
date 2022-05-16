using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Migration.Toolkit.Common;
using Migration.Toolkit.KXO.Context;

namespace Migration.Toolkit.KXO;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseKxoDbContext(this IServiceCollection services, ToolkitConfiguration toolkitConfiguration)
    {
        services.AddDbContextFactory<KxoContext>(options => options.UseSqlServer(toolkitConfiguration.TargetConnectionString));
        return services;
    }
}