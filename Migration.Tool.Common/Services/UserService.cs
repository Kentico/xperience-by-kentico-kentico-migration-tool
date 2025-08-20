using CMS.Membership;

namespace Migration.Tool.Common.Services;
public class UserService
{
    private const string DefaultAdminName = "administrator";

    private HashSet<Guid>? existingUsers;

    private readonly Lazy<UserInfo> defaultAdminUser = new(() =>
        UserInfo.Provider.Get()
            .WhereEquals(nameof(UserInfo.UserAdministrationAccess), true)
            .And().WhereEquals(nameof(UserInfo.UserName), DefaultAdminName)
            .FirstOrDefault() ?? throw new Exception($"Default administrator not found ({nameof(UserInfo.UserName)} {DefaultAdminName}, {nameof(UserInfo.UserAdministrationAccess)} true")
    );

    public UserInfo DefaultAdminUser => defaultAdminUser.Value;

    public bool UserExists(Guid userGuid)
    {
        existingUsers ??= UserInfo.Provider.Get().Select(x => x.UserGUID).ToHashSet();
        return existingUsers.Contains(userGuid);
    }
}
