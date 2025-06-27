using CMS.ContentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Workspaces;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common.Helpers;

namespace Migration.Tool.Common.Services;
public class UserService
{
    private HashSet<Guid>? existingUsers;

    private Guid? defaultAdminUserGuid;
    public Guid? DefaultAdminUserGuid
    {
        get
        {
            if (defaultAdminUserGuid.HasValue)
            {
                return defaultAdminUserGuid.Value;
            }

            var admins = UserInfo.Provider.Get().WhereEquals(nameof(UserInfo.UserAdministrationAccess), true)
                .Columns(nameof(UserInfo.UserGUID), nameof(UserInfo.UserName));
            defaultAdminUserGuid = (admins.FirstOrDefault(x => x.UserName == DefaultAdminName) ?? admins.FirstOrDefault())
                ?.UserGUID;
            return defaultAdminUserGuid;
        }
    }

    private const string DefaultAdminName = "administrator";

    public bool UserExists(Guid userGuid)
    {
        existingUsers ??= UserInfo.Provider.Get().Select(x => x.UserGUID).ToHashSet();
        return existingUsers.Contains(userGuid);
    }
}
