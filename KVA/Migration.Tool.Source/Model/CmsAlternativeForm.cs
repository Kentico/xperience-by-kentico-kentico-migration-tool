// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Tool.Common;

namespace Migration.Tool.Source.Model;
public partial interface ICmsAlternativeForm : ISourceModel<ICmsAlternativeForm>
{
    int FormID { get; }
    string FormDisplayName { get; }
    string FormName { get; }
    int FormClassID { get; }
    string? FormDefinition { get; }
    string? FormLayout { get; }
    Guid FormGUID { get; }
    DateTime FormLastModified { get; }
    int? FormCoupledClassID { get; }
    bool? FormHideNewParentFields { get; }
    string? FormLayoutType { get; }
    string? FormVersionGUID { get; }
    string? FormCustomizedColumns { get; }
    bool? FormIsCustom { get; }

    static string ISourceModel<ICmsAlternativeForm>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsAlternativeFormK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsAlternativeFormK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsAlternativeFormK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsAlternativeForm>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsAlternativeFormK11.IsAvailable(version),
        { Major: 12 } => CmsAlternativeFormK12.IsAvailable(version),
        { Major: 13 } => CmsAlternativeFormK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsAlternativeForm>.TableName => "CMS_AlternativeForm";
    static string ISourceModel<ICmsAlternativeForm>.GuidColumnName => "FormGUID"; //assumtion, class Guid column doesn't change between versions
    static ICmsAlternativeForm ISourceModel<ICmsAlternativeForm>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsAlternativeFormK11.FromReader(reader, version),
        { Major: 12 } => CmsAlternativeFormK12.FromReader(reader, version),
        { Major: 13 } => CmsAlternativeFormK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsAlternativeFormK11(int FormID, string FormDisplayName, string FormName, int FormClassID, string? FormDefinition, string? FormLayout, Guid FormGUID, DateTime FormLastModified, int? FormCoupledClassID, bool? FormHideNewParentFields, string? FormLayoutType, string? FormVersionGUID, string? FormCustomizedColumns, bool? FormIsCustom) : ICmsAlternativeForm, ISourceModel<CmsAlternativeFormK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "FormID";
    public static string TableName => "CMS_AlternativeForm";
    public static string GuidColumnName => "FormGUID";
    static CmsAlternativeFormK11 ISourceModel<CmsAlternativeFormK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("FormID"), reader.Unbox<string>("FormDisplayName"), reader.Unbox<string>("FormName"), reader.Unbox<int>("FormClassID"), reader.Unbox<string?>("FormDefinition"), reader.Unbox<string?>("FormLayout"), reader.Unbox<Guid>("FormGUID"), reader.Unbox<DateTime>("FormLastModified"), reader.Unbox<int?>("FormCoupledClassID"), reader.Unbox<bool?>("FormHideNewParentFields"), reader.Unbox<string?>("FormLayoutType"), reader.Unbox<string?>("FormVersionGUID"), reader.Unbox<string?>("FormCustomizedColumns"), reader.Unbox<bool?>("FormIsCustom")
        );
    public static CmsAlternativeFormK11 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("FormID"), reader.Unbox<string>("FormDisplayName"), reader.Unbox<string>("FormName"), reader.Unbox<int>("FormClassID"), reader.Unbox<string?>("FormDefinition"), reader.Unbox<string?>("FormLayout"), reader.Unbox<Guid>("FormGUID"), reader.Unbox<DateTime>("FormLastModified"), reader.Unbox<int?>("FormCoupledClassID"), reader.Unbox<bool?>("FormHideNewParentFields"), reader.Unbox<string?>("FormLayoutType"), reader.Unbox<string?>("FormVersionGUID"), reader.Unbox<string?>("FormCustomizedColumns"), reader.Unbox<bool?>("FormIsCustom")
        );
};
public partial record CmsAlternativeFormK12(int FormID, string FormDisplayName, string FormName, int FormClassID, string? FormDefinition, string? FormLayout, Guid FormGUID, DateTime FormLastModified, int? FormCoupledClassID, bool? FormHideNewParentFields, string? FormLayoutType, string? FormVersionGUID, string? FormCustomizedColumns, bool? FormIsCustom) : ICmsAlternativeForm, ISourceModel<CmsAlternativeFormK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "FormID";
    public static string TableName => "CMS_AlternativeForm";
    public static string GuidColumnName => "FormGUID";
    static CmsAlternativeFormK12 ISourceModel<CmsAlternativeFormK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("FormID"), reader.Unbox<string>("FormDisplayName"), reader.Unbox<string>("FormName"), reader.Unbox<int>("FormClassID"), reader.Unbox<string?>("FormDefinition"), reader.Unbox<string?>("FormLayout"), reader.Unbox<Guid>("FormGUID"), reader.Unbox<DateTime>("FormLastModified"), reader.Unbox<int?>("FormCoupledClassID"), reader.Unbox<bool?>("FormHideNewParentFields"), reader.Unbox<string?>("FormLayoutType"), reader.Unbox<string?>("FormVersionGUID"), reader.Unbox<string?>("FormCustomizedColumns"), reader.Unbox<bool?>("FormIsCustom")
        );
    public static CmsAlternativeFormK12 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("FormID"), reader.Unbox<string>("FormDisplayName"), reader.Unbox<string>("FormName"), reader.Unbox<int>("FormClassID"), reader.Unbox<string?>("FormDefinition"), reader.Unbox<string?>("FormLayout"), reader.Unbox<Guid>("FormGUID"), reader.Unbox<DateTime>("FormLastModified"), reader.Unbox<int?>("FormCoupledClassID"), reader.Unbox<bool?>("FormHideNewParentFields"), reader.Unbox<string?>("FormLayoutType"), reader.Unbox<string?>("FormVersionGUID"), reader.Unbox<string?>("FormCustomizedColumns"), reader.Unbox<bool?>("FormIsCustom")
        );
};
public partial record CmsAlternativeFormK13(int FormID, string FormDisplayName, string FormName, int FormClassID, string? FormDefinition, string? FormLayout, Guid FormGUID, DateTime FormLastModified, int? FormCoupledClassID, bool? FormHideNewParentFields, string? FormLayoutType, string? FormVersionGUID, string? FormCustomizedColumns, bool? FormIsCustom) : ICmsAlternativeForm, ISourceModel<CmsAlternativeFormK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "FormID";
    public static string TableName => "CMS_AlternativeForm";
    public static string GuidColumnName => "FormGUID";
    static CmsAlternativeFormK13 ISourceModel<CmsAlternativeFormK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("FormID"), reader.Unbox<string>("FormDisplayName"), reader.Unbox<string>("FormName"), reader.Unbox<int>("FormClassID"), reader.Unbox<string?>("FormDefinition"), reader.Unbox<string?>("FormLayout"), reader.Unbox<Guid>("FormGUID"), reader.Unbox<DateTime>("FormLastModified"), reader.Unbox<int?>("FormCoupledClassID"), reader.Unbox<bool?>("FormHideNewParentFields"), reader.Unbox<string?>("FormLayoutType"), reader.Unbox<string?>("FormVersionGUID"), reader.Unbox<string?>("FormCustomizedColumns"), reader.Unbox<bool?>("FormIsCustom")
        );
    public static CmsAlternativeFormK13 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("FormID"), reader.Unbox<string>("FormDisplayName"), reader.Unbox<string>("FormName"), reader.Unbox<int>("FormClassID"), reader.Unbox<string?>("FormDefinition"), reader.Unbox<string?>("FormLayout"), reader.Unbox<Guid>("FormGUID"), reader.Unbox<DateTime>("FormLastModified"), reader.Unbox<int?>("FormCoupledClassID"), reader.Unbox<bool?>("FormHideNewParentFields"), reader.Unbox<string?>("FormLayoutType"), reader.Unbox<string?>("FormVersionGUID"), reader.Unbox<string?>("FormCustomizedColumns"), reader.Unbox<bool?>("FormIsCustom")
        );
};

