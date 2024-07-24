// ReSharper disable InconsistentNaming

using System.Data;

using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;

public interface ICmsPageFormerUrlPath : ISourceModel<ICmsPageFormerUrlPath>
{
    static string ISourceModel<ICmsPageFormerUrlPath>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsPageFormerUrlPathK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsPageFormerUrlPathK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsPageFormerUrlPathK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };

    static bool ISourceModel<ICmsPageFormerUrlPath>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsPageFormerUrlPathK11.IsAvailable(version),
        { Major: 12 } => CmsPageFormerUrlPathK12.IsAvailable(version),
        { Major: 13 } => CmsPageFormerUrlPathK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };

    static string ISourceModel<ICmsPageFormerUrlPath>.TableName => "CMS_PageFormerUrlPath";
    static string ISourceModel<ICmsPageFormerUrlPath>.GuidColumnName => ""; //assumtion, class Guid column doesn't change between versions

    static ICmsPageFormerUrlPath ISourceModel<ICmsPageFormerUrlPath>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsPageFormerUrlPathK11.FromReader(reader, version),
        { Major: 12 } => CmsPageFormerUrlPathK12.FromReader(reader, version),
        { Major: 13 } => CmsPageFormerUrlPathK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}

public record CmsPageFormerUrlPathK11 : ICmsPageFormerUrlPath, ISourceModel<CmsPageFormerUrlPathK11>
{
    public static bool IsAvailable(SemanticVersion version) => false;
    public static string GetPrimaryKeyName(SemanticVersion version) => "";
    public static string TableName => "CMS_PageFormerUrlPath";
    public static string GuidColumnName => "";

    static CmsPageFormerUrlPathK11 ISourceModel<CmsPageFormerUrlPathK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
    );

    public static CmsPageFormerUrlPathK11 FromReader(IDataReader reader, SemanticVersion version) => new(
    );
}

public record CmsPageFormerUrlPathK12 : ICmsPageFormerUrlPath, ISourceModel<CmsPageFormerUrlPathK12>
{
    public static bool IsAvailable(SemanticVersion version) => false;
    public static string GetPrimaryKeyName(SemanticVersion version) => "";
    public static string TableName => "CMS_PageFormerUrlPath";
    public static string GuidColumnName => "";

    static CmsPageFormerUrlPathK12 ISourceModel<CmsPageFormerUrlPathK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
    );

    public static CmsPageFormerUrlPathK12 FromReader(IDataReader reader, SemanticVersion version) => new(
    );
}

public record CmsPageFormerUrlPathK13(
    int PageFormerUrlPathID,
    string PageFormerUrlPathUrlPath,
    string PageFormerUrlPathUrlPathHash,
    string PageFormerUrlPathCulture,
    int PageFormerUrlPathNodeID,
    int PageFormerUrlPathSiteID,
    DateTime PageFormerUrlPathLastModified) : ICmsPageFormerUrlPath, ISourceModel<CmsPageFormerUrlPathK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "PageFormerUrlPathID";
    public static string TableName => "CMS_PageFormerUrlPath";
    public static string GuidColumnName => "";

    static CmsPageFormerUrlPathK13 ISourceModel<CmsPageFormerUrlPathK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("PageFormerUrlPathID"), reader.Unbox<string>("PageFormerUrlPathUrlPath"), reader.Unbox<string>("PageFormerUrlPathUrlPathHash"), reader.Unbox<string>("PageFormerUrlPathCulture"), reader.Unbox<int>("PageFormerUrlPathNodeID"),
        reader.Unbox<int>("PageFormerUrlPathSiteID"), reader.Unbox<DateTime>("PageFormerUrlPathLastModified")
    );

    public static CmsPageFormerUrlPathK13 FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("PageFormerUrlPathID"), reader.Unbox<string>("PageFormerUrlPathUrlPath"), reader.Unbox<string>("PageFormerUrlPathUrlPathHash"), reader.Unbox<string>("PageFormerUrlPathCulture"), reader.Unbox<int>("PageFormerUrlPathNodeID"),
        reader.Unbox<int>("PageFormerUrlPathSiteID"), reader.Unbox<DateTime>("PageFormerUrlPathLastModified")
    );
}
