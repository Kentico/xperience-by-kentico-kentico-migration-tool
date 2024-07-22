using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX13.Contexts;
using Migration.Toolkit.KXP.Models;

namespace Migration.Toolkit.Core.KX13.Mappers;

public class CmsSettingsCategoryMapper(
    ILogger<CmsSettingsCategoryMapper> logger,
    PrimaryKeyMappingContext pkContext,
    IProtocol protocol,
    IEntityMapper<KX13M.CmsResource, CmsResource> cmsResourceMapper)
    : EntityMapperBase<KX13M.CmsSettingsCategory,
        CmsSettingsCategory>(logger, pkContext, protocol)
{
    protected override CmsSettingsCategory? CreateNewInstance(KX13M.CmsSettingsCategory source, MappingHelper mappingHelper,
        AddFailure addFailure) => new();


    protected override CmsSettingsCategory MapInternal(KX13M.CmsSettingsCategory source, CmsSettingsCategory target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
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
                pkContext.SetMapping<KX13M.CmsResource>(r => r.ResourceId, source.CategoryResourceId.Value, target.CategoryResourceId.Value);
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
                        addFailure(new MapperResultFailure<CmsSettingsCategory>(result.HandbookReference));
                        break;
                    }

                    default:
                        break;
                }
            }
        }
        else if (mappingHelper.TranslateIdAllowNulls<KX13M.CmsResource>(r => r.ResourceId, source.CategoryResourceId, out int? categoryResourceId))
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
                    addFailure(new MapperResultFailure<CmsSettingsCategory>(result.HandbookReference));
                    break;
                }

                default:
                    break;
            }
        }
        else if (mappingHelper.TranslateIdAllowNulls<KX13M.CmsCategory>(c => c.CategoryId, source.CategoryParentId, out int? categoryParentId))
        {
            target.CategoryParentId = categoryParentId;
        }

        return target;
    }
}
