using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Contexts;

namespace Migration.Toolkit.Core.Mappers;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KXP.Models;

public class CmsUserMapper : EntityMapperBase<KX13.Models.CmsUser, CmsUser>
{
    private readonly ILogger<CmsUserMapper> _logger;

    public CmsUserMapper(
        ILogger<CmsUserMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    ) : base(logger, primaryKeyMappingContext, protocol)
    {
        _logger = logger;
    }

    protected override CmsUser CreateNewInstance(KX13.Models.CmsUser tSourceEntity, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override CmsUser MapInternal(KX13.Models.CmsUser source, CmsUser target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        if (!newInstance && source.UserGuid != target.UserGuid)
        {
            // assertion failed
            _logger.LogTrace("Assertion failed, entity key mismatch");
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
            if (mappingHelper.TranslateRequiredId<KX13M.CmsRole>(r => r.RoleId, sourceCmsUserRole.RoleId, out var targetRoleId))
            {
                if (target.CmsUserRoles.All(x => x.RoleId != targetRoleId))
                {
                    target.CmsUserRoles.Add(new CmsUserRole
                    {
                        RoleId = targetRoleId,
                        User = target,
                    });
                }
            }
        }

        return target;
    }
}