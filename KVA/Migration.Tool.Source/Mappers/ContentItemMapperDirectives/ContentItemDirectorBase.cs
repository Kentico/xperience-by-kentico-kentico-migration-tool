using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
public abstract class ContentItemDirectorBase
{
    internal Func<Guid, JToken> MediaInfoLoader = null!;

    public abstract void Direct(ContentItemSource source, IContentItemActionProvider options);

    // Modifier 'virtual' chosen over 'abstract' to maintain backwards compatibility with custom user code that already implements a director
    public virtual void Direct(MediaContentItemSource source, IBaseContentItemActionProvider options) { }

    protected JToken IdentifierArrayPropertyValue(IEnumerable<Guid> itemGuids) => new JArray(itemGuids.Select(x => new JObject { { "identifier", x } }));
    protected JToken LinkedItemsPropertyValue(IEnumerable<Guid> itemGuids) => IdentifierArrayPropertyValue(itemGuids);
    protected JToken LinkedItemPropertyValue(Guid itemGuid) => LinkedItemsPropertyValue([itemGuid]);
    protected JToken MediaFilePropertyValue(Guid mediaFileGuid) => MediaInfoLoader(mediaFileGuid);
    protected JToken TaxonomyTagsPropertyValue(IEnumerable<Guid> tagGuids) => IdentifierArrayPropertyValue(tagGuids);

}
