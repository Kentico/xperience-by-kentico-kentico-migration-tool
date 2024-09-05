// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;
public partial interface ICmsVersionHistory : ISourceModel<ICmsVersionHistory>
{
    int VersionHistoryID { get; }
    int NodeSiteID { get; }
    int? DocumentID { get; }
    string NodeXML { get; }
    int? ModifiedByUserID { get; }
    DateTime ModifiedWhen { get; }
    string? VersionNumber { get; }
    string? VersionComment { get; }
    bool ToBePublished { get; }
    DateTime? PublishFrom { get; }
    DateTime? PublishTo { get; }
    DateTime? WasPublishedFrom { get; }
    DateTime? WasPublishedTo { get; }
    string? VersionDocumentName { get; }
    int? VersionClassID { get; }
    int? VersionWorkflowID { get; }
    int? VersionWorkflowStepID { get; }
    string? VersionNodeAliasPath { get; }
    int? VersionDeletedByUserID { get; }
    DateTime? VersionDeletedWhen { get; }

    static string ISourceModel<ICmsVersionHistory>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsVersionHistoryK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsVersionHistoryK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsVersionHistoryK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsVersionHistory>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsVersionHistoryK11.IsAvailable(version),
        { Major: 12 } => CmsVersionHistoryK12.IsAvailable(version),
        { Major: 13 } => CmsVersionHistoryK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsVersionHistory>.TableName => "CMS_VersionHistory";
    static string ISourceModel<ICmsVersionHistory>.GuidColumnName => ""; //assumtion, class Guid column doesn't change between versions
    static ICmsVersionHistory ISourceModel<ICmsVersionHistory>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsVersionHistoryK11.FromReader(reader, version),
        { Major: 12 } => CmsVersionHistoryK12.FromReader(reader, version),
        { Major: 13 } => CmsVersionHistoryK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsVersionHistoryK11(int VersionHistoryID, int NodeSiteID, int? DocumentID, string DocumentNamePath, string NodeXML, int? ModifiedByUserID, DateTime ModifiedWhen, string? VersionNumber, string? VersionComment, bool ToBePublished, DateTime? PublishFrom, DateTime? PublishTo, DateTime? WasPublishedFrom, DateTime? WasPublishedTo, string? VersionDocumentName, string? VersionDocumentType, int? VersionClassID, string? VersionMenuRedirectUrl, int? VersionWorkflowID, int? VersionWorkflowStepID, string? VersionNodeAliasPath, int? VersionDeletedByUserID, DateTime? VersionDeletedWhen) : ICmsVersionHistory, ISourceModel<CmsVersionHistoryK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "VersionHistoryID";
    public static string TableName => "CMS_VersionHistory";
    public static string GuidColumnName => "";
    static CmsVersionHistoryK11 ISourceModel<CmsVersionHistoryK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("VersionHistoryID"), reader.Unbox<int>("NodeSiteID"), reader.Unbox<int?>("DocumentID"), reader.Unbox<string>("DocumentNamePath"), reader.Unbox<string>("NodeXML"), reader.Unbox<int?>("ModifiedByUserID"), reader.Unbox<DateTime>("ModifiedWhen"), reader.Unbox<string?>("VersionNumber"), reader.Unbox<string?>("VersionComment"), reader.Unbox<bool>("ToBePublished"), reader.Unbox<DateTime?>("PublishFrom"), reader.Unbox<DateTime?>("PublishTo"), reader.Unbox<DateTime?>("WasPublishedFrom"), reader.Unbox<DateTime?>("WasPublishedTo"), reader.Unbox<string?>("VersionDocumentName"), reader.Unbox<string?>("VersionDocumentType"), reader.Unbox<int?>("VersionClassID"), reader.Unbox<string?>("VersionMenuRedirectUrl"), reader.Unbox<int?>("VersionWorkflowID"), reader.Unbox<int?>("VersionWorkflowStepID"), reader.Unbox<string?>("VersionNodeAliasPath"), reader.Unbox<int?>("VersionDeletedByUserID"), reader.Unbox<DateTime?>("VersionDeletedWhen")
        );
    public static CmsVersionHistoryK11 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("VersionHistoryID"), reader.Unbox<int>("NodeSiteID"), reader.Unbox<int?>("DocumentID"), reader.Unbox<string>("DocumentNamePath"), reader.Unbox<string>("NodeXML"), reader.Unbox<int?>("ModifiedByUserID"), reader.Unbox<DateTime>("ModifiedWhen"), reader.Unbox<string?>("VersionNumber"), reader.Unbox<string?>("VersionComment"), reader.Unbox<bool>("ToBePublished"), reader.Unbox<DateTime?>("PublishFrom"), reader.Unbox<DateTime?>("PublishTo"), reader.Unbox<DateTime?>("WasPublishedFrom"), reader.Unbox<DateTime?>("WasPublishedTo"), reader.Unbox<string?>("VersionDocumentName"), reader.Unbox<string?>("VersionDocumentType"), reader.Unbox<int?>("VersionClassID"), reader.Unbox<string?>("VersionMenuRedirectUrl"), reader.Unbox<int?>("VersionWorkflowID"), reader.Unbox<int?>("VersionWorkflowStepID"), reader.Unbox<string?>("VersionNodeAliasPath"), reader.Unbox<int?>("VersionDeletedByUserID"), reader.Unbox<DateTime?>("VersionDeletedWhen")
        );
};
public partial record CmsVersionHistoryK12(int VersionHistoryID, int NodeSiteID, int? DocumentID, string DocumentNamePath, string NodeXML, int? ModifiedByUserID, DateTime ModifiedWhen, string? VersionNumber, string? VersionComment, bool ToBePublished, DateTime? PublishFrom, DateTime? PublishTo, DateTime? WasPublishedFrom, DateTime? WasPublishedTo, string? VersionDocumentName, string? VersionDocumentType, int? VersionClassID, string? VersionMenuRedirectUrl, int? VersionWorkflowID, int? VersionWorkflowStepID, string? VersionNodeAliasPath, int? VersionDeletedByUserID, DateTime? VersionDeletedWhen) : ICmsVersionHistory, ISourceModel<CmsVersionHistoryK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "VersionHistoryID";
    public static string TableName => "CMS_VersionHistory";
    public static string GuidColumnName => "";
    static CmsVersionHistoryK12 ISourceModel<CmsVersionHistoryK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("VersionHistoryID"), reader.Unbox<int>("NodeSiteID"), reader.Unbox<int?>("DocumentID"), reader.Unbox<string>("DocumentNamePath"), reader.Unbox<string>("NodeXML"), reader.Unbox<int?>("ModifiedByUserID"), reader.Unbox<DateTime>("ModifiedWhen"), reader.Unbox<string?>("VersionNumber"), reader.Unbox<string?>("VersionComment"), reader.Unbox<bool>("ToBePublished"), reader.Unbox<DateTime?>("PublishFrom"), reader.Unbox<DateTime?>("PublishTo"), reader.Unbox<DateTime?>("WasPublishedFrom"), reader.Unbox<DateTime?>("WasPublishedTo"), reader.Unbox<string?>("VersionDocumentName"), reader.Unbox<string?>("VersionDocumentType"), reader.Unbox<int?>("VersionClassID"), reader.Unbox<string?>("VersionMenuRedirectUrl"), reader.Unbox<int?>("VersionWorkflowID"), reader.Unbox<int?>("VersionWorkflowStepID"), reader.Unbox<string?>("VersionNodeAliasPath"), reader.Unbox<int?>("VersionDeletedByUserID"), reader.Unbox<DateTime?>("VersionDeletedWhen")
        );
    public static CmsVersionHistoryK12 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("VersionHistoryID"), reader.Unbox<int>("NodeSiteID"), reader.Unbox<int?>("DocumentID"), reader.Unbox<string>("DocumentNamePath"), reader.Unbox<string>("NodeXML"), reader.Unbox<int?>("ModifiedByUserID"), reader.Unbox<DateTime>("ModifiedWhen"), reader.Unbox<string?>("VersionNumber"), reader.Unbox<string?>("VersionComment"), reader.Unbox<bool>("ToBePublished"), reader.Unbox<DateTime?>("PublishFrom"), reader.Unbox<DateTime?>("PublishTo"), reader.Unbox<DateTime?>("WasPublishedFrom"), reader.Unbox<DateTime?>("WasPublishedTo"), reader.Unbox<string?>("VersionDocumentName"), reader.Unbox<string?>("VersionDocumentType"), reader.Unbox<int?>("VersionClassID"), reader.Unbox<string?>("VersionMenuRedirectUrl"), reader.Unbox<int?>("VersionWorkflowID"), reader.Unbox<int?>("VersionWorkflowStepID"), reader.Unbox<string?>("VersionNodeAliasPath"), reader.Unbox<int?>("VersionDeletedByUserID"), reader.Unbox<DateTime?>("VersionDeletedWhen")
        );
};
public partial record CmsVersionHistoryK13(int VersionHistoryID, int NodeSiteID, int? DocumentID, string NodeXML, int? ModifiedByUserID, DateTime ModifiedWhen, string? VersionNumber, string? VersionComment, bool ToBePublished, DateTime? PublishFrom, DateTime? PublishTo, DateTime? WasPublishedFrom, DateTime? WasPublishedTo, string? VersionDocumentName, int? VersionClassID, int? VersionWorkflowID, int? VersionWorkflowStepID, string? VersionNodeAliasPath, int? VersionDeletedByUserID, DateTime? VersionDeletedWhen) : ICmsVersionHistory, ISourceModel<CmsVersionHistoryK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "VersionHistoryID";
    public static string TableName => "CMS_VersionHistory";
    public static string GuidColumnName => "";
    static CmsVersionHistoryK13 ISourceModel<CmsVersionHistoryK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("VersionHistoryID"), reader.Unbox<int>("NodeSiteID"), reader.Unbox<int?>("DocumentID"), reader.Unbox<string>("NodeXML"), reader.Unbox<int?>("ModifiedByUserID"), reader.Unbox<DateTime>("ModifiedWhen"), reader.Unbox<string?>("VersionNumber"), reader.Unbox<string?>("VersionComment"), reader.Unbox<bool>("ToBePublished"), reader.Unbox<DateTime?>("PublishFrom"), reader.Unbox<DateTime?>("PublishTo"), reader.Unbox<DateTime?>("WasPublishedFrom"), reader.Unbox<DateTime?>("WasPublishedTo"), reader.Unbox<string?>("VersionDocumentName"), reader.Unbox<int?>("VersionClassID"), reader.Unbox<int?>("VersionWorkflowID"), reader.Unbox<int?>("VersionWorkflowStepID"), reader.Unbox<string?>("VersionNodeAliasPath"), reader.Unbox<int?>("VersionDeletedByUserID"), reader.Unbox<DateTime?>("VersionDeletedWhen")
        );
    public static CmsVersionHistoryK13 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("VersionHistoryID"), reader.Unbox<int>("NodeSiteID"), reader.Unbox<int?>("DocumentID"), reader.Unbox<string>("NodeXML"), reader.Unbox<int?>("ModifiedByUserID"), reader.Unbox<DateTime>("ModifiedWhen"), reader.Unbox<string?>("VersionNumber"), reader.Unbox<string?>("VersionComment"), reader.Unbox<bool>("ToBePublished"), reader.Unbox<DateTime?>("PublishFrom"), reader.Unbox<DateTime?>("PublishTo"), reader.Unbox<DateTime?>("WasPublishedFrom"), reader.Unbox<DateTime?>("WasPublishedTo"), reader.Unbox<string?>("VersionDocumentName"), reader.Unbox<int?>("VersionClassID"), reader.Unbox<int?>("VersionWorkflowID"), reader.Unbox<int?>("VersionWorkflowStepID"), reader.Unbox<string?>("VersionNodeAliasPath"), reader.Unbox<int?>("VersionDeletedByUserID"), reader.Unbox<DateTime?>("VersionDeletedWhen")
        );
};

