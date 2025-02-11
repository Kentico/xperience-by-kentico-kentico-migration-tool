// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Tool.Common;

namespace Migration.Tool.Source.Model;
public partial interface ICmsClassSite : ISourceModel<ICmsClassSite>
{
    int ClassID { get; }
    int SiteID { get; }

    static string ISourceModel<ICmsClassSite>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsClassSiteK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsClassSiteK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsClassSiteK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsClassSite>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsClassSiteK11.IsAvailable(version),
        { Major: 12 } => CmsClassSiteK12.IsAvailable(version),
        { Major: 13 } => CmsClassSiteK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsClassSite>.TableName => "CMS_ClassSite";
    static string ISourceModel<ICmsClassSite>.GuidColumnName => ""; //assumtion, class Guid column doesn't change between versions
    static ICmsClassSite ISourceModel<ICmsClassSite>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsClassSiteK11.FromReader(reader, version),
        { Major: 12 } => CmsClassSiteK12.FromReader(reader, version),
        { Major: 13 } => CmsClassSiteK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsClassSiteK11(int ClassID, int SiteID) : ICmsClassSite, ISourceModel<CmsClassSiteK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ClassID";
    public static string TableName => "CMS_ClassSite";
    public static string GuidColumnName => "";
    static CmsClassSiteK11 ISourceModel<CmsClassSiteK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ClassID"), reader.Unbox<int>("SiteID")
        );
    public static CmsClassSiteK11 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ClassID"), reader.Unbox<int>("SiteID")
        );
};
public partial record CmsClassSiteK12(int ClassID, int SiteID) : ICmsClassSite, ISourceModel<CmsClassSiteK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ClassID";
    public static string TableName => "CMS_ClassSite";
    public static string GuidColumnName => "";
    static CmsClassSiteK12 ISourceModel<CmsClassSiteK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ClassID"), reader.Unbox<int>("SiteID")
        );
    public static CmsClassSiteK12 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ClassID"), reader.Unbox<int>("SiteID")
        );
};
public partial record CmsClassSiteK13(int ClassID, int SiteID) : ICmsClassSite, ISourceModel<CmsClassSiteK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ClassID";
    public static string TableName => "CMS_ClassSite";
    public static string GuidColumnName => "";
    static CmsClassSiteK13 ISourceModel<CmsClassSiteK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ClassID"), reader.Unbox<int>("SiteID")
        );
    public static CmsClassSiteK13 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ClassID"), reader.Unbox<int>("SiteID")
        );
};

