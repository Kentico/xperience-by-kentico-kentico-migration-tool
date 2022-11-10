namespace Migration.Toolkit.KXP.Api.Enums;

/// <summary>
/// copy from Kentico Xperience 13
/// </summary>
public enum UserPrivilegeLevelEnum
{
    /// <summary>User has no privilege level</summary>
    None = 0,
    /// <summary>User is able to use administration interface</summary>
    Editor = 1,
    /// <summary>
    /// User can use all applications except the global applications and functionality
    /// </summary>
    Admin = 2,
    /// <summary>
    /// User can use all applications and functionality without any exceptions
    /// </summary>
    GlobalAdmin = 3,
}