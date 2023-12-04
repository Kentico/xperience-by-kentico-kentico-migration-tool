namespace Migration.Toolkit.Common;

using System.Linq.Expressions;

public record MapSourceIdResult(bool Success, int? MappedId);

public interface IPrimaryKeyMappingContext
{
    void SetMapping(Type type, string keyName, int sourceId, int targetId);
    void SetMapping<T>(Expression<Func<T, object>> keyNameSelector, int sourceId, int targetId);
    int RequireMapFromSource<T>(Expression<Func<T, object>> keyNameSelector, int sourceId);
    bool TryRequireMapFromSource<T>(Expression<Func<T, object>> keyNameSelector, int? sourceId, out int targetIdResult);
    int? MapFromSource<T>(Expression<Func<T, object>> keyNameSelector, int? sourceId);
    int? MapFromSourceOrNull<T>(Expression<Func<T, object>> keyNameSelector, int? sourceId);
    MapSourceIdResult MapSourceId<T>(Expression<Func<T, object>> keyNameSelector, int? sourceId, bool useLocator = true);
    void PreloadDependencies<T>(Expression<Func<T, object>> keyNameSelector);
    bool HasMapping<T>(Expression<Func<T, object>> keyNameSelector, int? sourceId, bool useLocator = true);
}
