using System.Linq.Expressions;

namespace Migration.Tool.Source.Services;

public record SourceTargetKeyMapping(int SourceId, int TargetId);

public interface IPrimaryKeyLocatorService
{
    bool TryLocate<T>(Expression<Func<T, object>> keyNameSelector, int sourceId, out int targetId);
    IEnumerable<SourceTargetKeyMapping> SelectAll<T>(Expression<Func<T, object>> keyNameSelector);
}
