using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Services;

public class DeferredTreeNodesService
{
    private readonly List<ICmsTree> deferredNodes = [];

    public IEnumerable<ICmsTree> GetNodes() => deferredNodes.ToList();
    public void Clear() => deferredNodes.Clear();

    public void AddNode(ICmsTree node) => deferredNodes.Add(node);

    public readonly Dictionary<Guid, (Guid ContentItemCommonDataGuid, Guid WidgetVariantGuid)> WidgetizedDocuments = [];
}
