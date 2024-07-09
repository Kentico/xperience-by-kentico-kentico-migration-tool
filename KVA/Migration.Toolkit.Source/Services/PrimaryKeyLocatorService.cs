namespace Migration.Toolkit.Source.Services;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Services;
using Migration.Toolkit.KXP.Context;
using Migration.Toolkit.Source.Model;

public class PrimaryKeyLocatorService(
    ILogger<PrimaryKeyLocatorService> logger,
    IDbContextFactory<KxpContext> kxpContextFactory,
    ModelFacade modelFacade
    ) : IPrimaryKeyLocatorService
{
    private class KeyEqualityComparerWithLambda<T> : IEqualityComparer<T>
    {
        private readonly Func<T?, T?, bool> _equalityComparer;

        public KeyEqualityComparerWithLambda(Func<T?,T?,bool> equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public bool Equals(T? x, T? y) => _equalityComparer.Invoke(x, y);

        public int GetHashCode(T obj) => obj?.GetHashCode() ?? 0;
    }

    private record CmsUserKey(Guid UserGuid, string UserName);

    public IEnumerable<SourceTargetKeyMapping> SelectAll<T>(Expression<Func<T, object>> keyNameSelector)
    {
        using var kxpContext = kxpContextFactory.CreateDbContext();

        var sourceType = typeof(T);
        var memberName = keyNameSelector.GetMemberName();

        logger.LogTrace("Preload of entity {Entity} member {MemberName} mapping requested", sourceType.Name, memberName);

        if (sourceType == typeof(ICmsUser) && memberName == nameof(ICmsUser.UserID))
        {
            var sourceUsers = modelFacade.SelectAll<ICmsUser>().ToList();
            var targetUsers = kxpContext.CmsUsers.Select(x => new { x.UserId, x.UserName, x.UserGuid }).ToList();

            var result = sourceUsers.Join(targetUsers,
                a => new CmsUserKey(a.UserGUID, a.UserName),
                b => new CmsUserKey(b.UserGuid, b.UserName),
                (a, b) => new SourceTargetKeyMapping(a.UserID, b.UserId),
                new KeyEqualityComparerWithLambda<CmsUserKey>((ak, bk) => (ak?.UserGuid == bk?.UserGuid || ak?.UserName == bk?.UserName) && ak != null && bk != null)
            );

            foreach (var resultingMapping in result)
            {
                yield return resultingMapping;
            }

            yield break;
        }

        if (sourceType == typeof(IOmContact) && memberName == nameof(IOmContact.ContactID))
        {
            var source = modelFacade.SelectAll<IOmContact>()
                .OrderBy(c => c.ContactCreated)
                .Select(x => new { x.ContactID, x.ContactGUID }).ToList();
            var target = kxpContext.OmContacts
                .OrderBy(c => c.ContactCreated)
                .Select(x => new { x.ContactId, x.ContactGuid }).ToList();

            var result = source.Join(target,
                a => a.ContactGUID,
                b => b.ContactGuid,
                (a, b) => new SourceTargetKeyMapping(a.ContactID, b.ContactId)
            );

            foreach (var resultingMapping in result)
            {
                yield return resultingMapping;
            }

            yield break;
        }

//         if (sourceType == typeof(ICmsTree) && memberName == nameof(ICmsTree.NodeID))
//         {
// #error "NodeGuid may not be unique, use other means of searching for node!"
//             var source = modelFacade.SelectAll<ICmsTree>().Select(x => new { x.NodeID, x.NodeGUID, x.NodeSiteID }).ToList();
//             var target = kxpContext.CmsChannels.Select(x => new { x.ChannelId, x.ChannelGuid }).ToList();
//
//             var result = source.Join(target,
// #error "NodeGuid may not be unique, use other means of searching for node!"
//                 a => a.NodeGUID,
//                 b => b.ChannelGuid,
//                 (a, b) => new SourceTargetKeyMapping(a.NodeID, b.ChannelId)
//             );
//
//             foreach (var resultingMapping in result)
//             {
//                 yield return resultingMapping;
//             }
//
//             yield break;
//         }

        if (sourceType == typeof(ICmsState) && memberName == nameof(ICmsState.StateID))
        {
            var source = modelFacade.SelectAll<ICmsState>().Select(x => new { x.StateID, x.StateName }).ToList();
            var target = kxpContext.CmsStates.Select(x => new {  x.StateId, x.StateName }).ToList();

            var result = source.Join(target,
                a => a.StateName,
                b => b.StateName,
                (a, b) => new SourceTargetKeyMapping(a.StateID, b.StateId)
            );

            foreach (var resultingMapping in result)
            {
                yield return resultingMapping;
            }

            yield break;
        }

        if (sourceType == typeof(ICmsCountry) && memberName == nameof(ICmsCountry.CountryID))
        {
            var source = modelFacade.SelectAll<ICmsCountry>().Select(x => new { x.CountryID, x.CountryName }).ToList();
            var target = kxpContext.CmsCountries.Select(x => new {  x.CountryId, x.CountryName }).ToList();

            var result = source.Join(target,
                a => a.CountryName,
                b => b.CountryName,
                (a, b) => new SourceTargetKeyMapping(a.CountryID, b.CountryId)
            );

            foreach (var resultingMapping in result)
            {
                yield return resultingMapping;
            }

            yield break;
        }



        throw new NotImplementedException();
    }

    public bool TryLocate<T>(Expression<Func<T, object>> keyNameSelector, int sourceId, out int targetId)
    {
        using var kxpContext = kxpContextFactory.CreateDbContext();

        var sourceType = typeof(T);
        targetId = -1;
        try
        {
            if (sourceType == typeof(ICmsResource))
            {
                var sourceGuid = modelFacade.SelectById<ICmsResource>(sourceId)?.ResourceGUID;
                targetId = kxpContext.CmsResources.Where(x => x.ResourceGuid == sourceGuid).Select(x => x.ResourceId).Single();
                return true;
            }

            if (sourceType == typeof(ICmsClass))
            {
                var sourceGuid = modelFacade.SelectById<ICmsClass>(sourceId)?.ClassGUID;
                targetId = kxpContext.CmsClasses.Where(x => x.ClassGuid == sourceGuid).Select(x => x.ClassId).Single();
                return true;
            }

            if (sourceType == typeof(ICmsUser))
            {
                var source = modelFacade.SelectById<ICmsUser>(sourceId);
                targetId = kxpContext.CmsUsers.Where(x => x.UserGuid == source.UserGUID || x.UserName == source.UserName).Select(x => x.UserId).Single();
                return true;
            }

            if (sourceType == typeof(ICmsRole))
            {
                var sourceGuid = modelFacade.SelectById<ICmsRole>(sourceId)?.RoleGUID;
                targetId = kxpContext.CmsRoles.Where(x => x.RoleGuid == sourceGuid).Select(x => x.RoleId).Single();
                return true;
            }

            if (sourceType == typeof(ICmsSite))
            {
                var sourceGuid = modelFacade.SelectById<ICmsSite>(sourceId)?.SiteGUID;
                targetId = kxpContext.CmsChannels.Where(x => x.ChannelGuid == sourceGuid).Select(x => x.ChannelId).Single();
                return true;
            }

            if (sourceType == typeof(ICmsState))
            {
                var sourceName = modelFacade.SelectById<ICmsState>(sourceId)?.StateName;
                targetId = kxpContext.CmsStates.Where(x => x.StateName == sourceName).Select(x => x.StateId).Single();
                return true;
            }

            if (sourceType == typeof(ICmsCountry))
            {
                var sourceName = modelFacade.SelectById<ICmsCountry>(sourceId)?.CountryName;
                targetId = kxpContext.CmsCountries.Where(x => x.CountryName == sourceName).Select(x => x.CountryId).Single();
                return true;
            }

            if (sourceType == typeof(IOmContactStatus))
            {
                var sourceName = modelFacade.SelectById<IOmContactStatus>(sourceId)?.ContactStatusName;
                targetId = kxpContext.OmContactStatuses.Where(x => x.ContactStatusName == sourceName).Select(x => x.ContactStatusId).Single();
                return true;
            }

            if (sourceType == typeof(IOmContact))
            {
                var sourceGuid = modelFacade.SelectById<IOmContact>(sourceId)?.ContactGUID;
                targetId = kxpContext.OmContacts.Where(x => x.ContactGuid == sourceGuid).Select(x => x.ContactId).Single();
                return true;
            }

//             if (sourceType == typeof(ICmsTree))
//             {
// #error "NodeGuid may not be unique, use other means of searching for node!"
//                 var sourceGuid = modelFacade.SelectById<ICmsTree>(sourceId)?.NodeGUID;
//                 targetId = kxpContext.CmsChannels.Where(x => x.ChannelGuid == sourceGuid).Select(x => x.ChannelId).Single();
//                 return true;
//             }
        }
        catch (InvalidOperationException ioex)
        {
            if (ioex.Message.StartsWith("Sequence contains no elements"))
            {
                logger.LogDebug("Mapping {SourceFullType} primary key: {SourceId} failed, {Message}", sourceType.FullName, sourceId, ioex.Message);
            }
            else
            {
                logger.LogWarning("Mapping {SourceFullType} primary key: {SourceId} failed, {Message}", sourceType.FullName, sourceId, ioex.Message);
            }
            return false;
        }
        finally
        {
            if (targetId != -1)
            {
                logger.LogTrace("Mapping {SourceFullType} primary key: {SourceId} to {TargetId}", sourceType.FullName, sourceId, targetId);
            }
        }

        logger.LogError("Mapping {SourceFullType} primary key is not supported", sourceType.FullName);
        targetId = -1;
        return false;
    }
}