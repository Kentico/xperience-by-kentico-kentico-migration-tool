using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Services;
using Migration.Toolkit.Core.Services.CmsClass;
using Migration.Toolkit.KX13;
using Migration.Toolkit.KXP;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Context;
using Migration.Toolkit.TestConsole;

ConsoleHelper.EnableVirtualTerminalProcessing();

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
        builder.AddFile(config.GetSection("Logging"));
    });

services.UseKx13DbContext(settings);
services.UseKxpDbContext(settings);
services.UseKxpApi(config.GetRequiredSection("Settings").GetRequiredSection("TargetKxoApiSettings"), settings.TargetCmsDirPath);
services.AddSingleton(settings);
services.UseToolkitCore();

await using var serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
var mappingContext = scope.ServiceProvider.GetRequiredService<PrimaryKeyMappingContext>();
var toolkitConfiguration = scope.ServiceProvider.GetRequiredService<ToolkitConfiguration>();
var tableTypeLookupService = scope.ServiceProvider.GetRequiredService<TableReflectionService>();
// var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();
var kxpContext = scope.ServiceProvider.GetRequiredService<IDbContextFactory<KxpContext>>().CreateDbContext();


var countryMigrator = scope.ServiceProvider.GetRequiredService<CountryMigrator>();

countryMigrator.MigrateCountriesAndStates();

// var classService = scope.ServiceProvider.GetRequiredService<ClassService>();
// var classFields = classService.GetClassFields(new Guid("C1C4DEDA-9280-436C-9BF7-F1A0C706EC80")); // custom.News
//
// foreach (var classColumnModel in classFields)
// {
//     logger.LogInformation("{classColumnModel}", classColumnModel);
// }
//
// kxpContext.Dispose();
