namespace Migration.Toolkit.KX12;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Migration.Toolkit.Common;
using Migration.Toolkit.KX12.Context;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseKx12DbContext(this IServiceCollection services, ToolkitConfiguration toolkitConfiguration)
    {
        services.AddDbContextFactory<KX12Context>(options => options.UseSqlServer(toolkitConfiguration.KxConnectionString));
        return services;
    }
}