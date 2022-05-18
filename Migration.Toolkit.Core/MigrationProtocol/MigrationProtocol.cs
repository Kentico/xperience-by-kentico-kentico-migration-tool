using Migration.Toolkit.Core.Abstractions;

namespace Migration.Toolkit.Core.MigrationProtocol;

public class NullMigrationProtocol: IMigrationProtocol
{
    // TODO tk: 2022-05-18 get configuration, protocol detail: All, Required
    
    public void NeedsManualAction<TSource, TTarget>(HandbookReference handbookRef, string whatNeedsToBeDoneOrWhatHappened, TSource source, TTarget? target, ModelMappingResult<TTarget> mapped)
    {
        // TODO tk: 2022-05-18 write to protocol that manual action needs to be done - HandbookReference is used to point into user handbook for further instructions
    }
    
    public void Warning<T>(HandbookReference handbookRef, T? entity)
    {
        // TODO tk: 2022-05-18 impl
    }

    public void Warning<TSource, TTarget>(HandbookReference handbookRef, TSource? source, TTarget? target)
    {
        // TODO tk: 2022-05-18 impl
    }

    public void MappedTarget<TTarget>(ModelMappingResult<TTarget> mapped)
    {
        // TODO tk: 2022-05-18 log serialized mapping result conditionally
    }

    public void FetchedTarget<TTarget>(TTarget? target)
    {
        // TODO tk: 2022-05-18 log serialized target fetch conditionally
    }

    public void FetchedSource<TSource>(TSource? source)
    {
        // TODO tk: 2022-05-18 log serialized source fetch conditionally
    }

    public void Success<TSource, TTarget>(TSource source, TTarget target, ModelMappingResult<TTarget> mapped)
    {
        // TODO tk: 2022-05-18 write successful migration of entity
    }

    public IDisposable CreateScope<TScopeType>()
    {
        // TODO tk: 2022-05-18 scoping of protocol
        return new DummyDisposable();
    }

    private class DummyDisposable: IDisposable
    {
        public void Dispose() { }
    }
}