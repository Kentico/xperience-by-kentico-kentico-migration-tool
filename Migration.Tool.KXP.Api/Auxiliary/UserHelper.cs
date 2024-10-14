using Migration.Tool.KXP.Api.Enums;

namespace Migration.Tool.KXP.Api.Auxiliary;

public class UserHelper
{
    public static int[] PrivilegeLevelsMigratedAsAdminUser => [(int)UserPrivilegeLevelEnum.Editor, (int)UserPrivilegeLevelEnum.Admin, (int)UserPrivilegeLevelEnum.GlobalAdmin];
    public static int[] PrivilegeLevelsMigratedAsMemberUser => [(int)UserPrivilegeLevelEnum.None];
}
