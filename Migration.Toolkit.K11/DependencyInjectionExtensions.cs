namespace Migration.Toolkit.K11;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Migration.Toolkit.Common;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseK11DbContext(this IServiceCollection services, ToolkitConfiguration toolkitConfiguration)
    {
        services.AddDbContextFactory<K11Context>(options => options.UseSqlServer(toolkitConfiguration.KxConnectionString));
        return services;
    }
}