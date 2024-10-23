using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.K11;
using Migration.Tool.KXP.Context;

namespace Migration.Tool.Core.K11.Services;

public class KeyLocatorService(
    ILogger<KeyLocatorService> logger,
    IDbContextFactory<KxpContext> kxpContextFactory,
    IDbContextFactory<K11Context> k11ContextFactory)
{
    public bool TryLocate<TSource, TTarget, TTargetKey>(
        Expression<Func<TSource, object>> sourceKeySelector,
        Expression<Func<TTarget, TTargetKey>> targetKeySelector,
        Expression<Func<TSource, Guid>> sourceGuidSelector,
        Expression<Func<TTarget, Guid>> targetGuidSelector,
        object? sourceKey, out TTargetKey targetId
    ) where TSource : class where TTarget : class
    {
        using var kxpContext = kxpContextFactory.CreateDbContext();
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

            var param = Expression.Parameter(typeof(TTarget), "t");
            var member = targetGuidSelector.Body as MemberExpression ?? throw new InvalidOperationException($"Expression SHALL NOT be other than member expression, expression: {targetGuidSelector}");
            var targetEquals = Expression.Equal(
                Expression.MakeMemberAccess(param, member.Member),
                Expression.Constant(k11Guid, typeof(Guid))
            );
            var targetPredicate = Expression.Lambda<Func<TTarget, bool>>(targetEquals, param);

            var query = kxpContext.Set<TTarget>().Where(targetPredicate);
            var selector = Expression.Lambda<Func<TTarget, TTargetKey>>(targetKeySelector.Body, targetKeySelector.Parameters[0]);
            targetId = query.Select(selector).Single();
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
