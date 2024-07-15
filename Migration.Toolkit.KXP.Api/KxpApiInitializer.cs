namespace Migration.Toolkit.KXP.Api;

using CMS.DataEngine;
using Microsoft.Extensions.Logging;

public class KxpApiInitializer(ILogger<KxpApiInitializer> logger)
{
    private bool _apiInitializationCalled = false;

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
            else
            {
                _apiInitializationCalled = true;
                logger.LogInformation("Kxp api initialization finished");
            }
        }
        else
        {
            logger.LogTrace("Kxp api initialization already called, skipping init");
        }

        return true;
    }
}