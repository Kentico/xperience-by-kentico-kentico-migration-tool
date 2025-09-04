using System.Collections.Frozen;
using System.Collections.Immutable;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.Services;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Services;

/// <summary>
///     in cases where consumer exported site with documents and created new site with that particular export, GUID
///     conflicts will probably happen. This class aims to query and solve those conflicts
/// </summary>
/// <param name="modelFacade"></param>
/// <param name="logger"></param>
public class SpoiledGuidContext(ModelFacade modelFacade, ILogger<SpoiledGuidContext> logger) : ISpoiledGuidContext
{
    private IDictionary<Guid, ImmutableList<SpoiledDocumentGuidInfo>>? spoiledDocumentGuids;
    private IDictionary<Guid, ImmutableList<SpoiledNodeGuidInfo>>? spoiledNodeGuids;

    internal IDictionary<Guid, ImmutableList<SpoiledDocumentGuidInfo>> SpoiledDocumentGuids =>
        spoiledDocumentGuids ??= modelFacade.Select("""
                                                     SELECT DocumentGUID, NodeSiteID, NodeID 
                                                     FROM View_CMS_Tree_Joined TJ
                                                     WHERE EXISTS(
                                                     	SELECT 1
                                                     	FROM View_CMS_Tree_Joined TJI
                                                     	WHERE TJI.DocumentGUID = TJ.DocumentGUID
                                                     	GROUP BY DocumentGUID
                                                     	HAVING COUNT(DocumentID) > 1
                                                     )
                                                     """,
                (reader, version) => new
                {
                    DocumentGUID = reader.Unbox<Guid>("DocumentGUID"),
                    NodeID = reader.Unbox<int>("NodeID"),
                    NodeSiteID = reader.Unbox<int>("NodeSiteID"),
                })
            .GroupBy(x => x.DocumentGUID)
            .ToFrozenDictionary(
                x => x.Key,
                x => x.Select(i => new SpoiledDocumentGuidInfo(i.NodeSiteID, i.NodeID)).ToImmutableList()
            );

    internal IDictionary<Guid, ImmutableList<SpoiledNodeGuidInfo>> SpoiledNodeGuids =>
        spoiledNodeGuids ??= modelFacade.Select("""
                                                 SELECT NodeGUID, NodeSiteID 
                                                 FROM View_CMS_Tree_Joined TJ
                                                 WHERE EXISTS (
                                                 	SELECT 1
                                                 	FROM View_CMS_Tree_Joined TJI
                                                 	WHERE TJI.NodeGUID = TJ.NodeGUID
                                                 	GROUP BY NodeGUID
                                                 	HAVING COUNT(NodeGUID) > 1
                                                 )
                                                 """,
                (reader, version) => new
                {
                    NodeGUID = reader.Unbox<Guid>("NodeGUID"),
                    NodeSiteID = reader.Unbox<int>("NodeSiteID")
                })
            .GroupBy(x => x.NodeGUID)
            .ToFrozenDictionary(
                x => x.Key,
                x => x.Select(i => new SpoiledNodeGuidInfo(i.NodeSiteID)).ToImmutableList()
            );

    public Guid EnsureDocumentGuid(Guid documentGuid, int siteId, int nodeId, int documentId)
    {
        if (!SpoiledDocumentGuids.TryGetValue(documentGuid, out _))
        {
            return documentGuid;
        }

        var newGuid = GuidHelper.CreateDocumentGuid($"{documentId}|{nodeId}|{siteId}");
        logger.LogTrace("Spoiled document guid encountered '{OriginalGuid}', replaced by {NewGuid} {Details}", documentGuid, newGuid, new { siteId, nodeId, documentId });
        return newGuid;
    }

    public Guid EnsureNodeGuid(Guid nodeGuid, int siteId, int nodeId)
    {
        if (!SpoiledNodeGuids.TryGetValue(nodeGuid, out _))
        {
            return nodeGuid;
        }

        var newGuid = GuidHelper.CreateNodeGuid($"{nodeId}|{siteId}");
        logger.LogTrace("Spoiled node guid encountered '{OriginalGuid}', replaced by {NewGuid} {Details}", nodeGuid, newGuid, new { siteId, nodeId });
        return newGuid;
    }

    public Guid EnsureNodeGuid(Guid nodeGuid, int siteId)
    {
        if (!SpoiledNodeGuids.TryGetValue(nodeGuid, out _))
        {
            return nodeGuid;
        }

        int nodeId = modelFacade.Select("""
                                        SELECT NodeID FROM CMS_Tree WHERE NodeSiteID = @siteId AND NodeGUID = @nodeGuid
                                        """, (reader, version) => reader.Unbox<int>("NodeID"),
            new SqlParameter("siteId", siteId), new SqlParameter("nodeGuid", nodeGuid)
        ).Single();

        var newGuid = GuidHelper.CreateNodeGuid($"{nodeId}|{siteId}");
        logger.LogTrace("Spoiled node guid encountered '{OriginalGuid}', replaced by {NewGuid} {Details}", nodeGuid, newGuid, new { siteId, nodeId });
        return newGuid;
    }

    public Guid? GetNodeGuid(int nodeId, int siteId)
    {
        var nodeGuid = modelFacade.Select("""
                                          SELECT NodeGUID FROM CMS_Tree WHERE NodeSiteID = @siteId AND NodeId = @nodeId
                                          """, (reader, version) => reader.Unbox<Guid?>("NodeGUID"),
            new SqlParameter("siteId", siteId), new SqlParameter("nodeId", nodeId)
        ).FirstOrDefault();

        if (nodeGuid is not { } sNodeGuid)
        {
            return null;
        }

        if (!SpoiledNodeGuids.TryGetValue(sNodeGuid, out _))
        {
            return sNodeGuid;
        }

        var newGuid = GuidHelper.CreateNodeGuid($"{nodeId}|{siteId}");
        logger.LogTrace("Spoiled node guid encountered '{OriginalGuid}', replaced by {NewGuid} {Details}", nodeGuid, newGuid, new { siteId, nodeId });
        return newGuid;
    }

    public Guid EnsureNodeGuid(ICmsTree node) => EnsureNodeGuid(node.NodeGUID, node.NodeSiteID, node.NodeID);

    internal record SpoiledDocumentGuidInfo(int SiteId, int NodeId);

    internal record SpoiledNodeGuidInfo(int SiteId);
}
