namespace Migration.Toolkit.Core.Contexts;

using System.Linq.Expressions;
using Migration.Toolkit.Core.Services;

public record MapSourceKeyResult<TMapped>(bool Success, TMapped? Mapped);

public class KeyMappingContext
{
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly KeyLocatorService _keyLocatorService;

    public KeyMappingContext(PrimaryKeyMappingContext primaryKeyMappingContext, KeyLocatorService keyLocatorService)
    {
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _keyLocatorService = keyLocatorService;
    }

    public MapSourceKeyResult<TTargetKey> MapSourceKey<TSource, TTarget, TTargetKey>(Expression<Func<TSource, object>> sourceKeySelector,
        Expression<Func<TSource, Guid>> sourceGuidSelector,
        object? sourceKey,
        Expression<Func<TTarget, TTargetKey>> targetKeySelector,
        Expression<Func<TTarget, Guid>> targetGuidSelector) where TSource : class where TTarget : class
    {
        if (sourceKey is int id && _primaryKeyMappingContext.MapSourceId<TSource>(sourceKeySelector, id) is {Success:true, MappedId: TTargetKey targetKey })
        {
            return new MapSourceKeyResult<TTargetKey>(true, targetKey);
        }

        if (_keyLocatorService.TryLocate(sourceKeySelector, targetKeySelector, sourceGuidSelector, targetGuidSelector, sourceKey, out var located))
        {
            return new MapSourceKeyResult<TTargetKey>(true, located);
        }

        return new MapSourceKeyResult<TTargetKey>(false, default);
    }

    public MapSourceKeyResult<Guid?> GetGuid<T>(Expression<Func<T, object>> keySelector, Expression<Func<T, Guid>> guidSelector, object? key) where T : class =>
        _keyLocatorService.TryGetSourceGuid(keySelector, guidSelector, key, out var located)
            ? new MapSourceKeyResult<Guid?>(true, located)
            : new MapSourceKeyResult<Guid?>(false, null);
}