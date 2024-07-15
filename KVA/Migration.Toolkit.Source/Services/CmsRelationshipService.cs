namespace Migration.Toolkit.Source.Services;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Source.Model;

public class CmsRelationshipService(
    ModelFacade modelFacade)
{
    public record NodeRelationShipResult(ICmsRelationship Relationship, ICmsTree? RightNode);
    public IEnumerable<NodeRelationShipResult> GetNodeRelationships(int nodeId, string className, Guid fieldGuid)
    {
        var relationshipName = $"{className}_{fieldGuid}";
        var relationships = modelFacade.SelectWhere<ICmsRelationship>("""
                                                                      LeftNodeId = @nodeId AND
                                                                      EXISTS(
                                                                         SELECT 1 FROM CMS_RelationshipName
                                                                                  WHERE CMS_Relationship.RelationshipNameID = CMS_RelationshipName.RelationshipNameID AND
                                                                                        RelationshipName = @relationshipName
                                                                                  )
                                                                      """,
            new SqlParameter("nodeId", nodeId),
            new SqlParameter("relationshipName", relationshipName)
        );

        foreach (var cmsRelationship in relationships)
        {
            yield return new(cmsRelationship, modelFacade.SelectById<ICmsTree>(cmsRelationship.RightNodeID));
        }
    }
}