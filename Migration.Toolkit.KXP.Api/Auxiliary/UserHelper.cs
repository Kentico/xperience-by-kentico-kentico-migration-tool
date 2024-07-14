namespace Migration.Toolkit.KXP.Api.Auxiliary;

using Migration.Toolkit.KXP.Api.Enums;

public class UserHelper
{
    public static int[] PrivilegeLevelsMigratedAsAdminUser => [(int)UserPrivilegeLevelEnum.Editor, (int)UserPrivilegeLevelEnum.Admin, (int)UserPrivilegeLevelEnum.GlobalAdmin];
    public static int[] PrivilegeLevelsMigratedAsMemberUser => [(int)UserPrivilegeLevelEnum.None];
}