using CMS.DataEngine;
using Microsoft.Extensions.Logging;

namespace Migration.Toolkit.KXO.Api;

public class KxoApiInitializer
{
    private readonly ILogger<KxoApiInitializer> _logger;

    public KxoApiInitializer(ILogger<KxoApiInitializer> logger)
    {
        _logger = logger;
    }
    
    private bool apiInitializationCalled = false;
    
    public bool EnsureApiIsInitialized()
    {
        if (!apiInitializationCalled)
        {
            _logger.LogInformation("Kxo api initialization called");
            if (!CMSApplication.Init())
            {
                _logger.LogError("Kxo api initialization failed!");
                return false;
            }
            else
            {
                apiInitializationCalled = true;
                _logger.LogInformation("Kxo api initialization finished");    
            }
        }
        else
        {
            _logger.LogInformation("Kxo api initialization already called, skipping init");
        }

        return true;
    }
}