using System.Collections.Frozen;
using CMS.DataEngine;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Helpers;

namespace Migration.Toolkit.Source.Auxiliary;

public interface ISourceGuidContext
{
    int[]? GetSiteIds<T>(Guid entityGuid) where T : ISourceGuidEntity;
}

public class SourceGuidContext(ModelFacade modelFacade, ILogger<ISourceGuidContext> logger) : ISourceGuidContext
{
    private static class Cache<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        public static FrozenDictionary<Guid, int[]>? SourceGuidSites;
    }

    public int[]? GetSiteIds<T>(Guid entityGuid) where T : ISourceGuidEntity
    {
        var cache = Cache<T>.SourceGuidSites ??= T.Load(modelFacade);
        int[]? result = cache.GetValueOrDefault(entityGuid);
        switch (result)
        {
            case { Length: 0 }:
            {
                logger.LogDebug("Guid {Guid} has no site at all", entityGuid);
                return [];
            }
            case { Length: 1 }:
            {
                return result;
            }
            case null:
            {
                logger.LogDebug("Guid {Guid} is not found at all", entityGuid);
                return null;
            }
            default:
            {
                logger.LogDebug("Guid {Guid} is present on multiple sites {Sites}", entityGuid, string.Join("|", result));
                return result;
            }
        }
    }
}

public record EntityIdentityGuidResult(bool IsFixed, Guid Identity);
public class EntityIdentityFacade(ISourceGuidContext sourceGuidContext)
{
    public EntityIdentityGuidResult Translate<T>(Guid guid, int? siteId) where T : ISourceGuidEntity =>
        sourceGuidContext.GetSiteIds<T>(guid) switch
        {
            // not found => leave it as it is or throw
            { Length: 0 } => new EntityIdentityGuidResult(false, guid),
            // OK! lets leave guid unchanged
            { Length: 1 } => new EntityIdentityGuidResult(false, guid),
            // BAD, guid is not unique, we cannot use it in XbyK
            _ => new EntityIdentityGuidResult(true, GuidV5.NewNameBased(T.NewGuidNs, $"{guid}|{siteId ?? -1}"))
        };

    public EntityIdentityGuidResult Translate<T>(T s) where T : ISourceGuidEntity =>
        s.GetIdentity() is var (guid, siteId)
            ? Translate<T>(guid, siteId)
            : throw new InvalidOperationException($"Unable to determine source entity identity: {s}");
}

public class IdentityLocator(EntityIdentityFacade entityIdentityFacade, ILogger<IdentityLocator> logger)
{
    public (TTarget target, bool guidIsFixed) LocateTarget<TTarget, TSource>(Guid entityGuid, int? siteId)
        where TTarget : AbstractInfoBase<TTarget>, IInfoWithGuid, new()
        where TSource : ISourceGuidEntity
    {
        (bool isFixed, var safeAttachmentGuid) = entityIdentityFacade.Translate<TSource>(entityGuid, siteId);
        var target = Provider<TTarget>.Instance.Get(safeAttachmentGuid);

        logger.LogTrace("Located {Target} from source {Source}",
            new { ID = target.Generalized.ObjectID, CodeName = target.Generalized.ObjectCodeName, type = target.TypeInfo.ObjectClassName },
            new { type = typeof(TSource).Name, entityGuid, siteId }
        );

        return (target, isFixed);
    }

    public (TTarget target, bool guidIsFixed) LocateTarget<TTarget, TSource>(TSource s)
        where TTarget : AbstractInfoBase<TTarget>, IInfoWithGuid, new()
        where TSource : ISourceGuidEntity
    {
        // TODO tomas.krch: 2024-07-23 specialized approach for CMS_USER and CMS_MEMBER

        (var entityGuid, int? siteId) = s.GetIdentity();
        return LocateTarget<TTarget, TSource>(entityGuid, siteId);
    }
}
