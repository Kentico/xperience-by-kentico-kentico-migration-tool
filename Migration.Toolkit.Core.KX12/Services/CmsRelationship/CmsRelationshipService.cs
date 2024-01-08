namespace Migration.Toolkit.Core.KX12.Services.CmsRelationship;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.KX12.Context;

public class CmsRelationshipService
{
    private readonly ILogger<CmsRelationshipService> _logger;
    private readonly IDbContextFactory<KX12Context> _kx12ContextFactory;

    public CmsRelationshipService(
        ILogger<CmsRelationshipService> logger,
        IDbContextFactory<KX12Context> kx12ContextFactory
    )
    {
        _logger = logger;
        _kx12ContextFactory = kx12ContextFactory;
    }

    public IEnumerable<KX12M.CmsRelationship> GetNodeRelationships(int nodeId)
    {
        var kx12Context = _kx12ContextFactory.CreateDbContext();
        foreach (var cmsRelationship in kx12Context.CmsRelationships
                     .Include(r => r.RelationshipName)
                     .Include(r => r.RightNode)
                     .Where(r => r.LeftNodeId == nodeId))
        {
            yield return cmsRelationship;
        }
    }
}