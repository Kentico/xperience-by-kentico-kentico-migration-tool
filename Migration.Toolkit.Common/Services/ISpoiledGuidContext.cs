namespace Migration.Toolkit.Common.Services;

public interface ISpoiledGuidContext
{
    Guid EnsureDocumentGuid(Guid documentGuid, int siteId, int nodeId, int documentId);
    Guid EnsureNodeGuid(Guid nodeGuid, int siteId, int nodeId);
    Guid EnsureNodeGuid(Guid nodeGuid, int siteId);
    Guid? GetNodeGuid(int nodeId, int siteId);
}