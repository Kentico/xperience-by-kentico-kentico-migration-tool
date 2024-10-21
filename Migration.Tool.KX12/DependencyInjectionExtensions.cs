using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Migration.Tool.Common;
using Migration.Tool.KX12.Context;

namespace Migration.Tool.KX12;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseKx12DbContext(this IServiceCollection services, ToolConfiguration toolConfiguration)
    {
        services.AddDbContextFactory<KX12Context>(options => options.UseSqlServer(toolConfiguration.KxConnectionString));
        return services;
    }
}
