using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using CMS.ContactManagement;
using CMS.DataEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.K11;

namespace Migration.Tool.Core.K11.Services;

public class KeyLocatorService(
    ILogger<KeyLocatorService> logger,
    IDbContextFactory<K11Context> k11ContextFactory)
{
    /// <summary>
    /// Finds key of target matched to source by equal GUIDs
    /// </summary>
    /// <typeparam name="TSource">Type of source instance</typeparam>
    /// <typeparam name="TTarget">Type of target instance</typeparam>
    /// <typeparam name="TTargetKey">Type of target key</typeparam>
    /// <param name="sourceKeySelector">Expression that selects key from TSource instance</param>
    /// <param name="targetKeySelector">Expression that selects key from TTarget instance</param>
    /// <param name="sourceGuidSelector">Expression that selects GUID from TSource instance</param>
    /// <param name="targetByGuidProvider">Func that returns TTarget instance uniquely identified by its GUID. In case of multiple GUIDs in target dataset, it must return null</param>
    /// <param name="sourceKey">Source key to begin with</param>
    /// <param name="targetId">Matched target key</param>
    /// <returns></returns>
    public bool TryLocate<TSource, TTarget, TTargetKey>(
        Expression<Func<TSource, object>> sourceKeySelector,
        Func<TTarget, TTargetKey> targetKeySelector,
        Expression<Func<TSource, Guid>> sourceGuidSelector,
        Func<Guid, TTarget?> targetByGuidProvider,
        object? sourceKey, out TTargetKey targetId
    ) where TSource : class where TTarget : class
    {
        using var k11Context = k11ContextFactory.CreateDbContext();
        var sourceType = typeof(TSource);
        Unsafe.SkipInit(out targetId);

        try
        {
            if (sourceKey is null)
            {
                return false;
            }

            var sourceEquals = Expression.Equal(
                sourceKeySelector.Body,
                Expression.Convert(Expression.Constant(sourceKey, sourceKey.GetType()), typeof(object))
            );
            var sourcePredicate = Expression.Lambda<Func<TSource, bool>>(sourceEquals, sourceKeySelector.Parameters[0]);
            var k11Guid = k11Context.Set<TSource>().Where(sourcePredicate).Select(sourceGuidSelector).Single();

            var target = targetByGuidProvider(k11Guid);
            if (target is null)
            {
                logger.LogWarning("Mapping {SourceFullType} primary key: {SourceId} failed, GUID {TargetGUID} not present in target instance", sourceType.FullName, sourceKey, k11Guid);
                return false;
            }

            targetId = targetKeySelector(target);
            return true;
        }
        finally
        {
            if (!targetId?.Equals(default) ?? false)
            {
                logger.LogTrace("Mapping {SourceFullType} primary key: {SourceId} to {TargetId}", sourceType.FullName, sourceKey, targetId);
            }
        }
    }

    public bool TryGetSourceGuid<T>(Expression<Func<T, object>> keySelector, Expression<Func<T, Guid>> guidSelector, object? key, out Guid? guid)
        where T : class
    {
        using var KX12Context = k11ContextFactory.CreateDbContext();

        var type = typeof(T);
        Unsafe.SkipInit(out guid);

        try
        {
            if (key is null)
            {
                return false;
            }

            var sourceEquals = Expression.Equal(
                keySelector.Body,
                Expression.Convert(Expression.Constant(key, key.GetType()), typeof(object))
            );
            var sourcePredicate = Expression.Lambda<Func<T, bool>>(sourceEquals, keySelector.Parameters[0]);
            guid = KX12Context.Set<T>().Where(sourcePredicate).Select(guidSelector).Single();
            return true;
        }
        catch (InvalidOperationException ioex)
        {
            logger.LogWarning("Guid locator {SourceFullType} primary key: {Key} failed, {Message}", type.FullName, key, ioex.Message);
            return false;
        }
        finally
        {
            if (!guid?.Equals(default) ?? false)
            {
                logger.LogTrace("Guid locator {SourceFullType} primary key: {Key} located {Guid}", type.FullName, key, guid);
            }
        }
    }
}
