using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
public abstract class ContentItemDirectorBase
{
    internal Func<Guid, JToken> MediaInfoLoader = null!;

    /// <summary>
    /// Used to inject client's logic to determine how to handle an entity that is to be migrated as content item
    /// </summary>
    public abstract void Direct(ContentItemSource source, IContentItemActionProvider options);

    /// <summary>
    /// Used to inject client's logic to determine how to handle a source linked page
    /// </summary>
    public virtual void DirectLinkedNode(LinkedPageSource source, ILinkedPageActionProvider options) { }

    protected JToken IdentifierArrayPropertyValue(IEnumerable<Guid> itemGuids) => new JArray(itemGuids.Select(x => new JObject { { "identifier", x } }));
    protected JToken LinkedItemsPropertyValue(IEnumerable<Guid> itemGuids) => IdentifierArrayPropertyValue(itemGuids);
    protected JToken LinkedItemPropertyValue(Guid itemGuid) => LinkedItemsPropertyValue([itemGuid]);
    protected JToken MediaFilePropertyValue(Guid mediaFileGuid) => MediaInfoLoader(mediaFileGuid);
    protected JToken TaxonomyTagsPropertyValue(IEnumerable<Guid> tagGuids) => IdentifierArrayPropertyValue(tagGuids);

}
