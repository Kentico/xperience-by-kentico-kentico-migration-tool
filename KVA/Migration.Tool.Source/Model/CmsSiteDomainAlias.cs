// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Tool.Common;

namespace Migration.Tool.Source.Model;

public partial interface ICmsSiteDomainAlias : ISourceModel<ICmsSiteDomainAlias>
{
    int SiteDomainAliasID { get; }
    string? SiteDomainAliasName { get; }
    int SiteID { get; }
    string? SiteDefaultVisitorCulture { get; }
    Guid? SiteDomainGUID { get; }

    static string ISourceModel<ICmsSiteDomainAlias>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsSiteDomainAliasK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsSiteDomainAliasK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsSiteDomainAliasK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsSiteDomainAlias>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsSiteDomainAliasK11.IsAvailable(version),
        { Major: 12 } => CmsSiteDomainAliasK12.IsAvailable(version),
        { Major: 13 } => CmsSiteDomainAliasK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsSiteDomainAlias>.TableName => "CMS_SiteDomainAlias";
    static string ISourceModel<ICmsSiteDomainAlias>.GuidColumnName => "SiteDomainGUID";
    static ICmsSiteDomainAlias ISourceModel<ICmsSiteDomainAlias>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsSiteDomainAliasK11.FromReader(reader, version),
        { Major: 12 } => CmsSiteDomainAliasK12.FromReader(reader, version),
        { Major: 13 } => CmsSiteDomainAliasK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsSiteDomainAliasK11(int SiteDomainAliasID, string? SiteDomainAliasName, int SiteID, string? SiteDefaultVisitorCulture, Guid? SiteDomainGUID) : ICmsSiteDomainAlias, ISourceModel<CmsSiteDomainAliasK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "SiteDomainAliasID";
    public static string TableName => "CMS_SiteDomainAlias";
    public static string GuidColumnName => "SiteDomainGUID";
    static CmsSiteDomainAliasK11 ISourceModel<CmsSiteDomainAliasK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SiteDomainAliasID"), reader.Unbox<string?>("SiteDomainAliasName"), reader.Unbox<int>("SiteID"), reader.Unbox<string?>("SiteDefaultVisitorCulture"), reader.Unbox<Guid?>("SiteDomainGUID")
        );
    public static CmsSiteDomainAliasK11 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SiteDomainAliasID"), reader.Unbox<string?>("SiteDomainAliasName"), reader.Unbox<int>("SiteID"), reader.Unbox<string?>("SiteDefaultVisitorCulture"), reader.Unbox<Guid?>("SiteDomainGUID")
        );
};
public partial record CmsSiteDomainAliasK12(int SiteDomainAliasID, string? SiteDomainAliasName, int SiteID, string? SiteDefaultVisitorCulture, Guid? SiteDomainGUID) : ICmsSiteDomainAlias, ISourceModel<CmsSiteDomainAliasK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "SiteDomainAliasID";
    public static string TableName => "CMS_SiteDomainAlias";
    public static string GuidColumnName => "SiteDomainGUID";
    static CmsSiteDomainAliasK12 ISourceModel<CmsSiteDomainAliasK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SiteDomainAliasID"), reader.Unbox<string?>("SiteDomainAliasName"), reader.Unbox<int>("SiteID"), reader.Unbox<string?>("SiteDefaultVisitorCulture"), reader.Unbox<Guid?>("SiteDomainGUID")
        );
    public static CmsSiteDomainAliasK12 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SiteDomainAliasID"), reader.Unbox<string?>("SiteDomainAliasName"), reader.Unbox<int>("SiteID"), reader.Unbox<string?>("SiteDefaultVisitorCulture"), reader.Unbox<Guid?>("SiteDomainGUID")
        );
};
public partial record CmsSiteDomainAliasK13(int SiteDomainAliasID, string? SiteDomainAliasName, int SiteID, string? SiteDefaultVisitorCulture, Guid? SiteDomainGUID) : ICmsSiteDomainAlias, ISourceModel<CmsSiteDomainAliasK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "SiteDomainAliasID";
    public static string TableName => "CMS_SiteDomainAlias";
    public static string GuidColumnName => "SiteDomainGUID";
    static CmsSiteDomainAliasK13 ISourceModel<CmsSiteDomainAliasK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SiteDomainAliasID"), reader.Unbox<string?>("SiteDomainAliasName"), reader.Unbox<int>("SiteID"), reader.Unbox<string?>("SiteDefaultVisitorCulture"), reader.Unbox<Guid?>("SiteDomainGUID")
        );
    public static CmsSiteDomainAliasK13 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SiteDomainAliasID"), reader.Unbox<string?>("SiteDomainAliasName"), reader.Unbox<int>("SiteID"), reader.Unbox<string?>("SiteDefaultVisitorCulture"), reader.Unbox<Guid?>("SiteDomainGUID")
        );
};
