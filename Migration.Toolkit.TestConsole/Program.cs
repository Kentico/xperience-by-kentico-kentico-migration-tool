using System.Reflection;
using System.Text;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
services.UseKxpApi(config.GetRequiredSection("Settings").GetRequiredSection(ConfigurationNames.XbKApiSettings), settings.XbKDirPath);
services.AddSingleton(settings);
services.UseKx13ToolkitCore();

await using var serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
var mappingContext = scope.ServiceProvider.GetRequiredService<PrimaryKeyMappingContext>();
var toolkitConfiguration = scope.ServiceProvider.GetRequiredService<ToolkitConfiguration>();
var tableTypeLookupService = scope.ServiceProvider.GetRequiredService<TableReflectionService>();
// var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();
var kxpContext = scope.ServiceProvider.GetRequiredService<IDbContextFactory<KxpContext>>().CreateDbContext();

var json = @"{
    ""editableAreas"": [
    {
    ""identifier"": ""ContactUs"",
    ""sections"": [
    {
        ""identifier"": ""737e5d34-f1de-42bb-a4df-588f4dffb9b6"",
        ""type"": ""DancingGoat.SingleColumnSection"",
        ""properties"": {
            ""theme"": null
        },
        ""zones"": [
        {
            ""identifier"": ""a78772f4-9850-464d-8f6a-0400e71a6b05"",
            ""widgets"": [
            {
                ""identifier"": ""ccd51ab9-b7e2-4a93-a5c6-172cb8e270b7"",
                ""type"": ""Kentico.FormWidget"",
                ""variants"": [
                {
                    ""identifier"": ""77a99ac8-d93f-4cf9-bfb1-45b16f5dedcf"",
                    ""properties"": {
                        ""selectedForm"": ""DancingGoatCoreContactUsNew"",
                        ""anotherf"": ""DancingGoatCoreContactUsNew""
                    }
                }
                ]
            }
            ]
        }
        ]
    }
    ]
}
]
}";


// var editableAreas = JsonConvert.DeserializeObject<EditableAreasConfiguration>(json);
//
// foreach (var area in editableAreas.EditableAreas)
// {
//     foreach (var section in area.Sections)
//     {
//         // TODO tomas.krch: 2022-09-12 section properties
//         // section.Properties
//         Console.WriteLine($"{section.TypeIdentifier}");
//         Console.WriteLine($"{section.Properties}");
//
//         foreach (var zone in section.Zones)
//         {
//             foreach (var widget in zone.Widgets)
//             {
//                 Console.WriteLine($"{widget.TypeIdentifier}");
//                 foreach (var widgetVariant in widget.Variants)
//                 {
//                     // TODO tomas.krch: 2022-09-12 widgetVariant.Properties
//                     Console.WriteLine($"{widgetVariant.Properties}");
//                 }
//             }
//         }
//     }
// }


var jo = JObject.Parse(json);

var allProperties = jo.SelectTokens("$..properties").ToList();
foreach (var properties in allProperties)
{
    Console.WriteLine("-----------------------");
    var propertiesContainer = properties.Parent;
    Console.WriteLine($"{propertiesContainer}");
    
    var propsOf = propertiesContainer.Parent.Parent.Parent;
    Console.WriteLine($"{propsOf}");
    
    var c = 1;
}
// dynamic dynJson = JsonConvert.DeserializeObject(json);
// foreach (var item in dynJson)
// {
//     Console.WriteLine("{0} {1} {2} {3}\n", item.id, item.displayName, 
//         item.slug, item.imageUrl);
// }


// var sb = new StringBuilder();
// GenHelper.AppendFieldMappingDefinitionAsMarkdown(sb);
// var def = sb.ToString();
// Console.WriteLine(def);

// var countryMigrator = scope.ServiceProvider.GetRequiredService<CountryMigrator>();
//
// countryMigrator.MigrateCountriesAndStates();

// var classService = scope.ServiceProvider.GetRequiredService<ClassService>();
// var classFields = classService.GetClassFields(new Guid("C1C4DEDA-9280-436C-9BF7-F1A0C706EC80")); // custom.News
//
// foreach (var classColumnModel in classFields)
// {
//     logger.LogInformation("{classColumnModel}", classColumnModel);
// }
//
// kxpContext.Dispose();
