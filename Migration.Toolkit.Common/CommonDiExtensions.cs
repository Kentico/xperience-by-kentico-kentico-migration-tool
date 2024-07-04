namespace Migration.Toolkit.Common;

using Microsoft.Extensions.DependencyInjection;
using Migration.Toolkit.Common.MigrationProtocol;

public static class CommonDiExtensions
{
    public static IServiceCollection UseToolkitCommon(this IServiceCollection services)
    {
        services.AddSingleton<IProtocol, Protocol>();
        services.AddSingleton<IMigrationProtocol, TextMigrationProtocol>();
        services.AddSingleton<IMigrationProtocol, DebugMigrationProtocol>();
        return services;
    }
}