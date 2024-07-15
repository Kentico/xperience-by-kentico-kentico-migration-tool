namespace Migration.Toolkit.Source.Model;
// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

public interface ICmsCategory : ISourceModel<ICmsCategory>
{
    int CategoryID { get; }
    string CategoryDisplayName { get; }
    string? CategoryName { get; }
    string? CategoryDescription { get; }
    int? CategoryCount { get; }
    bool CategoryEnabled { get; }
    int? CategoryUserID { get; }
    Guid CategoryGUID { get; }
    DateTime CategoryLastModified { get; }
    int? CategorySiteID { get; }
    int? CategoryParentID { get; }
    string? CategoryIDPath { get; }
    string? CategoryNamePath { get; }
    int? CategoryLevel { get; }
    int? CategoryOrder { get; }

    static string ISourceModel<ICmsCategory>.GetPrimaryKeyName(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsCategoryK11.GetPrimaryKeyName(version),
            { Major: 12 } => CmsCategoryK12.GetPrimaryKeyName(version),
            { Major: 13 } => CmsCategoryK13.GetPrimaryKeyName(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static bool ISourceModel<ICmsCategory>.IsAvailable(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsCategoryK11.IsAvailable(version),
            { Major: 12 } => CmsCategoryK12.IsAvailable(version),
            { Major: 13 } => CmsCategoryK13.IsAvailable(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static string ISourceModel<ICmsCategory>.TableName => "CMS_Category";
    static string ISourceModel<ICmsCategory>.GuidColumnName => "CategoryGUID"; //assumtion, class Guid column doesn't change between versions
    static ICmsCategory ISourceModel<ICmsCategory>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsCategoryK11.FromReader(reader, version),
            { Major: 12 } => CmsCategoryK12.FromReader(reader, version),
            { Major: 13 } => CmsCategoryK13.FromReader(reader, version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
}
public partial record CmsCategoryK11(int CategoryID, string CategoryDisplayName, string? CategoryName, string? CategoryDescription, int? CategoryCount, bool CategoryEnabled, int? CategoryUserID, Guid CategoryGUID, DateTime CategoryLastModified, int? CategorySiteID, int? CategoryParentID, string? CategoryIDPath, string? CategoryNamePath, int? CategoryLevel, int? CategoryOrder) : ICmsCategory, ISourceModel<CmsCategoryK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "CategoryID";
    public static string TableName => "CMS_Category";
    public static string GuidColumnName => "CategoryGUID";
    static CmsCategoryK11 ISourceModel<CmsCategoryK11>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsCategoryK11(
            reader.Unbox<int>("CategoryID"), reader.Unbox<string>("CategoryDisplayName"), reader.Unbox<string?>("CategoryName"), reader.Unbox<string?>("CategoryDescription"), reader.Unbox<int?>("CategoryCount"), reader.Unbox<bool>("CategoryEnabled"), reader.Unbox<int?>("CategoryUserID"), reader.Unbox<Guid>("CategoryGUID"), reader.Unbox<DateTime>("CategoryLastModified"), reader.Unbox<int?>("CategorySiteID"), reader.Unbox<int?>("CategoryParentID"), reader.Unbox<string?>("CategoryIDPath"), reader.Unbox<string?>("CategoryNamePath"), reader.Unbox<int?>("CategoryLevel"), reader.Unbox<int?>("CategoryOrder")
        );
    }
    public static CmsCategoryK11 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsCategoryK11(
            reader.Unbox<int>("CategoryID"), reader.Unbox<string>("CategoryDisplayName"), reader.Unbox<string?>("CategoryName"), reader.Unbox<string?>("CategoryDescription"), reader.Unbox<int?>("CategoryCount"), reader.Unbox<bool>("CategoryEnabled"), reader.Unbox<int?>("CategoryUserID"), reader.Unbox<Guid>("CategoryGUID"), reader.Unbox<DateTime>("CategoryLastModified"), reader.Unbox<int?>("CategorySiteID"), reader.Unbox<int?>("CategoryParentID"), reader.Unbox<string?>("CategoryIDPath"), reader.Unbox<string?>("CategoryNamePath"), reader.Unbox<int?>("CategoryLevel"), reader.Unbox<int?>("CategoryOrder")
        );
    }
};
public partial record CmsCategoryK12(int CategoryID, string CategoryDisplayName, string? CategoryName, string? CategoryDescription, int? CategoryCount, bool CategoryEnabled, int? CategoryUserID, Guid CategoryGUID, DateTime CategoryLastModified, int? CategorySiteID, int? CategoryParentID, string? CategoryIDPath, string? CategoryNamePath, int? CategoryLevel, int? CategoryOrder) : ICmsCategory, ISourceModel<CmsCategoryK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "CategoryID";
    public static string TableName => "CMS_Category";
    public static string GuidColumnName => "CategoryGUID";
    static CmsCategoryK12 ISourceModel<CmsCategoryK12>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsCategoryK12(
            reader.Unbox<int>("CategoryID"), reader.Unbox<string>("CategoryDisplayName"), reader.Unbox<string?>("CategoryName"), reader.Unbox<string?>("CategoryDescription"), reader.Unbox<int?>("CategoryCount"), reader.Unbox<bool>("CategoryEnabled"), reader.Unbox<int?>("CategoryUserID"), reader.Unbox<Guid>("CategoryGUID"), reader.Unbox<DateTime>("CategoryLastModified"), reader.Unbox<int?>("CategorySiteID"), reader.Unbox<int?>("CategoryParentID"), reader.Unbox<string?>("CategoryIDPath"), reader.Unbox<string?>("CategoryNamePath"), reader.Unbox<int?>("CategoryLevel"), reader.Unbox<int?>("CategoryOrder")
        );
    }
    public static CmsCategoryK12 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsCategoryK12(
            reader.Unbox<int>("CategoryID"), reader.Unbox<string>("CategoryDisplayName"), reader.Unbox<string?>("CategoryName"), reader.Unbox<string?>("CategoryDescription"), reader.Unbox<int?>("CategoryCount"), reader.Unbox<bool>("CategoryEnabled"), reader.Unbox<int?>("CategoryUserID"), reader.Unbox<Guid>("CategoryGUID"), reader.Unbox<DateTime>("CategoryLastModified"), reader.Unbox<int?>("CategorySiteID"), reader.Unbox<int?>("CategoryParentID"), reader.Unbox<string?>("CategoryIDPath"), reader.Unbox<string?>("CategoryNamePath"), reader.Unbox<int?>("CategoryLevel"), reader.Unbox<int?>("CategoryOrder")
        );
    }
};
public partial record CmsCategoryK13(int CategoryID, string CategoryDisplayName, string? CategoryName, string? CategoryDescription, int? CategoryCount, bool CategoryEnabled, int? CategoryUserID, Guid CategoryGUID, DateTime CategoryLastModified, int? CategorySiteID, int? CategoryParentID, string? CategoryIDPath, string? CategoryNamePath, int? CategoryLevel, int? CategoryOrder) : ICmsCategory, ISourceModel<CmsCategoryK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "CategoryID";
    public static string TableName => "CMS_Category";
    public static string GuidColumnName => "CategoryGUID";
    static CmsCategoryK13 ISourceModel<CmsCategoryK13>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsCategoryK13(
            reader.Unbox<int>("CategoryID"), reader.Unbox<string>("CategoryDisplayName"), reader.Unbox<string?>("CategoryName"), reader.Unbox<string?>("CategoryDescription"), reader.Unbox<int?>("CategoryCount"), reader.Unbox<bool>("CategoryEnabled"), reader.Unbox<int?>("CategoryUserID"), reader.Unbox<Guid>("CategoryGUID"), reader.Unbox<DateTime>("CategoryLastModified"), reader.Unbox<int?>("CategorySiteID"), reader.Unbox<int?>("CategoryParentID"), reader.Unbox<string?>("CategoryIDPath"), reader.Unbox<string?>("CategoryNamePath"), reader.Unbox<int?>("CategoryLevel"), reader.Unbox<int?>("CategoryOrder")
        );
    }
    public static CmsCategoryK13 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsCategoryK13(
            reader.Unbox<int>("CategoryID"), reader.Unbox<string>("CategoryDisplayName"), reader.Unbox<string?>("CategoryName"), reader.Unbox<string?>("CategoryDescription"), reader.Unbox<int?>("CategoryCount"), reader.Unbox<bool>("CategoryEnabled"), reader.Unbox<int?>("CategoryUserID"), reader.Unbox<Guid>("CategoryGUID"), reader.Unbox<DateTime>("CategoryLastModified"), reader.Unbox<int?>("CategorySiteID"), reader.Unbox<int?>("CategoryParentID"), reader.Unbox<string?>("CategoryIDPath"), reader.Unbox<string?>("CategoryNamePath"), reader.Unbox<int?>("CategoryLevel"), reader.Unbox<int?>("CategoryOrder")
        );
    }
};