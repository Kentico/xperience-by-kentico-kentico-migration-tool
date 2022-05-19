using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core;
using Migration.Toolkit.Core.Configuration;
using Migration.Toolkit.Core.Contexts;
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
            options.TimestampFormat = "hh:mm:ss.fff ";
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
    SiteIdMapping = new()
    {
        { 1, 1 }
    }
});

await using var serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

string Yellow(string ctext) => $"\x1b[33m{ctext}\x1b[0m";
string Green(string ctext) => $"\x1b[32m{ctext}\x1b[0m";
// string Red(string ctext) => $"\x1b[31m{ctext}\x1b[0m";
void WriteCommandDesc(string desc, string commandMoniker) {
    Console.WriteLine($"{Yellow(commandMoniker)}: {desc}");
}

var mappingContext = scope.ServiceProvider.GetRequiredService<PrimaryKeyMappingContext>();
var globalConfiguration = scope.ServiceProvider.GetRequiredService<GlobalConfiguration>();
foreach (var (k, v) in globalConfiguration.SiteIdMapping)
{
    if (k.HasValue && v.HasValue)
    {
        mappingContext.SetMapping<CmsSite>(s => s.SiteId, k.Value, v.Value);
    }
}

void PrintCommandDescriptions()
{
    WriteCommandDesc($"starts migration of {Green(MigratePageTypesCommand.MonikerFriendly)}", $"migrate --{MigratePageTypesCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigratePagesCommand.MonikerFriendly)}", $"migrate --{MigratePagesCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateSettingKeysCommand.MonikerFriendly)}", $"migrate --{MigrateSettingKeysCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateContactGroupsCommand.MonikerFriendly)}", $"migrate --{MigrateContactGroupsCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateContactManagementCommand.MonikerFriendly)}", $"migrate --{MigrateContactManagementCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateDataProtectionCommand.MonikerFriendly)}", $"migrate --{MigrateDataProtectionCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateFormsCommand.MonikerFriendly)}", $"migrate --{MigrateFormsCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateMediaLibrariesCommand.MonikerFriendly)}", $"migrate --{MigrateMediaLibrariesCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateSitesCommand.MonikerFriendly)}", $"migrate --{MigrateSitesCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateUsersCommand.MonikerFriendly)}", $"migrate --{MigrateUsersCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateWebFarmsCommand.MonikerFriendly)}", $"migrate --{MigrateWebFarmsCommand.Moniker}");
    // Console.WriteLine($"Run with option {Yellow("--dry")} to execute command without persistence");
}

var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();

switch (args.Length)
{
    case 1 when args[0].IsIn("help", "h"):
    {
        PrintCommandDescriptions();
        break;
    }

    case 0:
    {
        Console.WriteLine($"Invalid arguments, for help call with command {Yellow("help")}, usable commands:");
        PrintCommandDescriptions();
        break;
    }

    default:
    {
        var dry = args.Contains("--dry");
        
        if (args[0] == "migrate" && args[1] == $"--{MigrateContactGroupsCommand.Moniker}")
        {
            await mediatr.Send(new MigrateContactGroupsCommand(dry));
            logger.LogInformation("Finished!");
        }

        if (args[0] == "migrate" && args[1] == $"--{MigrateContactManagementCommand.Moniker}")
        {
            await mediatr.Send(new MigrateContactManagementCommand(dry));
            logger.LogInformation("Finished!");
            break;
        }

        if (args[0] == "migrate" && args[1] == $"--{MigrateDataProtectionCommand.Moniker}")
        {
            await mediatr.Send(new MigrateDataProtectionCommand(dry));
            logger.LogInformation("Finished!");
            break;
        }

        if (args[0] == "migrate" && args[1] == $"--{MigrateFormsCommand.Moniker}")
        {
            await mediatr.Send(new MigrateFormsCommand(dry));
            logger.LogInformation("Finished!");
            break;
        }

        if (args[0] == "migrate" && args[1] == $"--{MigrateMediaLibrariesCommand.Moniker}")
        {
            await mediatr.Send(new MigrateMediaLibrariesCommand(dry));
            logger.LogInformation("Finished!");
            break;
        }

        if (args[0] == "migrate" && args[1] == $"--{MigratePageTypesCommand.Moniker}")
        {
            await mediatr.Send(new MigratePageTypesCommand(dry));
            logger.LogInformation("Finished!");
            break;
        }
        
        if (args[0] == "migrate" && args[1] == $"--{MigratePagesCommand.Moniker}")
        {
            await mediatr.Send(new MigratePagesCommand(dry));
            logger.LogInformation("Finished!");
            break;
        }

        if (args[0] == "migrate" && args[1] == $"--{MigrateSettingKeysCommand.Moniker}")
        {
            await mediatr.Send(new MigrateSettingKeysCommand(dry));
            logger.LogInformation("Finished!");
            break;
        }

        if (args[0] == "migrate" && args[1] == $"--{MigrateSitesCommand.Moniker}")
        {
            await mediatr.Send(new MigrateSitesCommand(dry));
            logger.LogInformation("Finished!");
            break;
        }

        if (args[0] == "migrate" && args[1] == $"--{MigrateUsersCommand.Moniker}")
        {
            await mediatr.Send(new MigrateUsersCommand(dry));
            logger.LogInformation("Finished!");
            break;
        }

        if (args[0] == "migrate" && args[1] == $"--{MigrateWebFarmsCommand.Moniker}")
        {
            await mediatr.Send(new MigrateWebFarmsCommand(dry));
            logger.LogInformation("Finished!");
            break;
        }
        
        Console.WriteLine($"Invalid arguments, for help call with command {Yellow("help")}, usable commands:");
        PrintCommandDescriptions();

        break;
    }
};