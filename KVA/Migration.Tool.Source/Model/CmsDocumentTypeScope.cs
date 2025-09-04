// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Tool.Common;

namespace Migration.Tool.Source.Model;
public partial interface ICmsDocumentTypeScope : ISourceModel<ICmsDocumentTypeScope>
{
    int ScopeID { get; }
    string ScopePath { get; }
    int? ScopeSiteID { get; }
    DateTime ScopeLastModified { get; }
    Guid? ScopeGUID { get; }
    bool? ScopeIncludeChildren { get; }
    bool? ScopeAllowAllTypes { get; }
    bool? ScopeAllowLinks { get; }
    bool? ScopeAllowABVariant { get; }
    string? ScopeMacroCondition { get; }

    static string ISourceModel<ICmsDocumentTypeScope>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsDocumentTypeScope.GetPrimaryKeyName(version),
        { Major: 12 } => CmsDocumentTypeScope.GetPrimaryKeyName(version),
        { Major: 13 } => CmsDocumentTypeScope.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsDocumentTypeScope>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsDocumentTypeScope.IsAvailable(version),
        { Major: 12 } => CmsDocumentTypeScope.IsAvailable(version),
        { Major: 13 } => CmsDocumentTypeScope.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsDocumentTypeScope>.TableName => "CMS_DocumentTypeScope";
    static string ISourceModel<ICmsDocumentTypeScope>.GuidColumnName => ""; //assumtion, class Guid column doesn't change between versions
    static ICmsDocumentTypeScope ISourceModel<ICmsDocumentTypeScope>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsDocumentTypeScope.FromReader(reader, version),
        { Major: 12 } => CmsDocumentTypeScope.FromReader(reader, version),
        { Major: 13 } => CmsDocumentTypeScope.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsDocumentTypeScope(int ScopeID, string ScopePath, int? ScopeSiteID, DateTime ScopeLastModified, Guid? ScopeGUID, bool? ScopeIncludeChildren,
    bool? ScopeAllowAllTypes, bool? ScopeAllowLinks, bool? ScopeAllowABVariant, string? ScopeMacroCondition) : ICmsDocumentTypeScope, ISourceModel<CmsDocumentTypeScope>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ScopeID";
    public static string TableName => "CMS_DocumentTypeScope";
    public static string GuidColumnName => "ScopeGUID";
    static CmsDocumentTypeScope ISourceModel<CmsDocumentTypeScope>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ScopeID"), reader.Unbox<string>("ScopePath"), reader.Unbox<int?>("ScopeSiteID"), reader.Unbox<DateTime>("ScopeLastModified"),
            reader.Unbox<Guid?>("ScopeGUID"), reader.Unbox<bool?>("ScopeIncludeChildren"), reader.Unbox<bool?>("ScopeAllowAllTypes"), reader.Unbox<bool?>("ScopeAllowLinks"),
            reader.Unbox<bool?>("ScopeAllowABVariant"), reader.Unbox<string?>("ScopeMacroCondition")
        );
    public static CmsDocumentTypeScope FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ScopeID"), reader.Unbox<string>("ScopePath"), reader.Unbox<int?>("ScopeSiteID"), reader.Unbox<DateTime>("ScopeLastModified"),
            reader.Unbox<Guid?>("ScopeGUID"), reader.Unbox<bool?>("ScopeIncludeChildren"), reader.Unbox<bool?>("ScopeAllowAllTypes"), reader.Unbox<bool?>("ScopeAllowLinks"),
            reader.Unbox<bool?>("ScopeAllowABVariant"), reader.Unbox<string?>("ScopeMacroCondition")
        );
};
