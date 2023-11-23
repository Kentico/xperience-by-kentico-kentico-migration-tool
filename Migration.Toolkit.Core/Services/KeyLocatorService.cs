namespace Migration.Toolkit.Core.Services;

using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KXP.Context;

public class KeyLocatorService
{
    private readonly ILogger<KeyLocatorService> _logger;
    private readonly IDbContextFactory<KxpContext> _kxpContextFactory;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;

    public KeyLocatorService(
        ILogger<KeyLocatorService> logger,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory
    )
    {
        _logger = logger;
        _kxpContextFactory = kxpContextFactory;
        _kx13ContextFactory = kx13ContextFactory;
    }

    public bool TryLocate<TSource, TTarget, TTargetKey>(
        Expression<Func<TSource, object>> sourceKeySelector,
        Expression<Func<TTarget, TTargetKey>> targetKeySelector,
        Expression<Func<TSource, Guid>> sourceGuidSelector,
        Expression<Func<TTarget, Guid>> targetGuidSelector,
        object? sourceKey, out TTargetKey targetId
    ) where TSource : class where TTarget: class
    {
        using var kxpContext = _kxpContextFactory.CreateDbContext();
        using var kx13Context = _kx13ContextFactory.CreateDbContext();

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
            var kx13Guid = kx13Context.Set<TSource>().Where(sourcePredicate).Select(sourceGuidSelector).Single();

            var param = Expression.Parameter(typeof(TTarget), "t");
            var member = targetGuidSelector.Body as MemberExpression;
            if (member == null)
            {
                throw new InvalidOperationException($"Expression SHALL NOT be other than member expression, expression: {targetGuidSelector}");
            }
            var targetEquals = Expression.Equal(
                Expression.MakeMemberAccess(param, member.Member),
                Expression.Constant(kx13Guid, typeof(Guid))
            );
            var targetPredicate = Expression.Lambda<Func<TTarget, bool>>(targetEquals, param);

            var query = kxpContext.Set<TTarget>().Where(targetPredicate);
            var selector = Expression.Lambda<Func<TTarget, TTargetKey>>(targetKeySelector.Body, targetKeySelector.Parameters[0]);
            targetId = query.Select(selector).Single();
            return true;
        }
        catch (InvalidOperationException ioex)
        {
            _logger.LogWarning("Mapping {SourceFullType} primary key: {SourceId} failed, {Message}", sourceType.FullName, sourceKey, ioex.Message);
            return false;
        }
        finally
        {
            if (!targetId?.Equals(default) ?? false)
            {
                _logger.LogTrace("Mapping {SourceFullType} primary key: {SourceId} to {TargetId}", sourceType.FullName, sourceKey, targetId);
            }
        }
    }

    public bool TryGetSourceGuid<T>(Expression<Func<T,object>> keySelector, Expression<Func<T,Guid>> guidSelector, object? key, out Guid? guid)
        where T : class
    {
        using var kx13Context = _kx13ContextFactory.CreateDbContext();

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
            guid = kx13Context.Set<T>().Where(sourcePredicate).Select(guidSelector).Single();
            return true;
        }
        catch (InvalidOperationException ioex)
        {
            _logger.LogWarning("Guid locator {SourceFullType} primary key: {Key} failed, {Message}", type.FullName, key, ioex.Message);
            return false;
        }
        finally
        {
            if (!guid?.Equals(default) ?? false)
            {
                _logger.LogTrace("Guid locator {SourceFullType} primary key: {Key} located {Guid}", type.FullName, key, guid);
            }
        }
    }
}