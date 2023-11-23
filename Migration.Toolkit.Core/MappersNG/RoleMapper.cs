namespace Migration.Toolkit.Core.MappersNG;

using CMS.Membership;
using Kentico.Xperience.UMT.Model;
using Migration.Toolkit.Core.Abstractions;


public record RoleMapperSource();
public class RoleMapper: UmtMapperBase<RoleMapperSource>
{
    protected override IEnumerable<IUmtModel> MapInternal(RoleMapperSource source)
    {
        var roleInfo = new RoleInfo
        {
            RoleDisplayName = null,
            RoleID = 0,
            RoleDescription = null,
            RoleName = null,
            RoleGUID = default,
            RoleLastModified = default
        };
        var userRoleInfo = new UserRoleInfo { UserID = 0, RoleID = 0 };

        var applicationPermission = new ApplicationPermissionInfo
        {
            ApplicationPermissionID = 0,
            RoleID = 0,
            ApplicationName = null,
            PermissionName = null,
            ApplicationPermissionGuid = default
        };

        yield return default;
    }
}