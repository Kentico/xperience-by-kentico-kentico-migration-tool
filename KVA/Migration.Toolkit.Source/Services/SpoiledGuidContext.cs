using System.Collections.Frozen;
using System.Collections.Immutable;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.Services;
using Migration.Toolkit.Source.Model;

namespace Migration.Toolkit.Source.Services;

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
                                                     SELECT DocumentGUID, STRING_AGG(CONCAT(NodeSiteID, '-', NodeID), '|') [SiteID-NodeID]
                                                     FROM View_CMS_Tree_Joined TJ
                                                     GROUP BY DocumentGUID
                                                     HAVING COUNT(DocumentID) > 1
                                                     """,
                (reader, version) => new { DocumentGUID = reader.Unbox<Guid>("DocumentGuid"), Info = reader.Unbox<string>("SiteID-NodeID") })
            .ToFrozenDictionary(
                x => x.DocumentGUID,
                x => x.Info.Split('|').Select(i =>
                {
                    string[] spl = i.Split('-');

                    return new SpoiledDocumentGuidInfo(int.Parse(spl[0]), int.Parse(spl[1]));
                }).ToImmutableList()
            );

    internal IDictionary<Guid, ImmutableList<SpoiledNodeGuidInfo>> SpoiledNodeGuids =>
        spoiledNodeGuids ??= modelFacade.Select("""
                                                 SELECT NodeGUID, STRING_AGG(NodeSiteID, '|') [SiteID]
                                                 FROM View_CMS_Tree_Joined TJ
                                                 GROUP BY NodeGUID
                                                 HAVING COUNT(NodeGUID) > 1
                                                 """,
                (reader, version) => new { DocumentGUID = reader.Unbox<Guid>("NodeGUID"), Info = reader.Unbox<string>("SiteID") })
            .ToFrozenDictionary(
                x => x.DocumentGUID,
                x => x.Info.Split('|').Select(i =>
                {
                    string[] spl = i.Split('-');

                    return new SpoiledNodeGuidInfo(int.Parse(spl[0]));
                }).ToImmutableList()
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
