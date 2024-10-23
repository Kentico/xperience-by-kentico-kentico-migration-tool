// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Tool.Common;

namespace Migration.Tool.Source.Model;
public partial interface ICmsSite : ISourceModel<ICmsSite>
{
    int SiteID { get; }
    string SiteName { get; }
    string SiteDisplayName { get; }
    string? SiteDescription { get; }
    string SiteStatus { get; }
    string SiteDomainName { get; }
    string? SiteDefaultVisitorCulture { get; }
    Guid SiteGUID { get; }
    DateTime SiteLastModified { get; }

    static string ISourceModel<ICmsSite>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsSiteK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsSiteK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsSiteK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsSite>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsSiteK11.IsAvailable(version),
        { Major: 12 } => CmsSiteK12.IsAvailable(version),
        { Major: 13 } => CmsSiteK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsSite>.TableName => "CMS_Site";
    static string ISourceModel<ICmsSite>.GuidColumnName => ""; //assumtion, class Guid column doesn't change between versions
    static ICmsSite ISourceModel<ICmsSite>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsSiteK11.FromReader(reader, version),
        { Major: 12 } => CmsSiteK12.FromReader(reader, version),
        { Major: 13 } => CmsSiteK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsSiteK11(int SiteID, string SiteName, string SiteDisplayName, string? SiteDescription, string SiteStatus, string SiteDomainName, int? SiteDefaultStylesheetID, string? SiteDefaultVisitorCulture, int? SiteDefaultEditorStylesheet, Guid SiteGUID, DateTime SiteLastModified, bool? SiteIsOffline, string? SiteOfflineRedirectURL, string? SiteOfflineMessage, string? SitePresentationURL, bool? SiteIsContentOnly) : ICmsSite, ISourceModel<CmsSiteK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "SiteID";
    public static string TableName => "CMS_Site";
    public static string GuidColumnName => "";
    static CmsSiteK11 ISourceModel<CmsSiteK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SiteID"), reader.Unbox<string>("SiteName"), reader.Unbox<string>("SiteDisplayName"), reader.Unbox<string?>("SiteDescription"), reader.Unbox<string>("SiteStatus"), reader.Unbox<string>("SiteDomainName"), reader.Unbox<int?>("SiteDefaultStylesheetID"), reader.Unbox<string?>("SiteDefaultVisitorCulture"), reader.Unbox<int?>("SiteDefaultEditorStylesheet"), reader.Unbox<Guid>("SiteGUID"), reader.Unbox<DateTime>("SiteLastModified"), reader.Unbox<bool?>("SiteIsOffline"), reader.Unbox<string?>("SiteOfflineRedirectURL"), reader.Unbox<string?>("SiteOfflineMessage"), reader.Unbox<string?>("SitePresentationURL"), reader.Unbox<bool?>("SiteIsContentOnly")
        );
    public static CmsSiteK11 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SiteID"), reader.Unbox<string>("SiteName"), reader.Unbox<string>("SiteDisplayName"), reader.Unbox<string?>("SiteDescription"), reader.Unbox<string>("SiteStatus"), reader.Unbox<string>("SiteDomainName"), reader.Unbox<int?>("SiteDefaultStylesheetID"), reader.Unbox<string?>("SiteDefaultVisitorCulture"), reader.Unbox<int?>("SiteDefaultEditorStylesheet"), reader.Unbox<Guid>("SiteGUID"), reader.Unbox<DateTime>("SiteLastModified"), reader.Unbox<bool?>("SiteIsOffline"), reader.Unbox<string?>("SiteOfflineRedirectURL"), reader.Unbox<string?>("SiteOfflineMessage"), reader.Unbox<string?>("SitePresentationURL"), reader.Unbox<bool?>("SiteIsContentOnly")
        );
};
public partial record CmsSiteK12(int SiteID, string SiteName, string SiteDisplayName, string? SiteDescription, string SiteStatus, string SiteDomainName, int? SiteDefaultStylesheetID, string? SiteDefaultVisitorCulture, int? SiteDefaultEditorStylesheet, Guid SiteGUID, DateTime SiteLastModified, bool? SiteIsOffline, string? SiteOfflineRedirectURL, string? SiteOfflineMessage, string? SitePresentationURL, bool? SiteIsContentOnly) : ICmsSite, ISourceModel<CmsSiteK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "SiteID";
    public static string TableName => "CMS_Site";
    public static string GuidColumnName => "";
    static CmsSiteK12 ISourceModel<CmsSiteK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SiteID"), reader.Unbox<string>("SiteName"), reader.Unbox<string>("SiteDisplayName"), reader.Unbox<string?>("SiteDescription"), reader.Unbox<string>("SiteStatus"), reader.Unbox<string>("SiteDomainName"), reader.Unbox<int?>("SiteDefaultStylesheetID"), reader.Unbox<string?>("SiteDefaultVisitorCulture"), reader.Unbox<int?>("SiteDefaultEditorStylesheet"), reader.Unbox<Guid>("SiteGUID"), reader.Unbox<DateTime>("SiteLastModified"), reader.Unbox<bool?>("SiteIsOffline"), reader.Unbox<string?>("SiteOfflineRedirectURL"), reader.Unbox<string?>("SiteOfflineMessage"), reader.Unbox<string?>("SitePresentationURL"), reader.Unbox<bool?>("SiteIsContentOnly")
        );
    public static CmsSiteK12 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SiteID"), reader.Unbox<string>("SiteName"), reader.Unbox<string>("SiteDisplayName"), reader.Unbox<string?>("SiteDescription"), reader.Unbox<string>("SiteStatus"), reader.Unbox<string>("SiteDomainName"), reader.Unbox<int?>("SiteDefaultStylesheetID"), reader.Unbox<string?>("SiteDefaultVisitorCulture"), reader.Unbox<int?>("SiteDefaultEditorStylesheet"), reader.Unbox<Guid>("SiteGUID"), reader.Unbox<DateTime>("SiteLastModified"), reader.Unbox<bool?>("SiteIsOffline"), reader.Unbox<string?>("SiteOfflineRedirectURL"), reader.Unbox<string?>("SiteOfflineMessage"), reader.Unbox<string?>("SitePresentationURL"), reader.Unbox<bool?>("SiteIsContentOnly")
        );
};
public partial record CmsSiteK13(int SiteID, string SiteName, string SiteDisplayName, string? SiteDescription, string SiteStatus, string SiteDomainName, string? SiteDefaultVisitorCulture, Guid SiteGUID, DateTime SiteLastModified, string SitePresentationURL) : ICmsSite, ISourceModel<CmsSiteK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "SiteID";
    public static string TableName => "CMS_Site";
    public static string GuidColumnName => "";
    static CmsSiteK13 ISourceModel<CmsSiteK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SiteID"), reader.Unbox<string>("SiteName"), reader.Unbox<string>("SiteDisplayName"), reader.Unbox<string?>("SiteDescription"), reader.Unbox<string>("SiteStatus"), reader.Unbox<string>("SiteDomainName"), reader.Unbox<string?>("SiteDefaultVisitorCulture"), reader.Unbox<Guid>("SiteGUID"), reader.Unbox<DateTime>("SiteLastModified"), reader.Unbox<string>("SitePresentationURL")
        );
    public static CmsSiteK13 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("SiteID"), reader.Unbox<string>("SiteName"), reader.Unbox<string>("SiteDisplayName"), reader.Unbox<string?>("SiteDescription"), reader.Unbox<string>("SiteStatus"), reader.Unbox<string>("SiteDomainName"), reader.Unbox<string?>("SiteDefaultVisitorCulture"), reader.Unbox<Guid>("SiteGUID"), reader.Unbox<DateTime>("SiteLastModified"), reader.Unbox<string>("SitePresentationURL")
        );
};

