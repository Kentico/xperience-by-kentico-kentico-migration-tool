using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Services.Ipc;
using Migration.Toolkit.Source.Contexts;

namespace Migration.Toolkit.Source.Services;

public class ModuleLoader(
    IpcService ipc,
    SourceInstanceContext sourceInstanceContext,
    ILogger<ModuleLoader> logger
)
    : IModuleLoader
{
    public async Task LoadAsync()
    {
        try
        {
            if (sourceInstanceContext.IsQuerySourceInstanceEnabled())
            {
                bool ipcConfigured = await ipc.IsConfiguredAsync();
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
