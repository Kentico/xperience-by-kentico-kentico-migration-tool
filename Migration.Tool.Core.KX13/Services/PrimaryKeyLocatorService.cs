using System.Linq.Expressions;
using CMS.ContactManagement;
using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.Globalization;
using CMS.Membership;
using CMS.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.KX13.Context;

namespace Migration.Tool.Core.KX13.Services;

public class PrimaryKeyLocatorService(
    ILogger<PrimaryKeyLocatorService> logger,
    IDbContextFactory<KX13Context> kx13ContextFactory)
    : IPrimaryKeyLocatorService
{
    public IEnumerable<SourceTargetKeyMapping> SelectAll<T>(Expression<Func<T, object>> keyNameSelector)
    {
        using var kx13Context = kx13ContextFactory.CreateDbContext();

        var sourceType = typeof(T);
        string memberName = keyNameSelector.GetMemberName();

        logger.LogTrace("Preload of entity {Entity} member {MemberName} mapping requested", sourceType.Name, memberName);

        if (sourceType == typeof(KX13M.CmsUser) && memberName == nameof(KX13M.CmsUser.UserId))
        {
            var sourceUsers = kx13Context.CmsUsers.Select(x => new { x.UserId, x.UserGuid, x.UserName }).ToList();
            var targetUsers = UserInfo.Provider.Get().Select(x => new { x.UserID, x.UserName, x.UserGUID }).ToList();

            var result = sourceUsers.Join(targetUsers,
                a => new CmsUserKey(a.UserGuid, a.UserName),
                b => new CmsUserKey(b.UserGUID, b.UserName),
                (a, b) => new SourceTargetKeyMapping(a.UserId, b.UserID),
                new KeyEqualityComparerWithLambda<CmsUserKey>((ak, bk) => (ak?.UserGuid == bk?.UserGuid || ak?.UserName == bk?.UserName) && ak != null && bk != null)
            );

            foreach (var resultingMapping in result)
            {
                yield return resultingMapping;
            }

            yield break;
        }

        if (sourceType == typeof(KX13M.OmContact) && memberName == nameof(KX13M.OmContact.ContactId))
        {
            var source = kx13Context.OmContacts
                .OrderBy(c => c.ContactCreated)
                .Select(x => new { x.ContactId, x.ContactGuid }).ToList();
            var target = ContactInfo.Provider.Get()
                .OrderBy(nameof(ContactInfo.ContactCreated))
                .Select(x => new { x.ContactID, x.ContactGUID }).ToList();

            var result = source.Join(target,
                a => a.ContactGuid,
                b => b.ContactGUID,
                (a, b) => new SourceTargetKeyMapping(a.ContactId, b.ContactID)
            );

            foreach (var resultingMapping in result)
            {
                yield return resultingMapping;
            }

            yield break;
        }

        if (sourceType == typeof(KX13M.CmsState) && memberName == nameof(KX13M.CmsState.StateId))
        {
            var source = kx13Context.CmsStates.Select(x => new { x.StateId, x.StateName }).ToList();
            var target = StateInfo.Provider.Get().Select(x => new { x.StateID, x.StateName }).ToList();

            var result = source.Join(target,
                a => a.StateName,
                b => b.StateName,
                (a, b) => new SourceTargetKeyMapping(a.StateId, b.StateID)
            );

            foreach (var resultingMapping in result)
            {
                yield return resultingMapping;
            }

            yield break;
        }

        if (sourceType == typeof(KX13M.CmsCountry) && memberName == nameof(KX13M.CmsCountry.CountryId))
        {
            var source = kx13Context.CmsCountries.Select(x => new { x.CountryId, x.CountryName }).ToList();
            var target = CountryInfo.Provider.Get().Select(x => new { x.CountryID, x.CountryName }).ToList();

            var result = source.Join(target,
                a => a.CountryName,
                b => b.CountryName,
                (a, b) => new SourceTargetKeyMapping(a.CountryId, b.CountryID)
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
        using var kx13Context = kx13ContextFactory.CreateDbContext();

        var sourceType = typeof(T);
        targetId = -1;
        try
        {
            if (sourceType == typeof(KX13M.CmsResource))
            {
                var kx13Guid = kx13Context.CmsResources.Where(c => c.ResourceId == sourceId).Select(x => x.ResourceGuid).Single();
                targetId = ResourceInfo.Provider.Get().WhereEquals(nameof(ResourceInfo.ResourceGUID), kx13Guid).Select(x => x.ResourceID).Single();
                return true;
            }

            if (sourceType == typeof(KX13M.CmsClass))
            {
                var kx13Guid = kx13Context.CmsClasses.Where(c => c.ClassId == sourceId).Select(x => x.ClassGuid).Single();
                targetId = DataClassInfoProvider.GetClasses().Where(x => x.ClassGUID == kx13Guid).Select(x => x.ClassID).Single();
                return true;
            }

            if (sourceType == typeof(KX13M.CmsUser))
            {
                var kx13User = kx13Context.CmsUsers.Where(c => c.UserId == sourceId).Select(x => new { x.UserGuid, x.UserName }).Single();
                targetId = UserInfo.Provider.Get().WhereEquals(nameof(UserInfo.UserGUID), kx13User.UserGuid).Or().WhereEquals(nameof(UserInfo.UserName), kx13User.UserName).Select(x => x.UserID).Single();
                return true;
            }

            if (sourceType == typeof(KX13M.CmsRole))
            {
                var kx13User = kx13Context.CmsRoles.Where(c => c.RoleId == sourceId).Select(x => new { x.RoleGuid }).Single();
                targetId = RoleInfo.Provider.Get().WhereEquals(nameof(RoleInfo.RoleGUID), kx13User.RoleGuid).Select(x => x.RoleID).Single();
                return true;
            }

            if (sourceType == typeof(KX13M.CmsSite))
            {
                var kx13Guid = kx13Context.CmsSites.Where(c => c.SiteId == sourceId).Select(x => x.SiteGuid).Single();
                targetId = ChannelInfo.Provider.Get().WhereEquals(nameof(ChannelInfo.ChannelGUID), kx13Guid).Select(x => x.ChannelID).Single();
                return true;
            }

            if (sourceType == typeof(KX13M.CmsState))
            {
                string stateName = kx13Context.CmsStates.Where(c => c.StateId == sourceId).Select(x => x.StateName).Single();
                targetId = StateInfo.Provider.Get().WhereEquals(nameof(StateInfo.StateName), stateName).Select(x => x.StateID).Single();
                return true;
            }

            if (sourceType == typeof(KX13M.CmsCountry))
            {
                string countryName = kx13Context.CmsCountries.Where(c => c.CountryId == sourceId).Select(x => x.CountryName).Single();
                targetId = CountryInfo.Provider.Get().WhereEquals(nameof(CountryInfo.CountryName), countryName).Select(x => x.CountryID).Single();
                return true;
            }

            if (sourceType == typeof(KX13M.OmContactStatus))
            {
                string statusName = kx13Context.OmContactStatuses.Where(c => c.ContactStatusId == sourceId).Select(x => x.ContactStatusName).Single();
                targetId = ContactStatusInfo.Provider.Get().WhereEquals(nameof(ContactStatusInfo.ContactStatusName), statusName).Select(x => x.ContactStatusID).Single();
                return true;
            }

            if (sourceType == typeof(KX13M.OmContact))
            {
                var contactGuid = kx13Context.OmContacts.Where(c => c.ContactId == sourceId).Select(x => x.ContactGuid).Single();
                targetId = ContactInfo.Provider.Get().WhereEquals(nameof(ContactInfo.ContactGUID), contactGuid).Select(x => x.ContactID).Single();
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
