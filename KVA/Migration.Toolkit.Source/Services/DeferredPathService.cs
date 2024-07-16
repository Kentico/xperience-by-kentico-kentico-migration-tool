namespace Migration.Toolkit.Source.Services;

public class DeferredPathService
{
    private readonly List<DeferredWidgetPatch> _deferredWidgetPatch = [];


    public IEnumerable<DeferredWidgetPatch> GetWidgetsToPatch() => _deferredWidgetPatch.ToList();

    public void AddPatch(Guid uniqueId, string className, int webSiteChannelId) => _deferredWidgetPatch.Add(new DeferredWidgetPatch(uniqueId, className, webSiteChannelId));

    public record DeferredWidgetPatch(Guid UniqueId, string ClassName, int WebSiteChannelId);
}
