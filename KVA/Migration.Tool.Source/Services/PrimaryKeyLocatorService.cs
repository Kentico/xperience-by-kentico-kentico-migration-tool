using System.Linq.Expressions;
using CMS.ContactManagement;
using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.Globalization;
using CMS.Membership;
using CMS.Modules;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Services;

public class PrimaryKeyLocatorService(
    ILogger<PrimaryKeyLocatorService> logger,
    ModelFacade modelFacade
) : IPrimaryKeyLocatorService
{
    public IEnumerable<SourceTargetKeyMapping> SelectAll<T>(Expression<Func<T, object>> keyNameSelector)
    {
        var sourceType = typeof(T);
        string memberName = keyNameSelector.GetMemberName();

        logger.LogTrace("Preload of entity {Entity} member {MemberName} mapping requested", sourceType.Name, memberName);

        if (sourceType == typeof(ICmsUser) && memberName == nameof(ICmsUser.UserID))
        {
            var sourceUsers = modelFacade.SelectAll<ICmsUser>().ToList();
            var targetUsers = UserInfo.Provider.Get().Select(x => new { x.UserID, x.UserName, x.UserGUID }).ToList();

            var result = sourceUsers.Join(targetUsers,
                a => new CmsUserKey(a.UserGUID, a.UserName),
                b => new CmsUserKey(b.UserGUID, b.UserName),
                (a, b) => new SourceTargetKeyMapping(a.UserID, b.UserID),
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
            var target = ContactInfo.Provider.Get().OrderBy(nameof(ContactInfo.ContactCreated))
                .Select(x => new { x.ContactID, x.ContactGUID }).ToList();

            var result = source.Join(target,
                a => a.ContactGUID,
                b => b.ContactGUID,
                (a, b) => new SourceTargetKeyMapping(a.ContactID, b.ContactID)
            );

            foreach (var resultingMapping in result)
            {
                yield return resultingMapping;
            }

            yield break;
        }

        if (sourceType == typeof(ICmsState) && memberName == nameof(ICmsState.StateID))
        {
            var source = modelFacade.SelectAll<ICmsState>().Select(x => new { x.StateID, x.StateName }).ToList();
            var target = StateInfo.Provider.Get().Select(x => new { x.StateID, x.StateName }).ToList();

            var result = source.Join(target,
                a => a.StateName,
                b => b.StateName,
                (a, b) => new SourceTargetKeyMapping(a.StateID, b.StateID)
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
            var target = CountryInfo.Provider.Get().Select(x => new { x.CountryID, x.CountryName }).ToList();

            var result = source.Join(target,
                a => a.CountryName,
                b => b.CountryName,
                (a, b) => new SourceTargetKeyMapping(a.CountryID, b.CountryID)
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
        var sourceType = typeof(T);
        targetId = -1;
        try
        {
            if (sourceType == typeof(ICmsResource))
            {
                var sourceGuid = modelFacade.SelectById<ICmsResource>(sourceId)?.ResourceGUID;
                targetId = ResourceInfo.Provider.Get().WhereEquals(nameof(ResourceInfo.ResourceGUID), sourceGuid).Select(x => x.ResourceID).Single();
                return true;
            }

            if (sourceType == typeof(ICmsClass))
            {
                var sourceGuid = modelFacade.SelectById<ICmsClass>(sourceId)?.ClassGUID;
                targetId = DataClassInfoProvider.GetClasses().Where(x => x.ClassGUID == sourceGuid).Select(x => x.ClassID).Single();
                return true;
            }

            if (sourceType == typeof(ICmsUser))
            {
                var source = modelFacade.SelectById<ICmsUser>(sourceId);
                targetId = UserInfo.Provider.Get().WhereEquals(nameof(UserInfo.UserGUID), source.UserGUID).Or().WhereEquals(nameof(UserInfo.UserName), source.UserName).Select(x => x.UserID).Single();
                return true;
            }

            if (sourceType == typeof(ICmsRole))
            {
                var sourceGuid = modelFacade.SelectById<ICmsRole>(sourceId)?.RoleGUID;
                targetId = RoleInfo.Provider.Get().WhereEquals(nameof(RoleInfo.RoleGUID), sourceGuid).Select(x => x.RoleID).Single();
                return true;
            }

            if (sourceType == typeof(ICmsSite))
            {
                var sourceGuid = modelFacade.SelectById<ICmsSite>(sourceId)?.SiteGUID;
                targetId = ChannelInfo.Provider.Get().WhereEquals(nameof(ChannelInfo.ChannelGUID), sourceGuid).Select(x => x.ChannelID).Single();
                return true;
            }

            if (sourceType == typeof(ICmsState))
            {
                string? sourceName = modelFacade.SelectById<ICmsState>(sourceId)?.StateName;
                targetId = StateInfo.Provider.Get().WhereEquals(nameof(StateInfo.StateName), sourceName).Select(x => x.StateID).Single();
                return true;
            }

            if (sourceType == typeof(ICmsCountry))
            {
                string? sourceName = modelFacade.SelectById<ICmsCountry>(sourceId)?.CountryName;
                targetId = CountryInfo.Provider.Get().WhereEquals(nameof(CountryInfo.CountryName), sourceName).Select(x => x.CountryID).Single();
                return true;
            }

            if (sourceType == typeof(IOmContactStatus))
            {
                string? sourceName = modelFacade.SelectById<IOmContactStatus>(sourceId)?.ContactStatusName;
                targetId = ContactStatusInfo.Provider.Get().WhereEquals(nameof(ContactStatusInfo.ContactStatusName), sourceName).Select(x => x.ContactStatusID).Single();
                return true;
            }

            if (sourceType == typeof(IOmContact))
            {
                var sourceGuid = modelFacade.SelectById<IOmContact>(sourceId)?.ContactGUID;
                targetId = ContactInfo.Provider.Get().WhereEquals(nameof(ContactInfo.ContactGUID), sourceGuid).Select(x => x.ContactID).Single();
                return true;
            }
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

    private class KeyEqualityComparerWithLambda<T>(Func<T?, T?, bool> equalityComparer) : IEqualityComparer<T>
    {
        public bool Equals(T? x, T? y) => equalityComparer.Invoke(x, y);

        public int GetHashCode(T obj) => obj?.GetHashCode() ?? 0;
    }

    private record CmsUserKey(Guid UserGuid, string UserName);
}
