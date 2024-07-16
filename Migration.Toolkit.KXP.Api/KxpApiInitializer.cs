using CMS.DataEngine;

using Microsoft.Extensions.Logging;

namespace Migration.Toolkit.KXP.Api;

public class KxpApiInitializer(ILogger<KxpApiInitializer> logger)
{
    private bool _apiInitializationCalled;

    public bool EnsureApiIsInitialized()
    {
        if (!_apiInitializationCalled)
        {
            logger.LogTrace("Kxp api initialization called");
            if (!CMSApplication.Init())
            {
                logger.LogError("Kxp api initialization failed!");
                return false;
            }

            _apiInitializationCalled = true;
            logger.LogInformation("Kxp api initialization finished");
        }
        else
        {
            logger.LogTrace("Kxp api initialization already called, skipping init");
        }

        return true;
    }
}
