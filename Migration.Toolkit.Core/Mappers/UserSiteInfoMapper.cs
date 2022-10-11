namespace Migration.Toolkit.Core.Mappers;

using CMS.Membership;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;

public class UserSiteInfoMapper: EntityMapperBase<KX13M.CmsUserSite, UserSiteInfo>
{
    public UserSiteInfoMapper(ILogger<UserSiteInfoMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : base(logger, pkContext, protocol)
    {
    }

    protected override UserSiteInfo? CreateNewInstance(KX13M.CmsUserSite source, MappingHelper mappingHelper, AddFailure addFailure)
        => UserSiteInfo.New();

    protected override UserSiteInfo MapInternal(KX13M.CmsUserSite source, UserSiteInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        if (mappingHelper.TranslateRequiredId<KX13M.CmsSite>(r => r.SiteId, source.SiteId, out var xbkSiteId))
        {
            target.SiteID = xbkSiteId;
        }

        if (mappingHelper.TranslateRequiredId<KX13M.CmsUser>(r => r.UserId, source.UserId, out var xbkUserId))
        {
            target.UserID = xbkUserId;
        }

        return target;
    }
}