namespace Migration.Toolkit.Core.Mappers;

using CMS.Membership;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.Contexts;

public class UserRoleInfoMapper: EntityMapperBase<KX12M.CmsUserRole, UserRoleInfo>
{
    public UserRoleInfoMapper(ILogger<UserRoleInfoMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : base(logger, pkContext, protocol)
    {
    }

    protected override UserRoleInfo? CreateNewInstance(KX12M.CmsUserRole source, MappingHelper mappingHelper, AddFailure addFailure)
        => UserRoleInfo.New();

    protected override UserRoleInfo MapInternal(KX12M.CmsUserRole source, UserRoleInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        if (mappingHelper.TranslateRequiredId<KX12M.CmsRole>(r => r.RoleId, source.RoleId, out var xbkRoleId))
        {
            target.RoleID = xbkRoleId;
        }

        if (mappingHelper.TranslateRequiredId<KX12M.CmsUser>(r => r.UserId, source.UserId, out var xbkUserId))
        {
            target.UserID = xbkUserId;
        }

        return target;
    }
}