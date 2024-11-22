using System.Linq.Expressions;

using Migration.Tool.Core.KX12.Services;

namespace Migration.Tool.Core.KX12.Contexts;

public record MapSourceKeyResult<TMapped>(bool Success, TMapped? Mapped);

public class KeyMappingContext(PrimaryKeyMappingContext primaryKeyMappingContext, KeyLocatorService keyLocatorService)
{
    public MapSourceKeyResult<TTargetKey> MapSourceKey<TSource, TTarget, TTargetKey>(Expression<Func<TSource, object>> sourceKeySelector,
        Expression<Func<TSource, Guid>> sourceGuidSelector,
        object? sourceKey,
        Func<TTarget, TTargetKey> targetKeySelector,
        Func<Guid, TTarget?> targetByGuidProvider) where TSource : class where TTarget : class
    {
        if (sourceKey is int id && primaryKeyMappingContext.MapSourceId(sourceKeySelector, id) is { Success: true, MappedId: TTargetKey targetKey })
        {
            return new MapSourceKeyResult<TTargetKey>(true, targetKey);
        }

        if (keyLocatorService.TryLocate(sourceKeySelector, targetKeySelector, sourceGuidSelector, targetByGuidProvider, sourceKey, out var located))
        {
            return new MapSourceKeyResult<TTargetKey>(true, located);
        }

        return new MapSourceKeyResult<TTargetKey>(false, default);
    }

    public MapSourceKeyResult<Guid?> GetGuid<T>(Expression<Func<T, object>> keySelector, Expression<Func<T, Guid>> guidSelector, object? key) where T : class =>
        keyLocatorService.TryGetSourceGuid(keySelector, guidSelector, key, out var located)
            ? new MapSourceKeyResult<Guid?>(true, located)
            : new MapSourceKeyResult<Guid?>(false, null);
}
