namespace Migration.Toolkit.Core.K11.Mappers;

using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.K11.Models;

public class CmsSettingsCategoryMapper(ILogger<CmsSettingsCategoryMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol,
        IEntityMapper<Toolkit.K11.Models.CmsResource, KXP.Models.CmsResource> cmsResourceMapper)
    : EntityMapperBase<Toolkit.K11.Models.CmsSettingsCategory,
    KXP.Models.CmsSettingsCategory>(logger, pkContext, protocol)
{
    protected override KXP.Models.CmsSettingsCategory? CreateNewInstance(Toolkit.K11.Models.CmsSettingsCategory source, MappingHelper mappingHelper,
        AddFailure addFailure) => new();


    protected override KXP.Models.CmsSettingsCategory MapInternal(Toolkit.K11.Models.CmsSettingsCategory source, KXP.Models.CmsSettingsCategory target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        // no category guid to match on...
        if (newInstance)
        {
            target.CategoryOrder = source.CategoryOrder;
            target.CategoryName = source.CategoryName;
            target.CategoryDisplayName = source.CategoryDisplayName;
            target.CategoryIdpath = source.CategoryIdpath;
            target.CategoryLevel = source.CategoryLevel;
            target.CategoryChildCount = source.CategoryChildCount;
            target.CategoryIconPath = source.CategoryIconPath;
            target.CategoryIsGroup = source.CategoryIsGroup;
            target.CategoryIsCustom = source.CategoryIsCustom;
        }

        if (source.CategoryResource != null)
        {
            if (target.CategoryResource != null && source.CategoryResourceId != null && target.CategoryResourceId != null)
            {
                // skip if target is present
                logger.LogTrace("Skipping category resource '{ResourceGuid}', already present in target instance", target.CategoryResource.ResourceGuid);
                pkContext.SetMapping<Toolkit.K11.Models.CmsResource>(r => r.ResourceId, source.CategoryResourceId.Value, target.CategoryResourceId.Value);
            }
            else
            {
                switch (cmsResourceMapper.Map(source.CategoryResource, target.CategoryResource))
                {
                    case { Success: true } result:
                        {
                            target.CategoryResource = result.Item;
                            break;
                        }
                    case { Success: false } result:
                        {
                            addFailure(new MapperResultFailure<KXP.Models.CmsSettingsCategory>(result.HandbookReference));
                            break;
                        }
                }
            }
        }
        else if (mappingHelper.TranslateIdAllowNulls<Toolkit.K11.Models.CmsResource>(r => r.ResourceId, source.CategoryResourceId, out var categoryResourceId))
        {
            target.CategoryResourceId = categoryResourceId;
        }

        if (source.CategoryParent != null)
        {
            switch (Map(source.CategoryParent, target.CategoryParent))
            {
                case { Success: true } result:
                    {
                        target.CategoryParent = result.Item;
                        break;
                    }
                case { Success: false } result:
                    {
                        addFailure(new MapperResultFailure<KXP.Models.CmsSettingsCategory>(result.HandbookReference));
                        break;
                    }
            }
        }
        else if (mappingHelper.TranslateIdAllowNulls<CmsCategory>(c => c.CategoryId, source.CategoryParentId, out var categoryParentId))
        {
            target.CategoryParentId = categoryParentId;
        }

        return target;
    }
}