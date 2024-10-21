using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.K11.Contexts;
using Migration.Tool.K11.Models;

namespace Migration.Tool.Core.K11.Mappers;

public class CmsUserMapper(
    ILogger<CmsUserMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol)
    : EntityMapperBase<CmsUser, KXP.Models.CmsUser>(logger, primaryKeyMappingContext, protocol)
{
    protected override KXP.Models.CmsUser CreateNewInstance(CmsUser tSourceEntity, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override KXP.Models.CmsUser MapInternal(CmsUser source, KXP.Models.CmsUser target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        if (!newInstance && source.UserGuid != target.UserGuid)
        {
            // assertion failed
            logger.LogTrace("Assertion failed, entity key mismatch");
            throw new InvalidOperationException("Assertion failed, entity key mismatch.");
        }

        target.UserName = source.UserName;
        target.FirstName = source.FirstName;
        target.LastName = source.LastName;
        target.Email = source.Email;
        target.UserPassword = source.UserPassword;
        target.UserEnabled = source.UserEnabled;
        target.UserCreated = source.UserCreated;
        target.LastLogon = source.LastLogon;
        target.UserGuid = source.UserGuid;
        target.UserLastModified = source.UserLastModified;
        target.UserSecurityStamp = source.UserSecurityStamp;
        target.UserAdministrationAccess = source.UserPrivilegeLevel == 3;
        target.UserIsPendingRegistration = false;
        target.UserPasswordLastChanged = null;
        target.UserRegistrationLinkExpiration = DateTime.Now.AddDays(365);

        foreach (var sourceCmsUserRole in source.CmsUserRoles)
        {
            if (mappingHelper.TranslateRequiredId<CmsRole>(r => r.RoleId, sourceCmsUserRole.RoleId, out int targetRoleId))
            {
                if (target.CmsUserRoles.All(x => x.RoleId != targetRoleId))
                {
                    target.CmsUserRoles.Add(new KXP.Models.CmsUserRole { RoleId = targetRoleId, User = target });
                }
            }
        }

        return target;
    }
}
