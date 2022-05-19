using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;

namespace Migration.Toolkit.Core.MigrationProtocol;

public class DebugMigrationProtocol: IMigrationProtocol
{
    private readonly ILogger<DebugMigrationProtocol> _logger;

    public DebugMigrationProtocol(ILogger<DebugMigrationProtocol> logger)
    {
        _logger = logger;
    }
    
    public void NeedsManualAction<TSource, TTarget>(HandbookReference handbookRef, string whatNeedsToBeDoneOrWhatHappened, TSource source, TTarget? target,
        ModelMappingResult<TTarget> mapped)
    {
        
    }

    public void MappedTarget<TTarget>(ModelMappingResult<TTarget> mapped)
    {
        
    }

    public void FetchedTarget<TTarget>(TTarget? target)
    {
        _logger.LogDebug("FetchedTarget: {type}: {source}", typeof(TTarget).FullName, target);
    }

    public void FetchedSource<TSource>(TSource? source)
    {
        _logger.LogDebug("FetchedSource: {type}: {source}", typeof(TSource).FullName, source);
    }

    public void Success<TSource, TTarget>(TSource source, TTarget target, ModelMappingResult<TTarget> mapped)
    {
        throw new NotImplementedException();
    }

    public IDisposable CreateScope<TScopeType>()
    {
        return new DummyDisposable();
    }
    
    private class DummyDisposable: IDisposable
    {
        public void Dispose() { }
    }

    public void Warning<T>(HandbookReference handbookRef, T? entity)
    {
        
    }

    public void Warning<TSource, TTarget>(HandbookReference handbookRef, TSource? source, TTarget? target)
    {
        
    }

    public void Fatal<T>(HandbookReference handbookRef, T? entity)
    {
        
    }
}