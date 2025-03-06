// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Tool.Common;

namespace Migration.Tool.Source.Model;
public partial interface ICmsAllowedChildClasses : ISourceModel<ICmsAllowedChildClasses>
{
    int ParentClassID { get; }
    int ChildClassID { get; }

    static string ISourceModel<ICmsAllowedChildClasses>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsAllowedChildClasses.GetPrimaryKeyName(version),
        { Major: 12 } => CmsAllowedChildClasses.GetPrimaryKeyName(version),
        { Major: 13 } => CmsAllowedChildClasses.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsAllowedChildClasses>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsAllowedChildClasses.IsAvailable(version),
        { Major: 12 } => CmsAllowedChildClasses.IsAvailable(version),
        { Major: 13 } => CmsAllowedChildClasses.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsAllowedChildClasses>.TableName => "CMS_AllowedChildClasses";
    static string ISourceModel<ICmsAllowedChildClasses>.GuidColumnName => ""; //assumtion, class Guid column doesn't change between versions
    static ICmsAllowedChildClasses ISourceModel<ICmsAllowedChildClasses>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsAllowedChildClasses.FromReader(reader, version),
        { Major: 12 } => CmsAllowedChildClasses.FromReader(reader, version),
        { Major: 13 } => CmsAllowedChildClasses.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsAllowedChildClasses(int ParentClassID, int ChildClassID) : ICmsAllowedChildClasses, ISourceModel<CmsAllowedChildClasses>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "";
    public static string TableName => "CMS_AllowedChildClasses";
    public static string GuidColumnName => "";
    static CmsAllowedChildClasses ISourceModel<CmsAllowedChildClasses>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ParentClassID"), reader.Unbox<int>("ChildClassID")
        );
    public static CmsAllowedChildClasses FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ParentClassID"), reader.Unbox<int>("ChildClassID")
        );
};
