using System.Reflection;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.CLI;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.Services;
using Migration.Toolkit.Core.K11;
using Migration.Toolkit.Core.KX12;
using Migration.Toolkit.Core.KX13;
using Migration.Toolkit.K11;
using Migration.Toolkit.KX12;
using Migration.Toolkit.KX13;
using Migration.Toolkit.KXP;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Context;
using Migration.Toolkit.Source;

using static Migration.Toolkit.Common.Helpers.ConsoleHelper;

EnableVirtualTerminalProcessing();

// https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration

var config = new ConfigurationBuilder()
        .SetBasePath(Environment.CurrentDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
        .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
        .Build()
    ;

var validationErrors = ConfigurationValidator.GetValidationErrors(config);
bool anyValidationErrors = false;
foreach (var (validationMessageType, message, recommendedFix) in validationErrors)
{
    switch (validationMessageType)
    {
        case ValidationMessageType.Error:
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.Write(Resources.ConfigurationError, RED, RESET, message);
                if (!string.IsNullOrWhiteSpace(recommendedFix))
                {
                    Console.Write(Resources.ConfigurationRecommendedFix, YELLOW, RESET, recommendedFix);
                }

                anyValidationErrors = true;
                Console.WriteLine();
            }

            break;
        case ValidationMessageType.Warning:
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.Write(Resources.ConfigurationWarning, YELLOW, RESET, message);
                if (!string.IsNullOrWhiteSpace(recommendedFix))
                {
                    Console.Write(Resources.ConfigurationRecommendedFix, YELLOW, RESET, recommendedFix);
                }

                Console.WriteLine();
            }

            break;
        default:
            break;
    }
}

if (anyValidationErrors)
{
    Console.WriteLine(Resources.ProgramAwaitingExitMessage);
    if (!args.Contains("--nowait"))
    {
        Console.ReadKey();
    }
    return;
}

var settingsSection = config.GetRequiredSection(ConfigurationNames.Settings);
var settings = settingsSection.Get<ToolkitConfiguration>() ?? new();
var kxpApiSettings = settingsSection.GetSection(ConfigurationNames.XbKApiSettings);
settings.SetXbKConnectionStringIfNotEmpty(kxpApiSettings["ConnectionStrings:CMSConnectionString"]);

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
        builder.AddFile(config.GetSection($"{ConfigurationNames.Logging}:File"));
    });


services.UseKsToolkitCore();
await using var conn = new SqlConnection(settings.KxConnectionString);
try
{
    await conn.OpenAsync();
    switch (VersionHelper.GetInstanceVersion(conn))
    {
        case { Major: 11 }:
        {
            services.UseK11DbContext(settings);
            services.UseK11ToolkitCore();
            Console.WriteLine($@"Source instance {Green($"version 11")} detected.");
            break;
        }
        case { Major: 12 }:
        {
            services.UseKx12DbContext(settings);
            services.UseKx12ToolkitCore();
            Console.WriteLine($@"Source instance {Green($"version 12")} detected");
            break;
        }
        case { Major: 13 }:
        {
            services.UseKx13DbContext(settings);
            services.UseKx13ToolkitCore();
            Console.WriteLine($@"Source instance {Green("version 13")} detected");
            break;
        }
        case { Major: { } version }:
        {
            Console.WriteLine($@"Source instance {Green($"version {version}")} detected. This instance is not supported");
            break;
        }
        default:
        {
            Console.WriteLine(
                $@"{Red("Parsing of source instance version failed")}, please check connection string and if source instance settings key with key name 'CMSDBVersion' is correctly filled.");
            return;
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($@"Startup failed with error: {ex}");
    return;
}


services.UseKxpDbContext(settings);


services.UseKxpApi(kxpApiSettings, settings.XbKDirPath);
services.AddSingleton(settings);
services.AddSingleton<ICommandParser, CommandParser>();
services.UseToolkitCommon();

await using var serviceProvider = services.BuildServiceProvider();
KsCoreDiExtensions.InitServiceProvider(serviceProvider);
using var scope = serviceProvider.CreateScope();

var loader = scope.ServiceProvider.GetRequiredService<IModuleLoader>();
await loader.LoadAsync();

var commandParser = scope.ServiceProvider.GetRequiredService<ICommandParser>();
var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();
var kxpContext = scope.ServiceProvider.GetRequiredService<IDbContextFactory<KxpContext>>().CreateDbContext();

var argsQ = new Queue<string>(args);
bool bypassDependencyCheck = false;
var commands = commandParser.Parse(argsQ, ref bypassDependencyCheck);

kxpContext.Dispose();

// sort commands
commands = commands.OrderBy(x => x.Rank).ToList();

var satisfiedDependencies = new HashSet<Type>();
bool dependenciesSatisfied = true;
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
                object? cmdMoniker = commandType.GetProperty("Moniker", BindingFlags.Static | BindingFlags.Public)?.GetValue(null);
                object? cmdMonikerNeeded = commandDependency.GetProperty("Moniker", BindingFlags.Static | BindingFlags.Public)?.GetValue(null);

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
