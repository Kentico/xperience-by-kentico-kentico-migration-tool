// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Tool.Common;

namespace Migration.Tool.Source.Model;
public partial interface ICmsDocumentTypeScopeClass : ISourceModel<ICmsDocumentTypeScopeClass>
{
    int ScopeID { get; }
    int ClassID { get; }

    static string ISourceModel<ICmsDocumentTypeScopeClass>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsDocumentTypeScope.GetPrimaryKeyName(version),
        { Major: 12 } => CmsDocumentTypeScope.GetPrimaryKeyName(version),
        { Major: 13 } => CmsDocumentTypeScope.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsDocumentTypeScopeClass>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsDocumentTypeScopeClass.IsAvailable(version),
        { Major: 12 } => CmsDocumentTypeScopeClass.IsAvailable(version),
        { Major: 13 } => CmsDocumentTypeScopeClass.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsDocumentTypeScopeClass>.TableName => "CMS_DocumentTypeScopeClass";
    static string ISourceModel<ICmsDocumentTypeScopeClass>.GuidColumnName => ""; //assumtion, class Guid column doesn't change between versions
    static ICmsDocumentTypeScopeClass ISourceModel<ICmsDocumentTypeScopeClass>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsDocumentTypeScopeClass.FromReader(reader, version),
        { Major: 12 } => CmsDocumentTypeScopeClass.FromReader(reader, version),
        { Major: 13 } => CmsDocumentTypeScopeClass.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsDocumentTypeScopeClass(int ScopeID, int ClassID) : ICmsDocumentTypeScopeClass, ISourceModel<CmsDocumentTypeScopeClass>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "";
    public static string TableName => "CMS_DocumentTypeScopeClass";
    public static string GuidColumnName => "";
    static CmsDocumentTypeScopeClass ISourceModel<CmsDocumentTypeScopeClass>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ScopeID"), reader.Unbox<int>("ClassID")
        );
    public static CmsDocumentTypeScopeClass FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ScopeID"), reader.Unbox<int>("ClassID")
        );
};
