namespace Migration.Toolkit.Source.Services;

public class DeferredPathService
{
    public record DeferredWidgetPatch(Guid UniqueId, string ClassName, int WebSiteChannelId);
    private readonly List<DeferredWidgetPatch> _deferredWidgetPatch = new();


    public IEnumerable<DeferredWidgetPatch> GetWidgetsToPatch()
    {
        return _deferredWidgetPatch.ToList();
    }

    public void AddPatch(Guid uniqueId, string className, int webSiteChannelId)
    {
        _deferredWidgetPatch.Add(new (uniqueId, className, webSiteChannelId));
    }
}