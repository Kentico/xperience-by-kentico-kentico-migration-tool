using CMS.DataEngine;
using Microsoft.Extensions.Logging;

namespace Migration.Toolkit.KXO.Api;

public class KxoClassFacade
{
    private readonly ILogger<KxoClassFacade> _logger;
    private readonly KxoApiInitializer _kxoApiInitializer;

    public KxoClassFacade(ILogger<KxoClassFacade> logger, KxoApiInitializer kxoApiInitializer)
    {
        _logger = logger;
        _kxoApiInitializer = kxoApiInitializer;

        _kxoApiInitializer.EnsureApiIsInitialized();
    }
    
    public void SetClass(DataClassInfo dataClassInfo)
    {
        // TODO tk: 2022-05-30  dataClassInfo.ClassConnectionString - check, might cause unexpected behavior
        
        DataClassInfoProvider.SetDataClassInfo(dataClassInfo);
    }

    public DataClassInfo GetClass(Guid classGuid)
    {
        return DataClassInfoProvider.GetDataClassInfo(classGuid);
    }
}