namespace Migration.Toolkit.KXP.Api;

using CMS.DataEngine;
using Microsoft.Extensions.Logging;

public class KxpApiInitializer
{
    private readonly ILogger<KxpApiInitializer> _logger;

    public KxpApiInitializer(ILogger<KxpApiInitializer> logger)
    {
        _logger = logger;
    }

    private bool _apiInitializationCalled = false;

    public bool EnsureApiIsInitialized()
    {
        if (!_apiInitializationCalled)
        {
            _logger.LogInformation("Kxp api initialization called");
            if (!CMSApplication.Init())
            {
                _logger.LogError("Kxp api initialization failed!");
                return false;
            }
            else
            {
                _apiInitializationCalled = true;
                _logger.LogInformation("Kxp api initialization finished");
            }
        }
        else
        {
            _logger.LogInformation("Kxp api initialization already called, skipping init");
        }

        return true;
    }
}