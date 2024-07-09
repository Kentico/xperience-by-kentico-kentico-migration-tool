namespace Migration.Toolkit.Core.K11.Services;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Services;
using Migration.Toolkit.K11;
using Migration.Toolkit.K11.Models;
using Migration.Toolkit.KXP.Context;

public class PrimaryKeyLocatorService(ILogger<PrimaryKeyLocatorService> logger,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IDbContextFactory<K11Context> k11ContextFactory)
    : IPrimaryKeyLocatorService
{
    private class KeyEqualityComparerWithLambda<T>(Func<T?, T?, bool> equalityComparer) : IEqualityComparer<T>
    {
        public bool Equals(T? x, T? y) => equalityComparer.Invoke(x, y);

        public int GetHashCode(T obj) => obj?.GetHashCode() ?? 0;
    }

    private record CmsUserKey(Guid UserGuid, string UserName);

    public IEnumerable<SourceTargetKeyMapping> SelectAll<T>(Expression<Func<T, object>> keyNameSelector)
    {
        using var kxpContext = kxpContextFactory.CreateDbContext();
        using var k11Context = k11ContextFactory.CreateDbContext();

        var sourceType = typeof(T);
        var memberName = keyNameSelector.GetMemberName();

        logger.LogTrace("Preload of entity {Entity} member {MemberName} mapping requested", sourceType.Name, memberName);

        if (sourceType == typeof(CmsUser) && memberName == nameof(CmsUser.UserId))
        {
            var sourceUsers = k11Context.CmsUsers.Select(x => new { x.UserId, x.UserGuid, x.UserName }).ToList();
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

        if (sourceType == typeof(OmContact) && memberName == nameof(OmContact.ContactId))
        {
            var source = k11Context.OmContacts
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

        if (sourceType == typeof(CmsState) && memberName == nameof(CmsState.StateId))
        {
            var source = k11Context.CmsStates.Select(x => new { x.StateId, x.StateName }).ToList();
            var target = kxpContext.CmsStates.Select(x => new {  x.StateId, x.StateName }).ToList();

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

        if (sourceType == typeof(CmsCountry) && memberName == nameof(CmsCountry.CountryId))
        {
            var source = k11Context.CmsCountries.Select(x => new { x.CountryId, x.CountryName }).ToList();
            var target = kxpContext.CmsCountries.Select(x => new {  x.CountryId, x.CountryName }).ToList();

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
        using var KX12Context = k11ContextFactory.CreateDbContext();

        var sourceType = typeof(T);
        targetId = -1;
        try
        {
            if (sourceType == typeof(CmsResource))
            {
                var k11Guid = KX12Context.CmsResources.Where(c => c.ResourceId == sourceId).Select(x => x.ResourceGuid).Single();
                targetId = kxpContext.CmsResources.Where(x => x.ResourceGuid == k11Guid).Select(x => x.ResourceId).Single();
                return true;
            }

            if (sourceType == typeof(Toolkit.K11.Models.CmsClass))
            {
                var k11Guid = KX12Context.CmsClasses.Where(c => c.ClassId == sourceId).Select(x => x.ClassGuid).Single();
                targetId = kxpContext.CmsClasses.Where(x => x.ClassGuid == k11Guid).Select(x => x.ClassId).Single();
                return true;
            }

            if (sourceType == typeof(CmsUser))
            {
                var k11User = KX12Context.CmsUsers.Where(c => c.UserId == sourceId).Select(x => new { x.UserGuid, x.UserName }).Single();
                targetId = kxpContext.CmsUsers.Where(x => x.UserGuid == k11User.UserGuid || x.UserName == k11User.UserName).Select(x => x.UserId).Single();
                return true;
            }

            if (sourceType == typeof(CmsRole))
            {
                var k11Role = KX12Context.CmsRoles.Where(c => c.RoleId == sourceId).Select(x => new { x.RoleGuid }).Single();
                targetId = kxpContext.CmsRoles.Where(x => x.RoleGuid == k11Role.RoleGuid).Select(x => x.RoleId).Single();
                return true;
            }

            if (sourceType == typeof(CmsSite))
            {
                var k11Guid = KX12Context.CmsSites.Where(c => c.SiteId == sourceId).Select(x => x.SiteGuid).Single();
                targetId = kxpContext.CmsChannels.Where(x => x.ChannelGuid == k11Guid).Select(x => x.ChannelId).Single();
                return true;
            }

            if (sourceType == typeof(CmsState))
            {
                var k11CodeName = KX12Context.CmsStates.Where(c => c.StateId == sourceId).Select(x => x.StateName).Single();
                targetId = kxpContext.CmsStates.Where(x => x.StateName == k11CodeName).Select(x => x.StateId).Single();
                return true;
            }

            if (sourceType == typeof(CmsCountry))
            {
                var k11CodeName = KX12Context.CmsCountries.Where(c => c.CountryId == sourceId).Select(x => x.CountryName).Single();
                targetId = kxpContext.CmsCountries.Where(x => x.CountryName == k11CodeName).Select(x => x.CountryId).Single();
                return true;
            }

            if (sourceType == typeof(OmContactStatus))
            {
                var k11Guid = KX12Context.OmContactStatuses.Where(c => c.ContactStatusId == sourceId).Select(x => x.ContactStatusName).Single();
                targetId = kxpContext.OmContactStatuses.Where(x => x.ContactStatusName == k11Guid).Select(x => x.ContactStatusId).Single();
                return true;
            }

            if (sourceType == typeof(OmContact))
            {
                var k11Guid = KX12Context.OmContacts.Where(c => c.ContactId == sourceId).Select(x => x.ContactGuid).Single();
                targetId = kxpContext.OmContacts.Where(x => x.ContactGuid == k11Guid).Select(x => x.ContactId).Single();
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
}