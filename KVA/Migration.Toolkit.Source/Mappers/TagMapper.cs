
using Kentico.Xperience.UMT.Model;

using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Source.Model;

namespace Migration.Toolkit.Source.Mappers;
public record TagModelSource(Guid TaxonomyGuid, ICmsCategory CmsCategory, Dictionary<int, Guid> CategoryId2Guid);
public class TagMapper(ILogger<TagMapper> logger) : UmtMapperBase<TagModelSource>
{
    protected override IEnumerable<IUmtModel> MapInternal(TagModelSource source)
    {
        var (taxonomyGuid, cmsCategory, id2Guid) = source;
        var tag = new TagModel
        {
            TagName = cmsCategory.CategoryName,
            TagTitle = cmsCategory.CategoryDisplayName,
            TagDescription = cmsCategory.CategoryDescription,
            TagGUID = cmsCategory.CategoryGUID,
            TagTaxonomyGUID = taxonomyGuid,
            TagOrder = 0,
            TagTranslations = [],
        };

        if (cmsCategory.CategoryParentID is { } categoryParentId)
        {
            if (id2Guid.TryGetValue(categoryParentId, out var categoryGuid))
            {
                tag.TagParentGUID = categoryGuid;
            }
            else
            {
                logger.LogWarning("Missing parent category {CategoryParentID} in source instance", categoryParentId);
            }
        }

        yield return tag;
    }
}
