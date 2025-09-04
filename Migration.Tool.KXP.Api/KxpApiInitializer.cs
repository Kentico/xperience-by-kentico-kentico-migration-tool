using CMS.DataEngine;

using Microsoft.Extensions.Logging;

namespace Migration.Tool.KXP.Api;

public class KxpApiInitializer(ILogger<KxpApiInitializer> logger)
{
    private bool apiInitializationCalled;

    public bool EnsureApiIsInitialized()
    {
        if (!apiInitializationCalled)
        {
            logger.LogTrace("Kxp api initialization called");
            if (!CMSApplication.Init())
            {
                logger.LogError("Kxp api initialization failed!");
                return false;
            }

            apiInitializationCalled = true;
            logger.LogInformation("Kxp api initialization finished");
        }
        else
        {
            logger.LogTrace("Kxp api initialization already called, skipping init");
        }

        return true;
    }
}
