
using CMS.Membership;

using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.K11.Models;

namespace Migration.Toolkit.Core.K11.Mappers;
public class UserRoleInfoMapper(ILogger<UserRoleInfoMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : EntityMapperBase<CmsUserRole, UserRoleInfo>(logger, pkContext, protocol)
{
    protected override UserRoleInfo? CreateNewInstance(CmsUserRole source, MappingHelper mappingHelper, AddFailure addFailure)
        => UserRoleInfo.New();

    protected override UserRoleInfo MapInternal(CmsUserRole source, UserRoleInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        if (mappingHelper.TranslateRequiredId<CmsRole>(r => r.RoleId, source.RoleId, out int xbkRoleId))
        {
            target.RoleID = xbkRoleId;
        }

        if (mappingHelper.TranslateRequiredId<CmsUser>(r => r.UserId, source.UserId, out int xbkUserId))
        {
            target.UserID = xbkUserId;
        }

        return target;
    }
}
