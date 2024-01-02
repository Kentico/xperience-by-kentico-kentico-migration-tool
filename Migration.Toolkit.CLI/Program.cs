using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.CLI;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Services.Ipc;
using Migration.Toolkit.Core;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Services;
using Migration.Toolkit.KX13;
using Migration.Toolkit.KXP;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Context;

const string red = "\x1b[31m";
const string yellow = "\x1b[33m";
const string green = "\x1b[32m";
const string reset = "\x1b[0m";

string Yellow(string ctext) => $"{yellow}{ctext}{reset}";
string Green(string ctext) => $"{green}{ctext}{reset}";
string Red(string ctext) => $"{red}{ctext}{reset}";

ConsoleHelper.EnableVirtualTerminalProcessing();

// https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration

var config = new ConfigurationBuilder()
        .SetBasePath(Environment.CurrentDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
        .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
        .Build()
    ;

var validationErrors = ConfigurationValidator.GetValidationErrors(config);
var anyValidationErrors = false;
foreach (var (validationMessageType, message, recommendedFix) in validationErrors)
{
    switch (validationMessageType)
    {
        case ValidationMessageType.Error:
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.Write(Resources.ConfigurationError, red, reset, message);
                if (!string.IsNullOrWhiteSpace(recommendedFix))
                {
                    Console.Write(Resources.ConfigurationRecommendedFix, yellow, reset, recommendedFix);
                }
                anyValidationErrors = true;
                Console.WriteLine();
            }
            break;
        case ValidationMessageType.Warning:
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.Write(Resources.ConfigurationWarning, yellow, reset, message);
                if (!string.IsNullOrWhiteSpace(recommendedFix))
                {
                    Console.Write(Resources.ConfigurationRecommendedFix, yellow, reset, recommendedFix);
                }

                Console.WriteLine();
            }

            break;
    }
}

if (anyValidationErrors)
{
    Console.WriteLine(Resources.ProgramAwaitingExitMessage);
    Console.ReadKey();
    return;
}

var settingsSection = config.GetRequiredSection(ConfigurationNames.Settings);
var settings = settingsSection.Get<ToolkitConfiguration>();
settings.EntityConfigurations ??= new EntityConfigurations();

var services = new ServiceCollection();

services
    .AddLogging(builder =>
    {
        builder.AddConfiguration(config.GetSection(ConfigurationNames.Logging));
        builder.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "hh:mm:ss.fff ";
        });
        builder.AddFile(config.GetSection(ConfigurationNames.Logging));
    });

services.UseKx13DbContext(settings);
services.UseKxpDbContext(settings);

var kxpApiSettings =
    settingsSection.GetSection(ConfigurationNames.XbKApiSettings) ??
#pragma warning disable CS0618 // usage of obsolete symbol is related to backwards compatibility maintenance
    settingsSection.GetSection(ConfigurationNames.TargetKxpApiSettings) ??
    settingsSection.GetSection(ConfigurationNames.TargetKxoApiSettings);
#pragma warning restore CS0618

services.UseKxpApi(kxpApiSettings, settings.XbKDirPath);
services.AddSingleton(settings);
services.UseKx13ToolkitCore();

await using var serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();
// var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

void WriteCommandDesc(string desc, string commandMoniker)
{
    Console.WriteLine($@"{Yellow(commandMoniker)}: {desc}");
}

var mappingContext = scope.ServiceProvider.GetRequiredService<PrimaryKeyMappingContext>();
var toolkitConfiguration = scope.ServiceProvider.GetRequiredService<ToolkitConfiguration>();
var tableTypeLookupService = scope.ServiceProvider.GetRequiredService<TableReflectionService>();
foreach (var (k, ek) in toolkitConfiguration.EntityConfigurations)
{
    var tableType = tableTypeLookupService.GetSourceTableTypeByTableName(k);

    foreach (var (kPkName, mappings) in ek.ExplicitPrimaryKeyMapping)
    {
        foreach (var (kPk, vPk) in mappings)
        {
            // TODO tk: 2022-05-26 report incorrect property setting
            if (int.TryParse(kPk, out var kPkParsed) && vPk.HasValue)
            {
                mappingContext.SetMapping(tableType, kPkName, kPkParsed, vPk.Value);
            }
        }
    }
}


void PrintCommandDescriptions()
{
    WriteCommandDesc($"starts migration of {Green(MigratePageTypesCommand.MonikerFriendly)}", $"migrate --{MigratePageTypesCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigratePagesCommand.MonikerFriendly)}", $"migrate --{MigratePagesCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateSettingKeysCommand.MonikerFriendly)}", $"migrate --{MigrateSettingKeysCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateContactManagementCommand.MonikerFriendly)}", $"migrate --{MigrateContactManagementCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateDataProtectionCommand.MonikerFriendly)}", $"migrate --{MigrateDataProtectionCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateFormsCommand.MonikerFriendly)}", $"migrate --{MigrateFormsCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateMediaLibrariesCommand.MonikerFriendly)}", $"migrate --{MigrateMediaLibrariesCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateSitesCommand.MonikerFriendly)}", $"migrate --{MigrateSitesCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateUsersCommand.MonikerFriendly)}", $"migrate --{MigrateUsersCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateMembersCommand.MonikerFriendly)}", $"migrate --{MigrateMembersCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateAttachmentsCommand.MonikerFriendly)}", $"migrate --{MigrateAttachmentsCommand.Moniker}");
    WriteCommandDesc($"starts migration of {Green(MigrateCustomModulesCommand.MonikerFriendly)}", $"migrate --{MigrateCustomModulesCommand.Moniker}");
}

var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();
var kxpContext = scope.ServiceProvider.GetRequiredService<IDbContextFactory<KxpContext>>().CreateDbContext();

var argsQ = new Queue<string>(args);
var commands = new List<ICommand>();
bool firstHaveToBeMigrate = true;
var bypassDependencyCheck = false;
while (argsQ.TryDequeue(out var arg))
{
    if (arg.IsIn("help", "h"))
    {
        PrintCommandDescriptions();
        break;
    }

    if (arg == "migrate" && firstHaveToBeMigrate)
    {
        firstHaveToBeMigrate = false;
        continue;
    }

    if (arg == "--bypass-dependency-check")
    {
        bypassDependencyCheck = true;
        continue;
    }

    if (firstHaveToBeMigrate)
    {
        Console.WriteLine($@"First must be command, for example {Green("migrate")}");
        PrintCommandDescriptions();
        break;
    }

    // if (arg == $"--{MigrateContactGroupsCommand.Moniker}")
    // {
    //     commands.Add(new MigrateContactGroupsCommand());
    //     continue;
    // }

    if (arg == $"--{MigrateContactManagementCommand.Moniker}")
    {
        commands.Add(new MigrateContactManagementCommand());
        continue;
    }

    if (arg == $"--{MigrateDataProtectionCommand.Moniker}")
    {
        // RequireNumberParameter("--batchSize", out var batchSize);
        commands.Add(new MigrateDataProtectionCommand());
        continue;
    }

    if (arg == $"--{MigrateFormsCommand.Moniker}")
    {
        commands.Add(new MigrateFormsCommand());
        continue;
    }

    if (arg == $"--{MigrateMediaLibrariesCommand.Moniker}")
    {
        commands.Add(new MigrateMediaLibrariesCommand());
        continue;
    }

    if (arg == $"--{MigratePageTypesCommand.Moniker}")
    {
        commands.Add(new MigratePageTypesCommand());
        continue;
    }

    if (arg == $"--{MigratePagesCommand.Moniker}")
    {
        commands.Add(new MigratePagesCommand());
        continue;
    }

    if (arg == $"--{MigrateSettingKeysCommand.Moniker}")
    {
        commands.Add(new MigrateSettingKeysCommand());
        continue;
    }

    if (arg == $"--{MigrateSitesCommand.Moniker}")
    {
        commands.Add(new MigrateSitesCommand());
        continue;
    }

    if (arg == $"--{MigrateUsersCommand.Moniker}")
    {
        commands.Add(new MigrateUsersCommand());
        continue;
    }

    if (arg == $"--{MigrateMembersCommand.Moniker}")
    {
        commands.Add(new MigrateMembersCommand());
    }

    if (arg == $"--{MigrateCustomModulesCommand.Moniker}")
    {
        commands.Add(new MigrateCustomModulesCommand());
        continue;
    }
}

kxpContext.Dispose();

var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
var ipc = scope.ServiceProvider.GetRequiredService<IpcService>();
var sourceInstanceContext = scope.ServiceProvider.GetRequiredService<SourceInstanceContext>();
try
{
    if (sourceInstanceContext.IsQuerySourceInstanceEnabled())
    {
        var ipcConfigured = await ipc.IsConfiguredAsync();
        if (ipcConfigured)
        {
            await sourceInstanceContext.RequestSourceInstanceInfo();
        }
    }
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Check if opt-in feature 'QuerySourceInstanceApi' is configured correctly and all connections configured are reachable and hosted on localhost");
    return;
}

// sort commands
commands = commands.OrderBy(x => x.Rank).ToList();

var satisfiedDependencies = new HashSet<Type>();
var dependenciesSatisfied = true;
if (!bypassDependencyCheck)
{
    foreach (var command in commands)
    {
        var commandType = command.GetType();
        satisfiedDependencies.Add(commandType);
        foreach (var commandDependency in command.Dependencies)
        {
            if (!satisfiedDependencies.Contains(commandDependency))
            {
                var cmdMoniker = commandType.GetProperty("Moniker", BindingFlags.Static | BindingFlags.Public)?.GetValue(null);
                var cmdMonikerNeeded = commandDependency.GetProperty("Moniker", BindingFlags.Static | BindingFlags.Public)?.GetValue(null);

                dependenciesSatisfied = false;

                Console.WriteLine($@"Migration {Yellow($"--{cmdMoniker}")} needs to run migration {Red($"--{cmdMonikerNeeded}")} before.");
            }
        }
    }
}

if (!dependenciesSatisfied)
{
    return;
}

foreach (var command in commands)
{
    await mediatr.Send(command);
}

if (!args.Contains("--nowait"))
{
    Console.WriteLine(Resources.ProgramAwaitingExitMessage);
    Console.ReadKey();
}