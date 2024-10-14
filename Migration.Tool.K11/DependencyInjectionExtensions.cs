using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Migration.Tool.Common;

namespace Migration.Tool.K11;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseK11DbContext(this IServiceCollection services, ToolConfiguration toolConfiguration)
    {
        services.AddDbContextFactory<K11Context>(options => options.UseSqlServer(toolConfiguration.KxConnectionString));
        return services;
    }
}
