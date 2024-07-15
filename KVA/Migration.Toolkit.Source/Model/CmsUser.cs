namespace Migration.Toolkit.Source.Model;
// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

public interface ICmsUser : ISourceModel<ICmsUser>
{
    int UserID { get; }
    string UserName { get; }
    string? FirstName { get; }
    string? MiddleName { get; }
    string? LastName { get; }
    string? FullName { get; }
    string? Email { get; }
    string UserPassword { get; }
    string? PreferredCultureCode { get; }
    string? PreferredUICultureCode { get; }
    bool UserEnabled { get; }
    bool? UserIsExternal { get; }
    string? UserPasswordFormat { get; }
    DateTime? UserCreated { get; }
    DateTime? LastLogon { get; }
    string? UserStartingAliasPath { get; }
    Guid UserGUID { get; }
    DateTime UserLastModified { get; }
    string? UserLastLogonInfo { get; }
    bool? UserIsHidden { get; }
    bool? UserIsDomain { get; }
    bool? UserHasAllowedCultures { get; }
    bool? UserMFRequired { get; }
    int UserPrivilegeLevel { get; }
    string? UserSecurityStamp { get; }
    byte[]? UserMFSecret { get; }
    long? UserMFTimestep { get; }

    static string ISourceModel<ICmsUser>.GetPrimaryKeyName(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsUserK11.GetPrimaryKeyName(version),
            { Major: 12 } => CmsUserK12.GetPrimaryKeyName(version),
            { Major: 13 } => CmsUserK13.GetPrimaryKeyName(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static bool ISourceModel<ICmsUser>.IsAvailable(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsUserK11.IsAvailable(version),
            { Major: 12 } => CmsUserK12.IsAvailable(version),
            { Major: 13 } => CmsUserK13.IsAvailable(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static string ISourceModel<ICmsUser>.TableName => "CMS_User";
    static string ISourceModel<ICmsUser>.GuidColumnName => "UserGUID"; //assumtion, class Guid column doesn't change between versions
    static ICmsUser ISourceModel<ICmsUser>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsUserK11.FromReader(reader, version),
            { Major: 12 } => CmsUserK12.FromReader(reader, version),
            { Major: 13 } => CmsUserK13.FromReader(reader, version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
}
public partial record CmsUserK11(int UserID, string UserName, string? FirstName, string? MiddleName, string? LastName, string? FullName, string? Email, string UserPassword, string? PreferredCultureCode, string? PreferredUICultureCode, bool UserEnabled, bool? UserIsExternal, string? UserPasswordFormat, DateTime? UserCreated, DateTime? LastLogon, string? UserStartingAliasPath, Guid UserGUID, DateTime UserLastModified, string? UserLastLogonInfo, bool? UserIsHidden, string? UserVisibility, bool? UserIsDomain, bool? UserHasAllowedCultures, bool? UserMFRequired, int UserPrivilegeLevel, string? UserSecurityStamp, byte[]? UserMFSecret, long? UserMFTimestep) : ICmsUser, ISourceModel<CmsUserK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "UserID";
    public static string TableName => "CMS_User";
    public static string GuidColumnName => "UserGUID";
    static CmsUserK11 ISourceModel<CmsUserK11>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsUserK11(
            reader.Unbox<int>("UserID"), reader.Unbox<string>("UserName"), reader.Unbox<string?>("FirstName"), reader.Unbox<string?>("MiddleName"), reader.Unbox<string?>("LastName"), reader.Unbox<string?>("FullName"), reader.Unbox<string?>("Email"), reader.Unbox<string>("UserPassword"), reader.Unbox<string?>("PreferredCultureCode"), reader.Unbox<string?>("PreferredUICultureCode"), reader.Unbox<bool>("UserEnabled"), reader.Unbox<bool?>("UserIsExternal"), reader.Unbox<string?>("UserPasswordFormat"), reader.Unbox<DateTime?>("UserCreated"), reader.Unbox<DateTime?>("LastLogon"), reader.Unbox<string?>("UserStartingAliasPath"), reader.Unbox<Guid>("UserGUID"), reader.Unbox<DateTime>("UserLastModified"), reader.Unbox<string?>("UserLastLogonInfo"), reader.Unbox<bool?>("UserIsHidden"), reader.Unbox<string?>("UserVisibility"), reader.Unbox<bool?>("UserIsDomain"), reader.Unbox<bool?>("UserHasAllowedCultures"), reader.Unbox<bool?>("UserMFRequired"), reader.Unbox<int>("UserPrivilegeLevel"), reader.Unbox<string?>("UserSecurityStamp"), reader.Unbox<byte[]?>("UserMFSecret"), reader.Unbox<long?>("UserMFTimestep")
        );
    }
    public static CmsUserK11 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsUserK11(
            reader.Unbox<int>("UserID"), reader.Unbox<string>("UserName"), reader.Unbox<string?>("FirstName"), reader.Unbox<string?>("MiddleName"), reader.Unbox<string?>("LastName"), reader.Unbox<string?>("FullName"), reader.Unbox<string?>("Email"), reader.Unbox<string>("UserPassword"), reader.Unbox<string?>("PreferredCultureCode"), reader.Unbox<string?>("PreferredUICultureCode"), reader.Unbox<bool>("UserEnabled"), reader.Unbox<bool?>("UserIsExternal"), reader.Unbox<string?>("UserPasswordFormat"), reader.Unbox<DateTime?>("UserCreated"), reader.Unbox<DateTime?>("LastLogon"), reader.Unbox<string?>("UserStartingAliasPath"), reader.Unbox<Guid>("UserGUID"), reader.Unbox<DateTime>("UserLastModified"), reader.Unbox<string?>("UserLastLogonInfo"), reader.Unbox<bool?>("UserIsHidden"), reader.Unbox<string?>("UserVisibility"), reader.Unbox<bool?>("UserIsDomain"), reader.Unbox<bool?>("UserHasAllowedCultures"), reader.Unbox<bool?>("UserMFRequired"), reader.Unbox<int>("UserPrivilegeLevel"), reader.Unbox<string?>("UserSecurityStamp"), reader.Unbox<byte[]?>("UserMFSecret"), reader.Unbox<long?>("UserMFTimestep")
        );
    }
};
public partial record CmsUserK12(int UserID, string UserName, string? FirstName, string? MiddleName, string? LastName, string? FullName, string? Email, string UserPassword, string? PreferredCultureCode, string? PreferredUICultureCode, bool UserEnabled, bool? UserIsExternal, string? UserPasswordFormat, DateTime? UserCreated, DateTime? LastLogon, string? UserStartingAliasPath, Guid UserGUID, DateTime UserLastModified, string? UserLastLogonInfo, bool? UserIsHidden, string? UserVisibility, bool? UserIsDomain, bool? UserHasAllowedCultures, bool? UserMFRequired, int UserPrivilegeLevel, string? UserSecurityStamp, byte[]? UserMFSecret, long? UserMFTimestep) : ICmsUser, ISourceModel<CmsUserK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "UserID";
    public static string TableName => "CMS_User";
    public static string GuidColumnName => "UserGUID";
    static CmsUserK12 ISourceModel<CmsUserK12>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsUserK12(
            reader.Unbox<int>("UserID"), reader.Unbox<string>("UserName"), reader.Unbox<string?>("FirstName"), reader.Unbox<string?>("MiddleName"), reader.Unbox<string?>("LastName"), reader.Unbox<string?>("FullName"), reader.Unbox<string?>("Email"), reader.Unbox<string>("UserPassword"), reader.Unbox<string?>("PreferredCultureCode"), reader.Unbox<string?>("PreferredUICultureCode"), reader.Unbox<bool>("UserEnabled"), reader.Unbox<bool?>("UserIsExternal"), reader.Unbox<string?>("UserPasswordFormat"), reader.Unbox<DateTime?>("UserCreated"), reader.Unbox<DateTime?>("LastLogon"), reader.Unbox<string?>("UserStartingAliasPath"), reader.Unbox<Guid>("UserGUID"), reader.Unbox<DateTime>("UserLastModified"), reader.Unbox<string?>("UserLastLogonInfo"), reader.Unbox<bool?>("UserIsHidden"), reader.Unbox<string?>("UserVisibility"), reader.Unbox<bool?>("UserIsDomain"), reader.Unbox<bool?>("UserHasAllowedCultures"), reader.Unbox<bool?>("UserMFRequired"), reader.Unbox<int>("UserPrivilegeLevel"), reader.Unbox<string?>("UserSecurityStamp"), reader.Unbox<byte[]?>("UserMFSecret"), reader.Unbox<long?>("UserMFTimestep")
        );
    }
    public static CmsUserK12 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsUserK12(
            reader.Unbox<int>("UserID"), reader.Unbox<string>("UserName"), reader.Unbox<string?>("FirstName"), reader.Unbox<string?>("MiddleName"), reader.Unbox<string?>("LastName"), reader.Unbox<string?>("FullName"), reader.Unbox<string?>("Email"), reader.Unbox<string>("UserPassword"), reader.Unbox<string?>("PreferredCultureCode"), reader.Unbox<string?>("PreferredUICultureCode"), reader.Unbox<bool>("UserEnabled"), reader.Unbox<bool?>("UserIsExternal"), reader.Unbox<string?>("UserPasswordFormat"), reader.Unbox<DateTime?>("UserCreated"), reader.Unbox<DateTime?>("LastLogon"), reader.Unbox<string?>("UserStartingAliasPath"), reader.Unbox<Guid>("UserGUID"), reader.Unbox<DateTime>("UserLastModified"), reader.Unbox<string?>("UserLastLogonInfo"), reader.Unbox<bool?>("UserIsHidden"), reader.Unbox<string?>("UserVisibility"), reader.Unbox<bool?>("UserIsDomain"), reader.Unbox<bool?>("UserHasAllowedCultures"), reader.Unbox<bool?>("UserMFRequired"), reader.Unbox<int>("UserPrivilegeLevel"), reader.Unbox<string?>("UserSecurityStamp"), reader.Unbox<byte[]?>("UserMFSecret"), reader.Unbox<long?>("UserMFTimestep")
        );
    }
};
public partial record CmsUserK13(int UserID, string UserName, string? FirstName, string? MiddleName, string? LastName, string? FullName, string? Email, string UserPassword, string? PreferredCultureCode, string? PreferredUICultureCode, bool UserEnabled, bool? UserIsExternal, string? UserPasswordFormat, DateTime? UserCreated, DateTime? LastLogon, string? UserStartingAliasPath, Guid UserGUID, DateTime UserLastModified, string? UserLastLogonInfo, bool? UserIsHidden, bool? UserIsDomain, bool? UserHasAllowedCultures, bool? UserMFRequired, int UserPrivilegeLevel, string? UserSecurityStamp, byte[]? UserMFSecret, long? UserMFTimestep) : ICmsUser, ISourceModel<CmsUserK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "UserID";
    public static string TableName => "CMS_User";
    public static string GuidColumnName => "UserGUID";
    static CmsUserK13 ISourceModel<CmsUserK13>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsUserK13(
            reader.Unbox<int>("UserID"), reader.Unbox<string>("UserName"), reader.Unbox<string?>("FirstName"), reader.Unbox<string?>("MiddleName"), reader.Unbox<string?>("LastName"), reader.Unbox<string?>("FullName"), reader.Unbox<string?>("Email"), reader.Unbox<string>("UserPassword"), reader.Unbox<string?>("PreferredCultureCode"), reader.Unbox<string?>("PreferredUICultureCode"), reader.Unbox<bool>("UserEnabled"), reader.Unbox<bool?>("UserIsExternal"), reader.Unbox<string?>("UserPasswordFormat"), reader.Unbox<DateTime?>("UserCreated"), reader.Unbox<DateTime?>("LastLogon"), reader.Unbox<string?>("UserStartingAliasPath"), reader.Unbox<Guid>("UserGUID"), reader.Unbox<DateTime>("UserLastModified"), reader.Unbox<string?>("UserLastLogonInfo"), reader.Unbox<bool?>("UserIsHidden"), reader.Unbox<bool?>("UserIsDomain"), reader.Unbox<bool?>("UserHasAllowedCultures"), reader.Unbox<bool?>("UserMFRequired"), reader.Unbox<int>("UserPrivilegeLevel"), reader.Unbox<string?>("UserSecurityStamp"), reader.Unbox<byte[]?>("UserMFSecret"), reader.Unbox<long?>("UserMFTimestep")
        );
    }
    public static CmsUserK13 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsUserK13(
            reader.Unbox<int>("UserID"), reader.Unbox<string>("UserName"), reader.Unbox<string?>("FirstName"), reader.Unbox<string?>("MiddleName"), reader.Unbox<string?>("LastName"), reader.Unbox<string?>("FullName"), reader.Unbox<string?>("Email"), reader.Unbox<string>("UserPassword"), reader.Unbox<string?>("PreferredCultureCode"), reader.Unbox<string?>("PreferredUICultureCode"), reader.Unbox<bool>("UserEnabled"), reader.Unbox<bool?>("UserIsExternal"), reader.Unbox<string?>("UserPasswordFormat"), reader.Unbox<DateTime?>("UserCreated"), reader.Unbox<DateTime?>("LastLogon"), reader.Unbox<string?>("UserStartingAliasPath"), reader.Unbox<Guid>("UserGUID"), reader.Unbox<DateTime>("UserLastModified"), reader.Unbox<string?>("UserLastLogonInfo"), reader.Unbox<bool?>("UserIsHidden"), reader.Unbox<bool?>("UserIsDomain"), reader.Unbox<bool?>("UserHasAllowedCultures"), reader.Unbox<bool?>("UserMFRequired"), reader.Unbox<int>("UserPrivilegeLevel"), reader.Unbox<string?>("UserSecurityStamp"), reader.Unbox<byte[]?>("UserMFSecret"), reader.Unbox<long?>("UserMFTimestep")
        );
    }
};