using Microsoft.Extensions.DependencyInjection;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;

namespace Migration.Tool.Common;

public static class CommonDiExtensions
{
    public static IServiceCollection UseToolCommon(this IServiceCollection services)
    {
        services.AddSingleton<IProtocol, Protocol>();
        services.AddSingleton<IMigrationProtocol, TextMigrationProtocol>();
        services.AddSingleton<IMigrationProtocol, DebugMigrationProtocol>();
        services.AddSingleton<UrlProtocol>();

        services.AddTransient<DefaultCustomTableClassMappingHandler>();

        return services;
    }
}
