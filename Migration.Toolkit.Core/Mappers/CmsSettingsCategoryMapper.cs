using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Contexts;

namespace Migration.Toolkit.Core.Mappers;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KXP.Models;

public class CmsSettingsCategoryMapper : EntityMapperBase<Migration.Toolkit.KX13.Models.CmsSettingsCategory,
    CmsSettingsCategory>
{
    private readonly ILogger<CmsSettingsCategoryMapper> _logger;
    private readonly PrimaryKeyMappingContext _pkContext;
    private readonly IEntityMapper<KX13.Models.CmsResource, CmsResource> _cmsResourceMapper;

    public CmsSettingsCategoryMapper(ILogger<CmsSettingsCategoryMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol,
        IEntityMapper<KX13M.CmsResource, CmsResource> cmsResourceMapper) : base(logger, pkContext, protocol)
    {
        _logger = logger;
        _pkContext = pkContext;
        _cmsResourceMapper = cmsResourceMapper;
    }

    protected override CmsSettingsCategory? CreateNewInstance(KX13.Models.CmsSettingsCategory source, MappingHelper mappingHelper,
        AddFailure addFailure) => new();


    protected override CmsSettingsCategory MapInternal(KX13.Models.CmsSettingsCategory source, CmsSettingsCategory target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
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
                _logger.LogTrace("Skipping category resource '{ResourceGuid}', already present in target instance", target.CategoryResource.ResourceGuid);
                _pkContext.SetMapping<KX13.Models.CmsResource>(r => r.ResourceId, source.CategoryResourceId.Value, target.CategoryResourceId.Value);
            }
            else
            {
                switch (_cmsResourceMapper.Map(source.CategoryResource, target.CategoryResource))
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
                }
            }
        }
        else if(mappingHelper.TranslateIdAllowNulls<KX13.Models.CmsResource>(r => r.ResourceId, source.CategoryResourceId, out var categoryResourceId))
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
            }
        }
        else if(mappingHelper.TranslateIdAllowNulls<KX13M.CmsCategory>(c => c.CategoryId, source.CategoryParentId, out var categoryParentId))
        {
            target.CategoryParentId = categoryParentId;
        }

        return target;
    }
}