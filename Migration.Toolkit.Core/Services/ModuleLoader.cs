namespace Migration.Toolkit.Core.Services;

using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Services.Ipc;
using Migration.Toolkit.Core.Contexts;

public class ModuleLoader(
    PrimaryKeyMappingContext mappingContext,
    ToolkitConfiguration configuration,
    TableReflectionService tableReflectionService,
    IpcService ipc,
    SourceInstanceContext sourceInstanceContext,
    ILogger<ModuleLoader> logger
)
    : IModuleLoader
{
    public async Task LoadAsync()
    {
        foreach (var (k, ek) in configuration.EntityConfigurations)
        {
            var tableType = tableReflectionService.GetSourceTableTypeByTableName(k);

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
            throw;
        }
    }
}