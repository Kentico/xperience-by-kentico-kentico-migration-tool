using CMS.Base;

using Microsoft.Extensions.DependencyInjection;

using Migration.Tool.Common;
using Migration.Tool.KXP.Api.Auxiliary;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Helpers;

public class AdminUserHelper
{
    /// <summary>
    ///     maps source user id to target admin user
    /// </summary>
    /// <param name="sourceUserId">source user id</param>
    /// <param name="memberFallbackTargetUserId">in case user was migrated to member, replacement admin user shall be returned</param>
    /// <param name="onAdminNotFound">called when no admin user exists in target instance, but is expected to exist</param>
    /// <returns>Target admin user ID</returns>
    public static int? MapTargetAdminUser(int? sourceUserId, int memberFallbackTargetUserId, Action? onAdminNotFound = null)
    {
        if (sourceUserId is null)
        {
            return null;
        }

        var modelFacade = KsCoreDiExtensions.ServiceProvider.GetRequiredService<ModelFacade>();
        if (modelFacade.SelectById<ICmsUser>(sourceUserId) is { } modifiedByUser)
        {
            if (UserHelper.PrivilegeLevelsMigratedAsAdminUser.Contains(modifiedByUser.UserPrivilegeLevel))
            {
                var primaryKeyMappingContext = KsCoreDiExtensions.ServiceProvider.GetRequiredService<IPrimaryKeyMappingContext>();
                switch (primaryKeyMappingContext.MapSourceId<ICmsUser>(u => u.UserID, sourceUserId))
                {
                    case { Success: true, MappedId: { } targetUserId }:
                        return targetUserId;
                    default:
                    {
                        onAdminNotFound?.Invoke();
                        return null;
                    }
                }
            }

            return CMSActionContext.CurrentUser.UserID;
        }

        // user not found, setting fallback
        return CMSActionContext.CurrentUser.UserID;
    }
}
