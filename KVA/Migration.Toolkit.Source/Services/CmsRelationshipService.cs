namespace Migration.Toolkit.Source.Services;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Source.Model;

public class CmsRelationshipService(
    ILogger<CmsRelationshipService> logger,
    ModelFacade modelFacade)
{
    public record NodeRelationShipResult(ICmsRelationship Relationship, ICmsTree? RightNode);
    public IEnumerable<NodeRelationShipResult> GetNodeRelationships(int nodeId)
    {
        var relationships = modelFacade.SelectWhere<ICmsRelationship>("LeftNodeId = @nodeId", new SqlParameter("nodeId", nodeId));
        foreach (var cmsRelationship in relationships)
        {
            yield return new(cmsRelationship, modelFacade.SelectById<ICmsTree>(cmsRelationship.RightNodeID));
        }
        // foreach (var cmsRelationship in kx13Context.CmsRelationships
        //              .Include(r => r.RelationshipName)
        //              .Include(r => r.RightNode)
        //              .Where(r => r.LeftNodeId == nodeId))
        // {
        //     yield return cmsRelationship;
        // }
    }
}