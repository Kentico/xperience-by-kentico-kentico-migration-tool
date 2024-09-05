// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;
public partial interface ICmsResource : ISourceModel<ICmsResource>
{
    int ResourceID { get; }
    string ResourceDisplayName { get; }
    string ResourceName { get; }
    string? ResourceDescription { get; }
    bool? ShowInDevelopment { get; }
    string? ResourceURL { get; }
    Guid ResourceGUID { get; }
    DateTime ResourceLastModified { get; }
    bool? ResourceIsInDevelopment { get; }
    bool? ResourceHasFiles { get; }
    string? ResourceVersion { get; }
    string? ResourceAuthor { get; }
    string? ResourceInstallationState { get; }
    string? ResourceInstalledVersion { get; }

    static string ISourceModel<ICmsResource>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsResourceK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsResourceK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsResourceK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsResource>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsResourceK11.IsAvailable(version),
        { Major: 12 } => CmsResourceK12.IsAvailable(version),
        { Major: 13 } => CmsResourceK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsResource>.TableName => "CMS_Resource";
    static string ISourceModel<ICmsResource>.GuidColumnName => "ResourceGUID"; //assumtion, class Guid column doesn't change between versions
    static ICmsResource ISourceModel<ICmsResource>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsResourceK11.FromReader(reader, version),
        { Major: 12 } => CmsResourceK12.FromReader(reader, version),
        { Major: 13 } => CmsResourceK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsResourceK11(int ResourceID, string ResourceDisplayName, string ResourceName, string? ResourceDescription, bool? ShowInDevelopment, string? ResourceURL, Guid ResourceGUID, DateTime ResourceLastModified, bool? ResourceIsInDevelopment, bool? ResourceHasFiles, string? ResourceVersion, string? ResourceAuthor, string? ResourceInstallationState, string? ResourceInstalledVersion) : ICmsResource, ISourceModel<CmsResourceK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ResourceID";
    public static string TableName => "CMS_Resource";
    public static string GuidColumnName => "ResourceGUID";
    static CmsResourceK11 ISourceModel<CmsResourceK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ResourceID"), reader.Unbox<string>("ResourceDisplayName"), reader.Unbox<string>("ResourceName"), reader.Unbox<string?>("ResourceDescription"), reader.Unbox<bool?>("ShowInDevelopment"), reader.Unbox<string?>("ResourceURL"), reader.Unbox<Guid>("ResourceGUID"), reader.Unbox<DateTime>("ResourceLastModified"), reader.Unbox<bool?>("ResourceIsInDevelopment"), reader.Unbox<bool?>("ResourceHasFiles"), reader.Unbox<string?>("ResourceVersion"), reader.Unbox<string?>("ResourceAuthor"), reader.Unbox<string?>("ResourceInstallationState"), reader.Unbox<string?>("ResourceInstalledVersion")
        );
    public static CmsResourceK11 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ResourceID"), reader.Unbox<string>("ResourceDisplayName"), reader.Unbox<string>("ResourceName"), reader.Unbox<string?>("ResourceDescription"), reader.Unbox<bool?>("ShowInDevelopment"), reader.Unbox<string?>("ResourceURL"), reader.Unbox<Guid>("ResourceGUID"), reader.Unbox<DateTime>("ResourceLastModified"), reader.Unbox<bool?>("ResourceIsInDevelopment"), reader.Unbox<bool?>("ResourceHasFiles"), reader.Unbox<string?>("ResourceVersion"), reader.Unbox<string?>("ResourceAuthor"), reader.Unbox<string?>("ResourceInstallationState"), reader.Unbox<string?>("ResourceInstalledVersion")
        );
};
public partial record CmsResourceK12(int ResourceID, string ResourceDisplayName, string ResourceName, string? ResourceDescription, bool? ShowInDevelopment, string? ResourceURL, Guid ResourceGUID, DateTime ResourceLastModified, bool? ResourceIsInDevelopment, bool? ResourceHasFiles, string? ResourceVersion, string? ResourceAuthor, string? ResourceInstallationState, string? ResourceInstalledVersion) : ICmsResource, ISourceModel<CmsResourceK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ResourceID";
    public static string TableName => "CMS_Resource";
    public static string GuidColumnName => "ResourceGUID";
    static CmsResourceK12 ISourceModel<CmsResourceK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ResourceID"), reader.Unbox<string>("ResourceDisplayName"), reader.Unbox<string>("ResourceName"), reader.Unbox<string?>("ResourceDescription"), reader.Unbox<bool?>("ShowInDevelopment"), reader.Unbox<string?>("ResourceURL"), reader.Unbox<Guid>("ResourceGUID"), reader.Unbox<DateTime>("ResourceLastModified"), reader.Unbox<bool?>("ResourceIsInDevelopment"), reader.Unbox<bool?>("ResourceHasFiles"), reader.Unbox<string?>("ResourceVersion"), reader.Unbox<string?>("ResourceAuthor"), reader.Unbox<string?>("ResourceInstallationState"), reader.Unbox<string?>("ResourceInstalledVersion")
        );
    public static CmsResourceK12 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ResourceID"), reader.Unbox<string>("ResourceDisplayName"), reader.Unbox<string>("ResourceName"), reader.Unbox<string?>("ResourceDescription"), reader.Unbox<bool?>("ShowInDevelopment"), reader.Unbox<string?>("ResourceURL"), reader.Unbox<Guid>("ResourceGUID"), reader.Unbox<DateTime>("ResourceLastModified"), reader.Unbox<bool?>("ResourceIsInDevelopment"), reader.Unbox<bool?>("ResourceHasFiles"), reader.Unbox<string?>("ResourceVersion"), reader.Unbox<string?>("ResourceAuthor"), reader.Unbox<string?>("ResourceInstallationState"), reader.Unbox<string?>("ResourceInstalledVersion")
        );
};
public partial record CmsResourceK13(int ResourceID, string ResourceDisplayName, string ResourceName, string? ResourceDescription, bool? ShowInDevelopment, string? ResourceURL, Guid ResourceGUID, DateTime ResourceLastModified, bool? ResourceIsInDevelopment, bool? ResourceHasFiles, string? ResourceVersion, string? ResourceAuthor, string? ResourceInstallationState, string? ResourceInstalledVersion) : ICmsResource, ISourceModel<CmsResourceK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ResourceID";
    public static string TableName => "CMS_Resource";
    public static string GuidColumnName => "ResourceGUID";
    static CmsResourceK13 ISourceModel<CmsResourceK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ResourceID"), reader.Unbox<string>("ResourceDisplayName"), reader.Unbox<string>("ResourceName"), reader.Unbox<string?>("ResourceDescription"), reader.Unbox<bool?>("ShowInDevelopment"), reader.Unbox<string?>("ResourceURL"), reader.Unbox<Guid>("ResourceGUID"), reader.Unbox<DateTime>("ResourceLastModified"), reader.Unbox<bool?>("ResourceIsInDevelopment"), reader.Unbox<bool?>("ResourceHasFiles"), reader.Unbox<string?>("ResourceVersion"), reader.Unbox<string?>("ResourceAuthor"), reader.Unbox<string?>("ResourceInstallationState"), reader.Unbox<string?>("ResourceInstalledVersion")
        );
    public static CmsResourceK13 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ResourceID"), reader.Unbox<string>("ResourceDisplayName"), reader.Unbox<string>("ResourceName"), reader.Unbox<string?>("ResourceDescription"), reader.Unbox<bool?>("ShowInDevelopment"), reader.Unbox<string?>("ResourceURL"), reader.Unbox<Guid>("ResourceGUID"), reader.Unbox<DateTime>("ResourceLastModified"), reader.Unbox<bool?>("ResourceIsInDevelopment"), reader.Unbox<bool?>("ResourceHasFiles"), reader.Unbox<string?>("ResourceVersion"), reader.Unbox<string?>("ResourceAuthor"), reader.Unbox<string?>("ResourceInstallationState"), reader.Unbox<string?>("ResourceInstalledVersion")
        );
};

