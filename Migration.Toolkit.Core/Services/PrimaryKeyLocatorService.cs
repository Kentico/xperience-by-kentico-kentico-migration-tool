using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Migration.Toolkit.Core.Services;

public class PrimaryKeyLocatorService : IPrimaryKeyLocatorService
{
    private readonly ILogger<PrimaryKeyLocatorService> _logger;
    private readonly IDbContextFactory<KXO.Context.KxoContext> _kxoContextFactory;
    private readonly IDbContextFactory<KX13.Context.KX13Context> _kx13ContextFactory;

    public PrimaryKeyLocatorService(
        ILogger<PrimaryKeyLocatorService> logger,
        IDbContextFactory<KXO.Context.KxoContext> kxoContextFactory,
        IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory
    )
    {
        _logger = logger;
        _kxoContextFactory = kxoContextFactory;
        _kx13ContextFactory = kx13ContextFactory;
    }

    public bool TryLocate<T>(Expression<Func<T, object>> keyNameSelector, int sourceId, out int targetId)
    {
        using var kxoContext = _kxoContextFactory.CreateDbContext();
        using var kx13Context = _kx13ContextFactory.CreateDbContext();

        // var memberName = keyNameSelector.GetMemberName();
        // var entityType = kx13Context.Model.FindEntityType(typeof(T));

        // TODO tk: 2022-05-18 can be done smarter => deferred to optimizations 
        var sourceType = typeof(T);
        targetId = -1;
        try
        {
            if (sourceType == typeof(KX13.Models.CmsClass))
            {
                var kx13Guid = kx13Context.CmsClasses.Where(c => c.ClassId == sourceId).Select(x => x.ClassGuid).Single();
                targetId = kxoContext.CmsClasses.Where(x => x.ClassGuid == kx13Guid).Select(x => x.ClassId).Single();
                return true;
            }

            if (sourceType == typeof(KX13.Models.CmsUser))
            {
                var kx13Guid = kx13Context.CmsUsers.Where(c => c.UserId == sourceId).Select(x => x.UserGuid).Single();
                targetId = kxoContext.CmsUsers.Where(x => x.UserGuid == kx13Guid).Select(x => x.UserId).Single();
                return true;
            }

            if (sourceType == typeof(KX13.Models.CmsSite))
            {
                var kx13Guid = kx13Context.CmsSites.Where(c => c.SiteId == sourceId).Select(x => x.SiteGuid).Single();
                targetId = kxoContext.CmsSites.Where(x => x.SiteGuid == kx13Guid).Select(x => x.SiteId).Single();
                return true;
            }
        }
        catch (InvalidOperationException ioex)
            // when(ioex.Message.Contains("SequenceContainsN"))
        {
            _logger.LogWarning("Mapping {sourceFullType} primary key: {sourceId} failed, {message}", sourceType.FullName, sourceId, ioex.Message);
            return false;
        }
        finally
        {
            if (targetId != -1)
            {
                _logger.LogTrace("Mapping {sourceFullType} primary key: {sourceId} to {targetId}", sourceType.FullName, sourceId, targetId);
            }
        }

        _logger.LogError("Mapping {sourceFullType} primary key is not supported", sourceType.FullName);
        targetId = -1;        
        return false;
    }
}