using CMS.Membership;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Contexts;

namespace Migration.Tool.Core.KX13.Mappers;

public class UserRoleInfoMapper : EntityMapperBase<KX13M.CmsUserRole, UserRoleInfo>
{
    public UserRoleInfoMapper(ILogger<UserRoleInfoMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : base(logger, pkContext, protocol)
    {
    }

    protected override UserRoleInfo? CreateNewInstance(KX13M.CmsUserRole source, MappingHelper mappingHelper, AddFailure addFailure)
        => UserRoleInfo.New();

    protected override UserRoleInfo MapInternal(KX13M.CmsUserRole source, UserRoleInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        if (mappingHelper.TranslateRequiredId<KX13M.CmsRole>(r => r.RoleId, source.RoleId, out int xbkRoleId))
        {
            target.RoleID = xbkRoleId;
        }

        if (mappingHelper.TranslateRequiredId<KX13M.CmsUser>(r => r.UserId, source.UserId, out int xbkUserId))
        {
            target.UserID = xbkUserId;
        }

        return target;
    }
}
