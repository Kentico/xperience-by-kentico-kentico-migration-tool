namespace Migration.Toolkit.Core.KX12.Services;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Services;
using Migration.Toolkit.KX12.Context;
using Migration.Toolkit.KXP.Context;

public class PrimaryKeyLocatorService : IPrimaryKeyLocatorService
{
    private readonly ILogger<PrimaryKeyLocatorService> _logger;
    private readonly IDbContextFactory<KxpContext> _kxpContextFactory;
    private readonly IDbContextFactory<KX12Context> _kx12ContextFactory;

    public PrimaryKeyLocatorService(
        ILogger<PrimaryKeyLocatorService> logger,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IDbContextFactory<KX12Context> kx12ContextFactory
    )
    {
        _logger = logger;
        _kxpContextFactory = kxpContextFactory;
        _kx12ContextFactory = kx12ContextFactory;
    }

    private class KeyEqualityComparerWithLambda<T> : IEqualityComparer<T>
    {
        private readonly Func<T?, T?, bool> _equalityComparer;

        public KeyEqualityComparerWithLambda(Func<T?, T?, bool> equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public bool Equals(T? x, T? y) => _equalityComparer.Invoke(x, y);

        public int GetHashCode(T obj) => obj?.GetHashCode() ?? 0;
    }

    private record CmsUserKey(Guid UserGuid, string UserName);

    public IEnumerable<SourceTargetKeyMapping> SelectAll<T>(Expression<Func<T, object>> keyNameSelector)
    {
        using var kxpContext = _kxpContextFactory.CreateDbContext();
        using var kx12Context = _kx12ContextFactory.CreateDbContext();

        var sourceType = typeof(T);
        var memberName = keyNameSelector.GetMemberName();

        _logger.LogTrace("Preload of entity {Entity} member {MemberName} mapping requested", sourceType.Name, memberName);

        if (sourceType == typeof(KX12M.CmsUser) && memberName == nameof(KX12M.CmsUser.UserId))
        {
            var sourceUsers = kx12Context.CmsUsers.Select(x => new { x.UserId, x.UserGuid, x.UserName }).ToList();
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

        if (sourceType == typeof(KX12M.OmContact) && memberName == nameof(KX12M.OmContact.ContactId))
        {
            var source = kx12Context.OmContacts
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

        if (sourceType == typeof(KX12M.CmsTree) && memberName == nameof(KX12M.CmsTree.NodeId))
        {
            var source = kx12Context.CmsTrees.Select(x => new { x.NodeId, x.NodeGuid }).ToList();
            var target = kxpContext.CmsChannels.Select(x => new { x.ChannelId, x.ChannelGuid }).ToList();

            var result = source.Join(target,
                a => a.NodeGuid,
                b => b.ChannelGuid,
                (a, b) => new SourceTargetKeyMapping(a.NodeId, b.ChannelId)
            );

            foreach (var resultingMapping in result)
            {
                yield return resultingMapping;
            }

            yield break;
        }

        if (sourceType == typeof(KX12M.CmsState) && memberName == nameof(KX12M.CmsState.StateId))
        {
            var source = kx12Context.CmsStates.Select(x => new { x.StateId, x.StateName }).ToList();
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

        if (sourceType == typeof(KX12M.CmsCountry) && memberName == nameof(KX12M.CmsCountry.CountryId))
        {
            var source = kx12Context.CmsCountries.Select(x => new { x.CountryId, x.CountryName }).ToList();
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
        using var kxpContext = _kxpContextFactory.CreateDbContext();
        using var KX12Context = _kx12ContextFactory.CreateDbContext();

        var sourceType = typeof(T);
        targetId = -1;
        try
        {
            if (sourceType == typeof(KX12M.CmsResource))
            {
                var k12Guid = KX12Context.CmsResources.Where(c => c.ResourceId == sourceId).Select(x => x.ResourceGuid).Single();
                targetId = kxpContext.CmsResources.Where(x => x.ResourceGuid == k12Guid).Select(x => x.ResourceId).Single();
                return true;
            }

            if (sourceType == typeof(KX12M.CmsClass))
            {
                var k12Guid = KX12Context.CmsClasses.Where(c => c.ClassId == sourceId).Select(x => x.ClassGuid).Single();
                targetId = kxpContext.CmsClasses.Where(x => x.ClassGuid == k12Guid).Select(x => x.ClassId).Single();
                return true;
            }

            if (sourceType == typeof(KX12M.CmsUser))
            {
                var k12User = KX12Context.CmsUsers.Where(c => c.UserId == sourceId).Select(x => new { x.UserGuid, x.UserName }).Single();
                targetId = kxpContext.CmsUsers.Where(x => x.UserGuid == k12User.UserGuid || x.UserName == k12User.UserName).Select(x => x.UserId).Single();
                return true;
            }

            if (sourceType == typeof(KX12M.CmsRole))
            {
                var k12User = KX12Context.CmsRoles.Where(c => c.RoleId == sourceId).Select(x => new { x.RoleGuid }).Single();
                targetId = kxpContext.CmsRoles.Where(x => x.RoleGuid == k12User.RoleGuid).Select(x => x.RoleId).Single();
                return true;
            }

            if (sourceType == typeof(KX12M.CmsSite))
            {
                var k12Guid = KX12Context.CmsSites.Where(c => c.SiteId == sourceId).Select(x => x.SiteGuid).Single();
                targetId = kxpContext.CmsChannels.Where(x => x.ChannelGuid == k12Guid).Select(x => x.ChannelId).Single();
                return true;
            }

            if (sourceType == typeof(KX12M.CmsState))
            {
                var k12CodeName = KX12Context.CmsStates.Where(c => c.StateId == sourceId).Select(x => x.StateName).Single();
                targetId = kxpContext.CmsStates.Where(x => x.StateName == k12CodeName).Select(x => x.StateId).Single();
                return true;
            }

            if (sourceType == typeof(KX12M.CmsCountry))
            {
                var k12CodeName = KX12Context.CmsCountries.Where(c => c.CountryId == sourceId).Select(x => x.CountryName).Single();
                targetId = kxpContext.CmsCountries.Where(x => x.CountryName == k12CodeName).Select(x => x.CountryId).Single();
                return true;
            }

            if (sourceType == typeof(KX12M.OmContactStatus))
            {
                var k12Guid = KX12Context.OmContactStatuses.Where(c => c.ContactStatusId == sourceId).Select(x => x.ContactStatusName).Single();
                targetId = kxpContext.OmContactStatuses.Where(x => x.ContactStatusName == k12Guid).Select(x => x.ContactStatusId).Single();
                return true;
            }

            if (sourceType == typeof(KX12M.OmContact))
            {
                var k12Guid = KX12Context.OmContacts.Where(c => c.ContactId == sourceId).Select(x => x.ContactGuid).Single();
                targetId = kxpContext.OmContacts.Where(x => x.ContactGuid == k12Guid).Select(x => x.ContactId).Single();
                return true;
            }

            if (sourceType == typeof(KX12M.CmsTree))
            {
                // careful - cms.root will have different guid
                var k12Guid = KX12Context.CmsTrees.Where(c => c.NodeId == sourceId).Select(x => x.NodeGuid).Single();
                targetId = kxpContext.CmsChannels.Where(x => x.ChannelGuid == k12Guid).Select(x => x.ChannelId).Single();
                return true;
            }
        }
        catch (InvalidOperationException ioex)
        {
            if (ioex.Message.StartsWith("Sequence contains no elements"))
            {
                _logger.LogDebug("Mapping {SourceFullType} primary key: {SourceId} failed, {Message}", sourceType.FullName, sourceId, ioex.Message);
            }
            else
            {
                _logger.LogWarning("Mapping {SourceFullType} primary key: {SourceId} failed, {Message}", sourceType.FullName, sourceId, ioex.Message);
            }
            return false;
        }
        finally
        {
            if (targetId != -1)
            {
                _logger.LogTrace("Mapping {SourceFullType} primary key: {SourceId} to {TargetId}", sourceType.FullName, sourceId, targetId);
            }
        }

        _logger.LogError("Mapping {SourceFullType} primary key is not supported", sourceType.FullName);
        targetId = -1;
        return false;
    }
}