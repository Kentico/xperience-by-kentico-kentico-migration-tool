using Microsoft.Data.SqlClient;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Mappers;
internal static class ModelFacadeExtensions
{
    internal static ICmsTree? LocateAncestor(this ModelFacade modelFacade, ICmsTree ksNode, int parentLevel)
    {
        ICmsTree? ancestor = ksNode;
        for (int i = 0; ancestor != null && i < -parentLevel; i++)
        {
            ancestor = modelFacade.Select<ICmsTree>("NodeID = @nodeID", "NodeOrder", new SqlParameter("nodeID", ancestor.NodeParentID)).FirstOrDefault();
        }
        return ancestor;
    }
}
