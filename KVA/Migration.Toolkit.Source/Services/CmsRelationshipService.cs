using Microsoft.Data.SqlClient;

using Migration.Toolkit.Source.Model;

namespace Migration.Toolkit.Source.Services;

public class CmsRelationshipService(
    ModelFacade modelFacade)
{
    public IEnumerable<NodeRelationShipResult> GetNodeRelationships(int nodeId, string className, Guid fieldGuid)
    {
        string relationshipName = $"{className}_{fieldGuid}";
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
            yield return new NodeRelationShipResult(cmsRelationship, modelFacade.SelectById<ICmsTree>(cmsRelationship.RightNodeID));
        }
    }

    public record NodeRelationShipResult(ICmsRelationship Relationship, ICmsTree? RightNode);
}
