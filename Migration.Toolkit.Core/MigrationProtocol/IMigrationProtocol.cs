using Migration.Toolkit.Core.Abstractions;

namespace Migration.Toolkit.Core.MigrationProtocol;

public interface IMigrationProtocol
{
    void NeedsManualAction<TSource, TTarget>(HandbookReference handbookRef, string whatNeedsToBeDoneOrWhatHappened, TSource source, TTarget? target, ModelMappingResult<TTarget> mapped);
    void MappedTarget<TTarget>(ModelMappingResult<TTarget> mapped);
    void FetchedTarget<TTarget>(TTarget? target);
    void FetchedSource<TSource>(TSource? source);
    void Success<TSource, TTarget>(TSource source, TTarget target, ModelMappingResult<TTarget> mapped);
    IDisposable CreateScope<TScopeType>();
}