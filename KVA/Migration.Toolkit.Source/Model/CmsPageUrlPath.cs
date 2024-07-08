namespace Migration.Toolkit.Source.Model;
// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

public interface ICmsPageUrlPath : ISourceModel<ICmsPageUrlPath>
{


    static string ISourceModel<ICmsPageUrlPath>.GetPrimaryKeyName(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsPageUrlPathK11.GetPrimaryKeyName(version),
            { Major: 12 } => CmsPageUrlPathK12.GetPrimaryKeyName(version),
            { Major: 13 } => CmsPageUrlPathK13.GetPrimaryKeyName(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static bool ISourceModel<ICmsPageUrlPath>.IsAvailable(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsPageUrlPathK11.IsAvailable(version),
            { Major: 12 } => CmsPageUrlPathK12.IsAvailable(version),
            { Major: 13 } => CmsPageUrlPathK13.IsAvailable(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static string ISourceModel<ICmsPageUrlPath>.TableName => "CMS_PageUrlPath";
    static string ISourceModel<ICmsPageUrlPath>.GuidColumnName => ""; //assumtion, class Guid column doesn't change between versions
    static ICmsPageUrlPath ISourceModel<ICmsPageUrlPath>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsPageUrlPathK11.FromReader(reader, version),
            { Major: 12 } => CmsPageUrlPathK12.FromReader(reader, version),
            { Major: 13 } => CmsPageUrlPathK13.FromReader(reader, version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
}
public partial record CmsPageUrlPathK11() : ICmsPageUrlPath, ISourceModel<CmsPageUrlPathK11>
{
    public static bool IsAvailable(SemanticVersion version) => false;
    public static string GetPrimaryKeyName(SemanticVersion version) => "";
    public static string TableName => "CMS_PageUrlPath";
    public static string GuidColumnName => "";
    static CmsPageUrlPathK11 ISourceModel<CmsPageUrlPathK11>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsPageUrlPathK11(

        );
    }
    public static CmsPageUrlPathK11 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsPageUrlPathK11(

        );
    }
};
public partial record CmsPageUrlPathK12() : ICmsPageUrlPath, ISourceModel<CmsPageUrlPathK12>
{
    public static bool IsAvailable(SemanticVersion version) => false;
    public static string GetPrimaryKeyName(SemanticVersion version) => "";
    public static string TableName => "CMS_PageUrlPath";
    public static string GuidColumnName => "";
    static CmsPageUrlPathK12 ISourceModel<CmsPageUrlPathK12>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsPageUrlPathK12(

        );
    }
    public static CmsPageUrlPathK12 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsPageUrlPathK12(

        );
    }
};
public partial record CmsPageUrlPathK13(int PageUrlPathID, Guid PageUrlPathGUID, string PageUrlPathCulture, int PageUrlPathNodeID, string PageUrlPathUrlPath, string PageUrlPathUrlPathHash, int PageUrlPathSiteID, DateTime PageUrlPathLastModified) : ICmsPageUrlPath, ISourceModel<CmsPageUrlPathK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "PageUrlPathID";
    public static string TableName => "CMS_PageUrlPath";
    public static string GuidColumnName => "PageUrlPathGUID";
    static CmsPageUrlPathK13 ISourceModel<CmsPageUrlPathK13>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsPageUrlPathK13(
            reader.Unbox<int>("PageUrlPathID"), reader.Unbox<Guid>("PageUrlPathGUID"), reader.Unbox<string>("PageUrlPathCulture"), reader.Unbox<int>("PageUrlPathNodeID"), reader.Unbox<string>("PageUrlPathUrlPath"), reader.Unbox<string>("PageUrlPathUrlPathHash"), reader.Unbox<int>("PageUrlPathSiteID"), reader.Unbox<DateTime>("PageUrlPathLastModified")
        );
    }
    public static CmsPageUrlPathK13 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsPageUrlPathK13(
            reader.Unbox<int>("PageUrlPathID"), reader.Unbox<Guid>("PageUrlPathGUID"), reader.Unbox<string>("PageUrlPathCulture"), reader.Unbox<int>("PageUrlPathNodeID"), reader.Unbox<string>("PageUrlPathUrlPath"), reader.Unbox<string>("PageUrlPathUrlPathHash"), reader.Unbox<int>("PageUrlPathSiteID"), reader.Unbox<DateTime>("PageUrlPathLastModified")
        );
    }
};