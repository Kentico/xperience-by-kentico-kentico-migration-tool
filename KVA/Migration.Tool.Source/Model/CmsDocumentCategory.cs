// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Tool.Common;

namespace Migration.Tool.Source.Model;
public partial interface ICmsDocumentCategory : ISourceModel<ICmsDocumentCategory>
{
    int DocumentID { get; }
    int CategoryID { get; }

    static string ISourceModel<ICmsDocumentCategory>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsDocumentCategoryK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsDocumentCategoryK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsDocumentCategoryK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsDocumentCategory>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsDocumentCategoryK11.IsAvailable(version),
        { Major: 12 } => CmsDocumentCategoryK12.IsAvailable(version),
        { Major: 13 } => CmsDocumentCategoryK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsDocumentCategory>.TableName => "CMS_DocumentCategory";
    static string ISourceModel<ICmsDocumentCategory>.GuidColumnName => ""; //assumtion, class Guid column doesn't change between versions
    static ICmsDocumentCategory ISourceModel<ICmsDocumentCategory>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsDocumentCategoryK11.FromReader(reader, version),
        { Major: 12 } => CmsDocumentCategoryK12.FromReader(reader, version),
        { Major: 13 } => CmsDocumentCategoryK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsDocumentCategoryK11(int DocumentID, int CategoryID) : ICmsDocumentCategory, ISourceModel<CmsDocumentCategoryK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "CategoryID";
    public static string TableName => "CMS_DocumentCategory";
    public static string GuidColumnName => "";
    static CmsDocumentCategoryK11 ISourceModel<CmsDocumentCategoryK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("DocumentID"), reader.Unbox<int>("CategoryID")
        );
    public static CmsDocumentCategoryK11 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("DocumentID"), reader.Unbox<int>("CategoryID")
        );
};
public partial record CmsDocumentCategoryK12(int DocumentID, int CategoryID) : ICmsDocumentCategory, ISourceModel<CmsDocumentCategoryK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "CategoryID";
    public static string TableName => "CMS_DocumentCategory";
    public static string GuidColumnName => "";
    static CmsDocumentCategoryK12 ISourceModel<CmsDocumentCategoryK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("DocumentID"), reader.Unbox<int>("CategoryID")
        );
    public static CmsDocumentCategoryK12 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("DocumentID"), reader.Unbox<int>("CategoryID")
        );
};
public partial record CmsDocumentCategoryK13(int DocumentID, int CategoryID) : ICmsDocumentCategory, ISourceModel<CmsDocumentCategoryK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "CategoryID";
    public static string TableName => "CMS_DocumentCategory";
    public static string GuidColumnName => "";
    static CmsDocumentCategoryK13 ISourceModel<CmsDocumentCategoryK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("DocumentID"), reader.Unbox<int>("CategoryID")
        );
    public static CmsDocumentCategoryK13 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("DocumentID"), reader.Unbox<int>("CategoryID")
        );
};

