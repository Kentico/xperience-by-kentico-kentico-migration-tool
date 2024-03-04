namespace Migration.Toolkit.Core.K11.Services.CmsRelationship;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.K11;
using Migration.Toolkit.K11.Models;

public class CmsRelationshipService(ILogger<CmsRelationshipService> logger,
    IDbContextFactory<K11Context> k11ContextFactory)
{
    private readonly ILogger<CmsRelationshipService> _logger = logger;

    public IEnumerable<CmsRelationship> GetNodeRelationships(int nodeId)
    {
        var k11Context = k11ContextFactory.CreateDbContext();
        foreach (var cmsRelationship in k11Context.CmsRelationships
                     .Include(r => r.RelationshipName)
                     .Include(r => r.RightNode)
                     .Where(r => r.LeftNodeId == nodeId))
        {
            yield return cmsRelationship;
        }
    }
}