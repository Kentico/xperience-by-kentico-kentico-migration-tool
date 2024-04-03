namespace Migration.Toolkit.Source.Model;
// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

public interface ICmsRole: ISourceModel<ICmsRole>
{
    int RoleID { get; }
    string RoleDisplayName { get; }
    string RoleName { get; }
    string? RoleDescription { get; }
    int? SiteID { get; }
    Guid RoleGUID { get; }
    DateTime RoleLastModified { get; }
    bool? RoleIsDomain { get; }    

    static string ISourceModel<ICmsRole>.GetPrimaryKeyName(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsRoleK11.GetPrimaryKeyName(version),
            { Major: 12 } => CmsRoleK12.GetPrimaryKeyName(version),
            { Major: 13 } => CmsRoleK13.GetPrimaryKeyName(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static bool ISourceModel<ICmsRole>.IsAvailable(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsRoleK11.IsAvailable(version),
            { Major: 12 } => CmsRoleK12.IsAvailable(version),
            { Major: 13 } => CmsRoleK13.IsAvailable(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static string ISourceModel<ICmsRole>.TableName => "CMS_Role";
    static string ISourceModel<ICmsRole>.GuidColumnName => "RoleGUID"; //assumtion, class Guid column doesn't change between versions
    static ICmsRole ISourceModel<ICmsRole>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsRoleK11.FromReader(reader, version),
            { Major: 12 } => CmsRoleK12.FromReader(reader, version),
            { Major: 13 } => CmsRoleK13.FromReader(reader, version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
}
public partial record CmsRoleK11(int RoleID, string RoleDisplayName, string RoleName, string? RoleDescription, int? SiteID, Guid RoleGUID, DateTime RoleLastModified, int? RoleGroupID, bool? RoleIsGroupAdministrator, bool? RoleIsDomain): ICmsRole, ISourceModel<CmsRoleK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "RoleID";   
    public static string TableName => "CMS_Role";
    public static string GuidColumnName => "RoleGUID";
    static CmsRoleK11 ISourceModel<CmsRoleK11>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsRoleK11(
            reader.Unbox<int>("RoleID"), reader.Unbox<string>("RoleDisplayName"), reader.Unbox<string>("RoleName"), reader.Unbox<string?>("RoleDescription"), reader.Unbox<int?>("SiteID"), reader.Unbox<Guid>("RoleGUID"), reader.Unbox<DateTime>("RoleLastModified"), reader.Unbox<int?>("RoleGroupID"), reader.Unbox<bool?>("RoleIsGroupAdministrator"), reader.Unbox<bool?>("RoleIsDomain")                
        );
    }
    public static CmsRoleK11 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsRoleK11(
            reader.Unbox<int>("RoleID"), reader.Unbox<string>("RoleDisplayName"), reader.Unbox<string>("RoleName"), reader.Unbox<string?>("RoleDescription"), reader.Unbox<int?>("SiteID"), reader.Unbox<Guid>("RoleGUID"), reader.Unbox<DateTime>("RoleLastModified"), reader.Unbox<int?>("RoleGroupID"), reader.Unbox<bool?>("RoleIsGroupAdministrator"), reader.Unbox<bool?>("RoleIsDomain")                
        );
    }
};
public partial record CmsRoleK12(int RoleID, string RoleDisplayName, string RoleName, string? RoleDescription, int? SiteID, Guid RoleGUID, DateTime RoleLastModified, int? RoleGroupID, bool? RoleIsGroupAdministrator, bool? RoleIsDomain): ICmsRole, ISourceModel<CmsRoleK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "RoleID";   
    public static string TableName => "CMS_Role";
    public static string GuidColumnName => "RoleGUID";
    static CmsRoleK12 ISourceModel<CmsRoleK12>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsRoleK12(
            reader.Unbox<int>("RoleID"), reader.Unbox<string>("RoleDisplayName"), reader.Unbox<string>("RoleName"), reader.Unbox<string?>("RoleDescription"), reader.Unbox<int?>("SiteID"), reader.Unbox<Guid>("RoleGUID"), reader.Unbox<DateTime>("RoleLastModified"), reader.Unbox<int?>("RoleGroupID"), reader.Unbox<bool?>("RoleIsGroupAdministrator"), reader.Unbox<bool?>("RoleIsDomain")                
        );
    }
    public static CmsRoleK12 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsRoleK12(
            reader.Unbox<int>("RoleID"), reader.Unbox<string>("RoleDisplayName"), reader.Unbox<string>("RoleName"), reader.Unbox<string?>("RoleDescription"), reader.Unbox<int?>("SiteID"), reader.Unbox<Guid>("RoleGUID"), reader.Unbox<DateTime>("RoleLastModified"), reader.Unbox<int?>("RoleGroupID"), reader.Unbox<bool?>("RoleIsGroupAdministrator"), reader.Unbox<bool?>("RoleIsDomain")                
        );
    }
};
public partial record CmsRoleK13(int RoleID, string RoleDisplayName, string RoleName, string? RoleDescription, int? SiteID, Guid RoleGUID, DateTime RoleLastModified, bool? RoleIsDomain): ICmsRole, ISourceModel<CmsRoleK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "RoleID";   
    public static string TableName => "CMS_Role";
    public static string GuidColumnName => "RoleGUID";
    static CmsRoleK13 ISourceModel<CmsRoleK13>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsRoleK13(
            reader.Unbox<int>("RoleID"), reader.Unbox<string>("RoleDisplayName"), reader.Unbox<string>("RoleName"), reader.Unbox<string?>("RoleDescription"), reader.Unbox<int?>("SiteID"), reader.Unbox<Guid>("RoleGUID"), reader.Unbox<DateTime>("RoleLastModified"), reader.Unbox<bool?>("RoleIsDomain")                
        );
    }
    public static CmsRoleK13 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsRoleK13(
            reader.Unbox<int>("RoleID"), reader.Unbox<string>("RoleDisplayName"), reader.Unbox<string>("RoleName"), reader.Unbox<string?>("RoleDescription"), reader.Unbox<int?>("SiteID"), reader.Unbox<Guid>("RoleGUID"), reader.Unbox<DateTime>("RoleLastModified"), reader.Unbox<bool?>("RoleIsDomain")                
        );
    }
};

