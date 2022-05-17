using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Commands;
using Migration.Toolkit.Core.Configuration;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigratePageTypes;
using Migration.Toolkit.Core.MigrateSettingKeys;
using Migration.Toolkit.KX13;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXO;

// https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration

var config = new ConfigurationBuilder()
        .SetBasePath(Environment.CurrentDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
        .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
        // .AddEnvironmentVariables()
        .Build()
    ;

var settings = config.GetRequiredSection("Settings").Get<ToolkitConfiguration>();

var services = new ServiceCollection();
services
    .AddLogging(builder =>
    {
        builder.AddConfiguration(config.GetSection("Logging"));
        builder.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "hh:mm:ss ";
        });
    });

services.UseKx13DbContext(settings);
services.UseKxoDbContext(settings);
services.UseToolkitCore();


services.AddSingleton(new EntityConfigurations
{
    {"CMS_Site", new EntityConfiguration(false) }
});

services.AddSingleton(new GlobalConfiguration
{
    SiteIdMapping = new Dictionary<int?, int?>
    {
        { 1, 1 }
    }
});

await using var serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

string Yellow(string ctext) => $"\x1b[33m{ctext}\x1b[0m";
string Green(string ctext) => $"\x1b[32m{ctext}\x1b[0m";
void WriteCommandDesc(string desc, string commandMoniker) {
    Console.WriteLine($"{Yellow(commandMoniker)}: {desc}");
}

var mappingContext = scope.ServiceProvider.GetRequiredService<PkMappingContext>();
var globalConfiguration = scope.ServiceProvider.GetRequiredService<GlobalConfiguration>();
foreach (var (k, v) in globalConfiguration.SiteIdMapping)
{
    if (k.HasValue && v.HasValue)
    {
        mappingContext.SetMapping<CmsSite>(s => s.SiteId, k.Value, v.Value);
    }
}

var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();

switch (args.Length)
{
    case 1 when args[0].IsIn("help", "h"):
        WriteCommandDesc($"starts migration of {Green("Page types")}", $"migrate --{MigratePageTypesCommand.Moniker}");
        WriteCommandDesc($"starts migration of {Green("Setting keys")}", $"migrate --{MigrateSettingKeysCommand.Moniker}");
        break;
    
    case 2 when args[0] == $"migrate" && args[1] == $"--{MigratePageTypesCommand.Moniker}":
    {
        await mediatr.Send(new MigratePageTypesCommand());
        logger.LogInformation("Finished!");
        break;
    }

    case 2 when args[0] == $"migrate" && args[1] == $"--{MigrateSettingKeysCommand.Moniker}":
    {
        await mediatr.Send(new MigrateSettingKeysCommand());
        logger.LogInformation("Finished!");
        break;
    }

    default:
        logger.LogError($"Invalid arguments, for help call with command {Yellow("help")}, usable commands:");
        WriteCommandDesc($"starts migration of {Green("Page types")}", $"migrate --{MigratePageTypesCommand.Moniker}");
        WriteCommandDesc($"starts migration of {Green("Setting keys")}", $"migrate --{MigrateSettingKeysCommand.Moniker}");
        break;
};