using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;

namespace Migration.Toolkit.Core.Mappers;

using Migration.Toolkit.KXP.Models;

public class CmsPageUrlPathMapper : EntityMapperBase<KX13.Models.CmsPageUrlPath, CmsPageUrlPath>
{
    public CmsPageUrlPathMapper(
        ILogger<CmsPageUrlPathMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    ) : base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override CmsPageUrlPath? CreateNewInstance(KX13.Models.CmsPageUrlPath source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override CmsPageUrlPath MapInternal(KX13.Models.CmsPageUrlPath source, CmsPageUrlPath target, bool newInstance,
        MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.PageUrlPathGuid = source.PageUrlPathGuid;
        target.PageUrlPathCulture = source.PageUrlPathCulture;
        target.PageUrlPathUrlPath = source.PageUrlPathUrlPath;
        target.PageUrlPathUrlPathHash = source.PageUrlPathUrlPathHash;
        target.PageUrlPathLastModified = source.PageUrlPathLastModified;

        if (mappingHelper.TranslateRequiredId<KX13.Models.CmsSite>(c => c.SiteId, source.PageUrlPathSiteId, out var siteId))
        {
            target.PageUrlPathSiteId = siteId;
        }

        return target;
    }
}