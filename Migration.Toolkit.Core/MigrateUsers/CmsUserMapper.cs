using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;

namespace Migration.Toolkit.Core.MigrateUsers;

public class CmsUserMapper: IEntityMapper<KX13.Models.CmsUser, KXO.Models.CmsUser>
{
    private readonly ILogger<CmsUserMapper> _logger;

    public CmsUserMapper(ILogger<CmsUserMapper> logger)
    {
        _logger = logger;
    }
    
    public ModelMappingResult<KXO.Models.CmsUser> Map(KX13.Models.CmsUser? source, KXO.Models.CmsUser? target)
    {
        if (source is null)
        {
            _logger.LogTrace("Source entity is not defined.");
            return new ModelMappingFailedSourceNotDefined<Migration.Toolkit.KXO.Models.CmsUser>();
        }

        var newInstance = false;
        if (target is null)
        {
            _logger.LogTrace("Null target supplied, creating new instance.");
            target = new Migration.Toolkit.KXO.Models.CmsUser();
            newInstance = true;
        }
        else if (source.UserGuid != target.UserGuid)
        {
            // assertion failed
            _logger.LogTrace("Assertion failed, entity key mismatch.");
            return new ModelMappingFailedKeyMismatch<Migration.Toolkit.KXO.Models.CmsUser>();
        }

        // do not try to insert pk
        // target.UserId = source.UserId;
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

        // TODO tk: 2022-05-18 deduced - check
        target.UserAdministrationAccess = source.UserPrivilegeLevel == 3;
        // TODO tk: 2022-05-18 deduce info
        target.UserIsPendingRegistration = false;
        // TODO tk: 2022-05-18 deduce info
        target.UserPasswordLastChanged = DateTime.Now;
        // TODO tk: 2022-05-18 deduce info
        target.UserRegistrationLinkExpiration = DateTime.Now.AddDays(365);

        // TODO tk: 2022-05-18 ROLE
        // TODO tk: 2022-05-18 SITE

        return new ModelMappingSuccess<KXO.Models.CmsUser>(target, newInstance);

        // removed in kxo
        // target.MiddleName = source.MiddleName;
        // target.FullName = source.FullName;
        // target.PreferredCultureCode = source.PreferredCultureCode;
        // target.PreferredUicultureCode = source.PreferredUicultureCode;
        // target.UserIsExternal = source.UserIsExternal;
        // target.UserPasswordFormat = source.UserPasswordFormat;
        // target.UserStartingAliasPath = source.UserStartingAliasPath;
        // target.UserLastLogonInfo = source.UserLastLogonInfo;
        // target.UserIsHidden = source.UserIsHidden;
        // target.UserIsDomain = source.UserIsDomain;
        // target.UserHasAllowedCultures = source.UserHasAllowedCultures;
        // target.UserMfrequired = source.UserMfrequired;
        // target.UserPrivilegeLevel = source.UserPrivilegeLevel;
        // target.UserMftimestep = source.UserMftimestep;
    }
}