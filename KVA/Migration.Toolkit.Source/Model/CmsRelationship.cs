// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;
public partial interface ICmsRelationship : ISourceModel<ICmsRelationship>
{
    int RelationshipID { get; }
    int LeftNodeID { get; }
    int RightNodeID { get; }
    int RelationshipNameID { get; }
    string? RelationshipCustomData { get; }
    int? RelationshipOrder { get; }
    bool? RelationshipIsAdHoc { get; }

    static string ISourceModel<ICmsRelationship>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsRelationshipK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsRelationshipK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsRelationshipK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsRelationship>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsRelationshipK11.IsAvailable(version),
        { Major: 12 } => CmsRelationshipK12.IsAvailable(version),
        { Major: 13 } => CmsRelationshipK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsRelationship>.TableName => "CMS_Relationship";
    static string ISourceModel<ICmsRelationship>.GuidColumnName => ""; //assumtion, class Guid column doesn't change between versions
    static ICmsRelationship ISourceModel<ICmsRelationship>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsRelationshipK11.FromReader(reader, version),
        { Major: 12 } => CmsRelationshipK12.FromReader(reader, version),
        { Major: 13 } => CmsRelationshipK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsRelationshipK11(int RelationshipID, int LeftNodeID, int RightNodeID, int RelationshipNameID, string? RelationshipCustomData, int? RelationshipOrder, bool? RelationshipIsAdHoc) : ICmsRelationship, ISourceModel<CmsRelationshipK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "RelationshipID";
    public static string TableName => "CMS_Relationship";
    public static string GuidColumnName => "";
    static CmsRelationshipK11 ISourceModel<CmsRelationshipK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("RelationshipID"), reader.Unbox<int>("LeftNodeID"), reader.Unbox<int>("RightNodeID"), reader.Unbox<int>("RelationshipNameID"), reader.Unbox<string?>("RelationshipCustomData"), reader.Unbox<int?>("RelationshipOrder"), reader.Unbox<bool?>("RelationshipIsAdHoc")
        );
    public static CmsRelationshipK11 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("RelationshipID"), reader.Unbox<int>("LeftNodeID"), reader.Unbox<int>("RightNodeID"), reader.Unbox<int>("RelationshipNameID"), reader.Unbox<string?>("RelationshipCustomData"), reader.Unbox<int?>("RelationshipOrder"), reader.Unbox<bool?>("RelationshipIsAdHoc")
        );
};
public partial record CmsRelationshipK12(int RelationshipID, int LeftNodeID, int RightNodeID, int RelationshipNameID, string? RelationshipCustomData, int? RelationshipOrder, bool? RelationshipIsAdHoc) : ICmsRelationship, ISourceModel<CmsRelationshipK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "RelationshipID";
    public static string TableName => "CMS_Relationship";
    public static string GuidColumnName => "";
    static CmsRelationshipK12 ISourceModel<CmsRelationshipK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("RelationshipID"), reader.Unbox<int>("LeftNodeID"), reader.Unbox<int>("RightNodeID"), reader.Unbox<int>("RelationshipNameID"), reader.Unbox<string?>("RelationshipCustomData"), reader.Unbox<int?>("RelationshipOrder"), reader.Unbox<bool?>("RelationshipIsAdHoc")
        );
    public static CmsRelationshipK12 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("RelationshipID"), reader.Unbox<int>("LeftNodeID"), reader.Unbox<int>("RightNodeID"), reader.Unbox<int>("RelationshipNameID"), reader.Unbox<string?>("RelationshipCustomData"), reader.Unbox<int?>("RelationshipOrder"), reader.Unbox<bool?>("RelationshipIsAdHoc")
        );
};
public partial record CmsRelationshipK13(int RelationshipID, int LeftNodeID, int RightNodeID, int RelationshipNameID, string? RelationshipCustomData, int? RelationshipOrder, bool? RelationshipIsAdHoc) : ICmsRelationship, ISourceModel<CmsRelationshipK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "RelationshipID";
    public static string TableName => "CMS_Relationship";
    public static string GuidColumnName => "";
    static CmsRelationshipK13 ISourceModel<CmsRelationshipK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("RelationshipID"), reader.Unbox<int>("LeftNodeID"), reader.Unbox<int>("RightNodeID"), reader.Unbox<int>("RelationshipNameID"), reader.Unbox<string?>("RelationshipCustomData"), reader.Unbox<int?>("RelationshipOrder"), reader.Unbox<bool?>("RelationshipIsAdHoc")
        );
    public static CmsRelationshipK13 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("RelationshipID"), reader.Unbox<int>("LeftNodeID"), reader.Unbox<int>("RightNodeID"), reader.Unbox<int>("RelationshipNameID"), reader.Unbox<string?>("RelationshipCustomData"), reader.Unbox<int?>("RelationshipOrder"), reader.Unbox<bool?>("RelationshipIsAdHoc")
        );
};

