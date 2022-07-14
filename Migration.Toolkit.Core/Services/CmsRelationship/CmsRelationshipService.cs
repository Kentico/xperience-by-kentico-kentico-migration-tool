using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.KX13.Context;

namespace Migration.Toolkit.Core.Services.CmsRelationship;

public class CmsRelationshipService
{
    private readonly ILogger<CmsRelationshipService> _logger;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;

    public CmsRelationshipService(
        ILogger<CmsRelationshipService> logger,
        IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory
    )
    {
        _logger = logger;
        _kx13ContextFactory = kx13ContextFactory;
    }

    public IEnumerable<KX13M.CmsRelationship> GetNodeRelationships(int nodeId)
    {
        var kx13Context = _kx13ContextFactory.CreateDbContext();
        foreach (var cmsRelationship in kx13Context.CmsRelationships
                     .Include(r => r.RelationshipName)
                     .Include(r => r.RightNode)
                     .Where(r => r.LeftNodeId == nodeId))
        {
            yield return cmsRelationship;
        }
    }
}