namespace Migration.Tool.Source.Services;

public class DeferredPathService
{
    private readonly List<DeferredWidgetPatch> deferredWidgetPatch = [];

    public IEnumerable<DeferredWidgetPatch> GetWidgetsToPatch() => deferredWidgetPatch.ToList();

    public void AddPatch(Guid uniqueId, string className, int webSiteChannelId) => deferredWidgetPatch.Add(new DeferredWidgetPatch(uniqueId, className, webSiteChannelId));

    public record DeferredWidgetPatch(Guid UniqueId, string ClassName, int WebSiteChannelId);
}
