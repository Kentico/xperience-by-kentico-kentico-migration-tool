using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KXP.Context;

namespace Migration.Toolkit.Core.KX13.Services;

public class PrimaryKeyLocatorService(
    ILogger<PrimaryKeyLocatorService> logger,
    IDbContextFactory<KxpContext> kxpContextFactory,
    IDbContextFactory<KX13Context> kx13ContextFactory)
    : IPrimaryKeyLocatorService
{
    public IEnumerable<SourceTargetKeyMapping> SelectAll<T>(Expression<Func<T, object>> keyNameSelector)
    {
        using var kxpContext = kxpContextFactory.CreateDbContext();
        using var kx13Context = kx13ContextFactory.CreateDbContext();

        var sourceType = typeof(T);
        string memberName = keyNameSelector.GetMemberName();

        logger.LogTrace("Preload of entity {Entity} member {MemberName} mapping requested", sourceType.Name, memberName);

        if (sourceType == typeof(KX13M.CmsUser) && memberName == nameof(KX13M.CmsUser.UserId))
        {
            var sourceUsers = kx13Context.CmsUsers.Select(x => new { x.UserId, x.UserGuid, x.UserName }).ToList();
            var targetUsers = kxpContext.CmsUsers.Select(x => new { x.UserId, x.UserName, x.UserGuid }).ToList();

            var result = sourceUsers.Join(targetUsers,
                a => new CmsUserKey(a.UserGuid, a.UserName),
                b => new CmsUserKey(b.UserGuid, b.UserName),
                (a, b) => new SourceTargetKeyMapping(a.UserId, b.UserId),
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
            var target = kxpContext.OmContacts
                .OrderBy(c => c.ContactCreated)
                .Select(x => new { x.ContactId, x.ContactGuid }).ToList();

            var result = source.Join(target,
                a => a.ContactGuid,
                b => b.ContactGuid,
                (a, b) => new SourceTargetKeyMapping(a.ContactId, b.ContactId)
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
            var target = kxpContext.CmsStates.Select(x => new { x.StateId, x.StateName }).ToList();

            var result = source.Join(target,
                a => a.StateName,
                b => b.StateName,
                (a, b) => new SourceTargetKeyMapping(a.StateId, b.StateId)
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
            var target = kxpContext.CmsCountries.Select(x => new { x.CountryId, x.CountryName }).ToList();

            var result = source.Join(target,
                a => a.CountryName,
                b => b.CountryName,
                (a, b) => new SourceTargetKeyMapping(a.CountryId, b.CountryId)
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
        using var kx13Context = kx13ContextFactory.CreateDbContext();

        var sourceType = typeof(T);
        targetId = -1;
        try
        {
            if (sourceType == typeof(KX13M.CmsResource))
            {
                var kx13Guid = kx13Context.CmsResources.Where(c => c.ResourceId == sourceId).Select(x => x.ResourceGuid).Single();
                targetId = kxpContext.CmsResources.Where(x => x.ResourceGuid == kx13Guid).Select(x => x.ResourceId).Single();
                return true;
            }

            if (sourceType == typeof(KX13M.CmsClass))
            {
                var kx13Guid = kx13Context.CmsClasses.Where(c => c.ClassId == sourceId).Select(x => x.ClassGuid).Single();
                targetId = kxpContext.CmsClasses.Where(x => x.ClassGuid == kx13Guid).Select(x => x.ClassId).Single();
                return true;
            }

            if (sourceType == typeof(KX13M.CmsUser))
            {
                var kx13User = kx13Context.CmsUsers.Where(c => c.UserId == sourceId).Select(x => new { x.UserGuid, x.UserName }).Single();
                targetId = kxpContext.CmsUsers.Where(x => x.UserGuid == kx13User.UserGuid || x.UserName == kx13User.UserName).Select(x => x.UserId).Single();
                return true;
            }

            if (sourceType == typeof(KX13M.CmsRole))
            {
                var kx13User = kx13Context.CmsRoles.Where(c => c.RoleId == sourceId).Select(x => new { x.RoleGuid }).Single();
                targetId = kxpContext.CmsRoles.Where(x => x.RoleGuid == kx13User.RoleGuid).Select(x => x.RoleId).Single();
                return true;
            }

            if (sourceType == typeof(KX13M.CmsSite))
            {
                var kx13Guid = kx13Context.CmsSites.Where(c => c.SiteId == sourceId).Select(x => x.SiteGuid).Single();
                targetId = kxpContext.CmsChannels.Where(x => x.ChannelGuid == kx13Guid).Select(x => x.ChannelId).Single();
                return true;
            }

            if (sourceType == typeof(KX13M.CmsState))
            {
                string kx13CodeName = kx13Context.CmsStates.Where(c => c.StateId == sourceId).Select(x => x.StateName).Single();
                targetId = kxpContext.CmsStates.Where(x => x.StateName == kx13CodeName).Select(x => x.StateId).Single();
                return true;
            }

            if (sourceType == typeof(KX13M.CmsCountry))
            {
                string kx13CodeName = kx13Context.CmsCountries.Where(c => c.CountryId == sourceId).Select(x => x.CountryName).Single();
                targetId = kxpContext.CmsCountries.Where(x => x.CountryName == kx13CodeName).Select(x => x.CountryId).Single();
                return true;
            }

            if (sourceType == typeof(KX13M.OmContactStatus))
            {
                string kx13Guid = kx13Context.OmContactStatuses.Where(c => c.ContactStatusId == sourceId).Select(x => x.ContactStatusName).Single();
                targetId = kxpContext.OmContactStatuses.Where(x => x.ContactStatusName == kx13Guid).Select(x => x.ContactStatusId).Single();
                return true;
            }

            if (sourceType == typeof(KX13M.OmContact))
            {
                var kx13Guid = kx13Context.OmContacts.Where(c => c.ContactId == sourceId).Select(x => x.ContactGuid).Single();
                targetId = kxpContext.OmContacts.Where(x => x.ContactGuid == kx13Guid).Select(x => x.ContactId).Single();
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
