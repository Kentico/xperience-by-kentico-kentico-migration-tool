using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.KX12.Context;

namespace Migration.Tool.Core.KX12.Services;

public class KeyLocatorService(
    ILogger<KeyLocatorService> logger,
    IDbContextFactory<KX12Context> kx12ContextFactory)
{
    public bool TryLocate<TSource, TTarget, TTargetKey>(
        Expression<Func<TSource, object>> sourceKeySelector,
        Func<TTarget, TTargetKey> targetKeySelector,
        Expression<Func<TSource, Guid>> sourceGuidSelector,
        Func<Guid, TTarget?> targetByGuidProvider,
        object? sourceKey, out TTargetKey targetId
    ) where TSource : class where TTarget : class
    {
        using var kx12Context = kx12ContextFactory.CreateDbContext();

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
            var k12Guid = kx12Context.Set<TSource>().Where(sourcePredicate).Select(sourceGuidSelector).Single();

            var target = targetByGuidProvider(k12Guid);
            if (target is null)
            {
                logger.LogWarning("Mapping {SourceFullType} primary key: {SourceId} failed, GUID {TargetGUID} not present in target instance", sourceType.FullName, sourceKey, k12Guid);
                return false;
            }

            targetId = targetKeySelector(target);
            return true;
        }
        catch (InvalidOperationException ioex)
        {
            logger.LogWarning("Mapping {SourceFullType} primary key: {SourceId} failed, {Message}", sourceType.FullName, sourceKey, ioex.Message);
            return false;
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
        using var kx12Context = kx12ContextFactory.CreateDbContext();

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
            guid = kx12Context.Set<T>().Where(sourcePredicate).Select(guidSelector).Single();
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
