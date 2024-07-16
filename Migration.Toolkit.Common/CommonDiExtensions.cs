using Microsoft.Extensions.DependencyInjection;

using Migration.Toolkit.Common.MigrationProtocol;

namespace Migration.Toolkit.Common;

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
