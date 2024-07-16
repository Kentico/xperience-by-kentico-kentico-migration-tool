// ReSharper disable InconsistentNaming

using System.Data;

using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;

public interface ICmsDocument : ISourceModel<ICmsDocument>
{
    int DocumentID { get; }
    string DocumentName { get; }
    DateTime? DocumentModifiedWhen { get; }
    int? DocumentModifiedByUserID { get; }
    int? DocumentForeignKeyValue { get; }
    int? DocumentCreatedByUserID { get; }
    DateTime? DocumentCreatedWhen { get; }
    int? DocumentCheckedOutByUserID { get; }
    DateTime? DocumentCheckedOutWhen { get; }
    int? DocumentCheckedOutVersionHistoryID { get; }
    int? DocumentPublishedVersionHistoryID { get; }
    int? DocumentWorkflowStepID { get; }
    DateTime? DocumentPublishFrom { get; }
    DateTime? DocumentPublishTo { get; }
    string DocumentCulture { get; }
    int DocumentNodeID { get; }
    string? DocumentPageTitle { get; }
    string? DocumentPageKeyWords { get; }
    string? DocumentPageDescription { get; }
    string? DocumentContent { get; }
    string? DocumentCustomData { get; }
    string? DocumentTags { get; }
    int? DocumentTagGroupID { get; }
    DateTime? DocumentLastPublished { get; }
    bool? DocumentSearchExcluded { get; }
    string? DocumentLastVersionNumber { get; }
    bool? DocumentIsArchived { get; }
    Guid? DocumentGUID { get; }
    Guid? DocumentWorkflowCycleGUID { get; }
    bool? DocumentIsWaitingForTranslation { get; }
    string? DocumentSKUName { get; }
    string? DocumentSKUDescription { get; }
    string? DocumentSKUShortDescription { get; }
    string? DocumentWorkflowActionStatus { get; }
    bool DocumentCanBePublished { get; }

    static string ISourceModel<ICmsDocument>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsDocumentK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsDocumentK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsDocumentK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };

    static bool ISourceModel<ICmsDocument>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsDocumentK11.IsAvailable(version),
        { Major: 12 } => CmsDocumentK12.IsAvailable(version),
        { Major: 13 } => CmsDocumentK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };

    static string ISourceModel<ICmsDocument>.TableName => "CMS_Document";
    static string ISourceModel<ICmsDocument>.GuidColumnName => "DocumentGUID"; //assumtion, class Guid column doesn't change between versions

    static ICmsDocument ISourceModel<ICmsDocument>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsDocumentK11.FromReader(reader, version),
        { Major: 12 } => CmsDocumentK12.FromReader(reader, version),
        { Major: 13 } => CmsDocumentK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}

public record CmsDocumentK11(
    int DocumentID,
    string DocumentName,
    string? DocumentNamePath,
    DateTime? DocumentModifiedWhen,
    int? DocumentModifiedByUserID,
    int? DocumentForeignKeyValue,
    int? DocumentCreatedByUserID,
    DateTime? DocumentCreatedWhen,
    int? DocumentCheckedOutByUserID,
    DateTime? DocumentCheckedOutWhen,
    int? DocumentCheckedOutVersionHistoryID,
    int? DocumentPublishedVersionHistoryID,
    int? DocumentWorkflowStepID,
    DateTime? DocumentPublishFrom,
    DateTime? DocumentPublishTo,
    string? DocumentUrlPath,
    string DocumentCulture,
    int DocumentNodeID,
    string? DocumentPageTitle,
    string? DocumentPageKeyWords,
    string? DocumentPageDescription,
    bool DocumentShowInSiteMap,
    bool DocumentMenuItemHideInNavigation,
    string? DocumentMenuCaption,
    string? DocumentMenuStyle,
    string? DocumentMenuItemImage,
    string? DocumentMenuItemLeftImage,
    string? DocumentMenuItemRightImage,
    int? DocumentPageTemplateID,
    string? DocumentMenuJavascript,
    string? DocumentMenuRedirectUrl,
    bool? DocumentUseNamePathForUrlPath,
    int? DocumentStylesheetID,
    string? DocumentContent,
    string? DocumentMenuClass,
    string? DocumentMenuStyleHighlighted,
    string? DocumentMenuClassHighlighted,
    string? DocumentMenuItemImageHighlighted,
    string? DocumentMenuItemLeftImageHighlighted,
    string? DocumentMenuItemRightImageHighlighted,
    bool? DocumentMenuItemInactive,
    string? DocumentCustomData,
    string? DocumentExtensions,
    string? DocumentTags,
    int? DocumentTagGroupID,
    string? DocumentWildcardRule,
    string? DocumentWebParts,
    double? DocumentRatingValue,
    int? DocumentRatings,
    int? DocumentPriority,
    string? DocumentType,
    DateTime? DocumentLastPublished,
    bool? DocumentUseCustomExtensions,
    string? DocumentGroupWebParts,
    bool? DocumentCheckedOutAutomatically,
    string? DocumentTrackConversionName,
    string? DocumentConversionValue,
    bool? DocumentSearchExcluded,
    string? DocumentLastVersionNumber,
    bool? DocumentIsArchived,
    string? DocumentHash,
    bool? DocumentLogVisitActivity,
    Guid? DocumentGUID,
    Guid? DocumentWorkflowCycleGUID,
    string? DocumentSitemapSettings,
    bool? DocumentIsWaitingForTranslation,
    string? DocumentSKUName,
    string? DocumentSKUDescription,
    string? DocumentSKUShortDescription,
    string? DocumentWorkflowActionStatus,
    bool? DocumentMenuRedirectToFirstChild,
    bool DocumentCanBePublished,
    bool DocumentInheritsStylesheet) : ICmsDocument, ISourceModel<CmsDocumentK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "DocumentID";
    public static string TableName => "CMS_Document";
    public static string GuidColumnName => "DocumentGUID";

    static CmsDocumentK11 ISourceModel<CmsDocumentK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("DocumentID"), reader.Unbox<string>("DocumentName"), reader.Unbox<string?>("DocumentNamePath"), reader.Unbox<DateTime?>("DocumentModifiedWhen"), reader.Unbox<int?>("DocumentModifiedByUserID"),
        reader.Unbox<int?>("DocumentForeignKeyValue"), reader.Unbox<int?>("DocumentCreatedByUserID"), reader.Unbox<DateTime?>("DocumentCreatedWhen"), reader.Unbox<int?>("DocumentCheckedOutByUserID"), reader.Unbox<DateTime?>("DocumentCheckedOutWhen"),
        reader.Unbox<int?>("DocumentCheckedOutVersionHistoryID"), reader.Unbox<int?>("DocumentPublishedVersionHistoryID"), reader.Unbox<int?>("DocumentWorkflowStepID"), reader.Unbox<DateTime?>("DocumentPublishFrom"),
        reader.Unbox<DateTime?>("DocumentPublishTo"), reader.Unbox<string?>("DocumentUrlPath"), reader.Unbox<string>("DocumentCulture"), reader.Unbox<int>("DocumentNodeID"), reader.Unbox<string?>("DocumentPageTitle"),
        reader.Unbox<string?>("DocumentPageKeyWords"), reader.Unbox<string?>("DocumentPageDescription"), reader.Unbox<bool>("DocumentShowInSiteMap"), reader.Unbox<bool>("DocumentMenuItemHideInNavigation"),
        reader.Unbox<string?>("DocumentMenuCaption"), reader.Unbox<string?>("DocumentMenuStyle"), reader.Unbox<string?>("DocumentMenuItemImage"), reader.Unbox<string?>("DocumentMenuItemLeftImage"), reader.Unbox<string?>("DocumentMenuItemRightImage"),
        reader.Unbox<int?>("DocumentPageTemplateID"), reader.Unbox<string?>("DocumentMenuJavascript"), reader.Unbox<string?>("DocumentMenuRedirectUrl"), reader.Unbox<bool?>("DocumentUseNamePathForUrlPath"), reader.Unbox<int?>("DocumentStylesheetID"),
        reader.Unbox<string?>("DocumentContent"), reader.Unbox<string?>("DocumentMenuClass"), reader.Unbox<string?>("DocumentMenuStyleHighlighted"), reader.Unbox<string?>("DocumentMenuClassHighlighted"),
        reader.Unbox<string?>("DocumentMenuItemImageHighlighted"), reader.Unbox<string?>("DocumentMenuItemLeftImageHighlighted"), reader.Unbox<string?>("DocumentMenuItemRightImageHighlighted"), reader.Unbox<bool?>("DocumentMenuItemInactive"),
        reader.Unbox<string?>("DocumentCustomData"), reader.Unbox<string?>("DocumentExtensions"), reader.Unbox<string?>("DocumentTags"), reader.Unbox<int?>("DocumentTagGroupID"), reader.Unbox<string?>("DocumentWildcardRule"),
        reader.Unbox<string?>("DocumentWebParts"), reader.Unbox<double?>("DocumentRatingValue"), reader.Unbox<int?>("DocumentRatings"), reader.Unbox<int?>("DocumentPriority"), reader.Unbox<string?>("DocumentType"),
        reader.Unbox<DateTime?>("DocumentLastPublished"), reader.Unbox<bool?>("DocumentUseCustomExtensions"), reader.Unbox<string?>("DocumentGroupWebParts"), reader.Unbox<bool?>("DocumentCheckedOutAutomatically"),
        reader.Unbox<string?>("DocumentTrackConversionName"), reader.Unbox<string?>("DocumentConversionValue"), reader.Unbox<bool?>("DocumentSearchExcluded"), reader.Unbox<string?>("DocumentLastVersionNumber"),
        reader.Unbox<bool?>("DocumentIsArchived"), reader.Unbox<string?>("DocumentHash"), reader.Unbox<bool?>("DocumentLogVisitActivity"), reader.Unbox<Guid?>("DocumentGUID"), reader.Unbox<Guid?>("DocumentWorkflowCycleGUID"),
        reader.Unbox<string?>("DocumentSitemapSettings"), reader.Unbox<bool?>("DocumentIsWaitingForTranslation"), reader.Unbox<string?>("DocumentSKUName"), reader.Unbox<string?>("DocumentSKUDescription"),
        reader.Unbox<string?>("DocumentSKUShortDescription"), reader.Unbox<string?>("DocumentWorkflowActionStatus"), reader.Unbox<bool?>("DocumentMenuRedirectToFirstChild"), reader.Unbox<bool>("DocumentCanBePublished"),
        reader.Unbox<bool>("DocumentInheritsStylesheet")
    );

    public static CmsDocumentK11 FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("DocumentID"), reader.Unbox<string>("DocumentName"), reader.Unbox<string?>("DocumentNamePath"), reader.Unbox<DateTime?>("DocumentModifiedWhen"), reader.Unbox<int?>("DocumentModifiedByUserID"),
        reader.Unbox<int?>("DocumentForeignKeyValue"), reader.Unbox<int?>("DocumentCreatedByUserID"), reader.Unbox<DateTime?>("DocumentCreatedWhen"), reader.Unbox<int?>("DocumentCheckedOutByUserID"), reader.Unbox<DateTime?>("DocumentCheckedOutWhen"),
        reader.Unbox<int?>("DocumentCheckedOutVersionHistoryID"), reader.Unbox<int?>("DocumentPublishedVersionHistoryID"), reader.Unbox<int?>("DocumentWorkflowStepID"), reader.Unbox<DateTime?>("DocumentPublishFrom"),
        reader.Unbox<DateTime?>("DocumentPublishTo"), reader.Unbox<string?>("DocumentUrlPath"), reader.Unbox<string>("DocumentCulture"), reader.Unbox<int>("DocumentNodeID"), reader.Unbox<string?>("DocumentPageTitle"),
        reader.Unbox<string?>("DocumentPageKeyWords"), reader.Unbox<string?>("DocumentPageDescription"), reader.Unbox<bool>("DocumentShowInSiteMap"), reader.Unbox<bool>("DocumentMenuItemHideInNavigation"),
        reader.Unbox<string?>("DocumentMenuCaption"), reader.Unbox<string?>("DocumentMenuStyle"), reader.Unbox<string?>("DocumentMenuItemImage"), reader.Unbox<string?>("DocumentMenuItemLeftImage"), reader.Unbox<string?>("DocumentMenuItemRightImage"),
        reader.Unbox<int?>("DocumentPageTemplateID"), reader.Unbox<string?>("DocumentMenuJavascript"), reader.Unbox<string?>("DocumentMenuRedirectUrl"), reader.Unbox<bool?>("DocumentUseNamePathForUrlPath"), reader.Unbox<int?>("DocumentStylesheetID"),
        reader.Unbox<string?>("DocumentContent"), reader.Unbox<string?>("DocumentMenuClass"), reader.Unbox<string?>("DocumentMenuStyleHighlighted"), reader.Unbox<string?>("DocumentMenuClassHighlighted"),
        reader.Unbox<string?>("DocumentMenuItemImageHighlighted"), reader.Unbox<string?>("DocumentMenuItemLeftImageHighlighted"), reader.Unbox<string?>("DocumentMenuItemRightImageHighlighted"), reader.Unbox<bool?>("DocumentMenuItemInactive"),
        reader.Unbox<string?>("DocumentCustomData"), reader.Unbox<string?>("DocumentExtensions"), reader.Unbox<string?>("DocumentTags"), reader.Unbox<int?>("DocumentTagGroupID"), reader.Unbox<string?>("DocumentWildcardRule"),
        reader.Unbox<string?>("DocumentWebParts"), reader.Unbox<double?>("DocumentRatingValue"), reader.Unbox<int?>("DocumentRatings"), reader.Unbox<int?>("DocumentPriority"), reader.Unbox<string?>("DocumentType"),
        reader.Unbox<DateTime?>("DocumentLastPublished"), reader.Unbox<bool?>("DocumentUseCustomExtensions"), reader.Unbox<string?>("DocumentGroupWebParts"), reader.Unbox<bool?>("DocumentCheckedOutAutomatically"),
        reader.Unbox<string?>("DocumentTrackConversionName"), reader.Unbox<string?>("DocumentConversionValue"), reader.Unbox<bool?>("DocumentSearchExcluded"), reader.Unbox<string?>("DocumentLastVersionNumber"),
        reader.Unbox<bool?>("DocumentIsArchived"), reader.Unbox<string?>("DocumentHash"), reader.Unbox<bool?>("DocumentLogVisitActivity"), reader.Unbox<Guid?>("DocumentGUID"), reader.Unbox<Guid?>("DocumentWorkflowCycleGUID"),
        reader.Unbox<string?>("DocumentSitemapSettings"), reader.Unbox<bool?>("DocumentIsWaitingForTranslation"), reader.Unbox<string?>("DocumentSKUName"), reader.Unbox<string?>("DocumentSKUDescription"),
        reader.Unbox<string?>("DocumentSKUShortDescription"), reader.Unbox<string?>("DocumentWorkflowActionStatus"), reader.Unbox<bool?>("DocumentMenuRedirectToFirstChild"), reader.Unbox<bool>("DocumentCanBePublished"),
        reader.Unbox<bool>("DocumentInheritsStylesheet")
    );
}

public record CmsDocumentK12(
    int DocumentID,
    string DocumentName,
    string? DocumentNamePath,
    DateTime? DocumentModifiedWhen,
    int? DocumentModifiedByUserID,
    int? DocumentForeignKeyValue,
    int? DocumentCreatedByUserID,
    DateTime? DocumentCreatedWhen,
    int? DocumentCheckedOutByUserID,
    DateTime? DocumentCheckedOutWhen,
    int? DocumentCheckedOutVersionHistoryID,
    int? DocumentPublishedVersionHistoryID,
    int? DocumentWorkflowStepID,
    DateTime? DocumentPublishFrom,
    DateTime? DocumentPublishTo,
    string? DocumentUrlPath,
    string DocumentCulture,
    int DocumentNodeID,
    string? DocumentPageTitle,
    string? DocumentPageKeyWords,
    string? DocumentPageDescription,
    bool DocumentShowInSiteMap,
    bool DocumentMenuItemHideInNavigation,
    string? DocumentMenuCaption,
    string? DocumentMenuStyle,
    string? DocumentMenuItemImage,
    string? DocumentMenuItemLeftImage,
    string? DocumentMenuItemRightImage,
    int? DocumentPageTemplateID,
    string? DocumentMenuJavascript,
    string? DocumentMenuRedirectUrl,
    bool? DocumentUseNamePathForUrlPath,
    int? DocumentStylesheetID,
    string? DocumentContent,
    string? DocumentMenuClass,
    string? DocumentMenuStyleHighlighted,
    string? DocumentMenuClassHighlighted,
    string? DocumentMenuItemImageHighlighted,
    string? DocumentMenuItemLeftImageHighlighted,
    string? DocumentMenuItemRightImageHighlighted,
    bool? DocumentMenuItemInactive,
    string? DocumentCustomData,
    string? DocumentExtensions,
    string? DocumentTags,
    int? DocumentTagGroupID,
    string? DocumentWildcardRule,
    string? DocumentWebParts,
    double? DocumentRatingValue,
    int? DocumentRatings,
    int? DocumentPriority,
    string? DocumentType,
    DateTime? DocumentLastPublished,
    bool? DocumentUseCustomExtensions,
    string? DocumentGroupWebParts,
    bool? DocumentCheckedOutAutomatically,
    string? DocumentTrackConversionName,
    string? DocumentConversionValue,
    bool? DocumentSearchExcluded,
    string? DocumentLastVersionNumber,
    bool? DocumentIsArchived,
    string? DocumentHash,
    bool? DocumentLogVisitActivity,
    Guid? DocumentGUID,
    Guid? DocumentWorkflowCycleGUID,
    string? DocumentSitemapSettings,
    bool? DocumentIsWaitingForTranslation,
    string? DocumentSKUName,
    string? DocumentSKUDescription,
    string? DocumentSKUShortDescription,
    string? DocumentWorkflowActionStatus,
    bool? DocumentMenuRedirectToFirstChild,
    bool DocumentCanBePublished,
    bool DocumentInheritsStylesheet,
    string? DocumentPageBuilderWidgets,
    string? DocumentPageTemplateConfiguration,
    string? DocumentABTestConfiguration) : ICmsDocument, ISourceModel<CmsDocumentK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "DocumentID";
    public static string TableName => "CMS_Document";
    public static string GuidColumnName => "DocumentGUID";

    static CmsDocumentK12 ISourceModel<CmsDocumentK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("DocumentID"), reader.Unbox<string>("DocumentName"), reader.Unbox<string?>("DocumentNamePath"), reader.Unbox<DateTime?>("DocumentModifiedWhen"), reader.Unbox<int?>("DocumentModifiedByUserID"),
        reader.Unbox<int?>("DocumentForeignKeyValue"), reader.Unbox<int?>("DocumentCreatedByUserID"), reader.Unbox<DateTime?>("DocumentCreatedWhen"), reader.Unbox<int?>("DocumentCheckedOutByUserID"), reader.Unbox<DateTime?>("DocumentCheckedOutWhen"),
        reader.Unbox<int?>("DocumentCheckedOutVersionHistoryID"), reader.Unbox<int?>("DocumentPublishedVersionHistoryID"), reader.Unbox<int?>("DocumentWorkflowStepID"), reader.Unbox<DateTime?>("DocumentPublishFrom"),
        reader.Unbox<DateTime?>("DocumentPublishTo"), reader.Unbox<string?>("DocumentUrlPath"), reader.Unbox<string>("DocumentCulture"), reader.Unbox<int>("DocumentNodeID"), reader.Unbox<string?>("DocumentPageTitle"),
        reader.Unbox<string?>("DocumentPageKeyWords"), reader.Unbox<string?>("DocumentPageDescription"), reader.Unbox<bool>("DocumentShowInSiteMap"), reader.Unbox<bool>("DocumentMenuItemHideInNavigation"),
        reader.Unbox<string?>("DocumentMenuCaption"), reader.Unbox<string?>("DocumentMenuStyle"), reader.Unbox<string?>("DocumentMenuItemImage"), reader.Unbox<string?>("DocumentMenuItemLeftImage"), reader.Unbox<string?>("DocumentMenuItemRightImage"),
        reader.Unbox<int?>("DocumentPageTemplateID"), reader.Unbox<string?>("DocumentMenuJavascript"), reader.Unbox<string?>("DocumentMenuRedirectUrl"), reader.Unbox<bool?>("DocumentUseNamePathForUrlPath"), reader.Unbox<int?>("DocumentStylesheetID"),
        reader.Unbox<string?>("DocumentContent"), reader.Unbox<string?>("DocumentMenuClass"), reader.Unbox<string?>("DocumentMenuStyleHighlighted"), reader.Unbox<string?>("DocumentMenuClassHighlighted"),
        reader.Unbox<string?>("DocumentMenuItemImageHighlighted"), reader.Unbox<string?>("DocumentMenuItemLeftImageHighlighted"), reader.Unbox<string?>("DocumentMenuItemRightImageHighlighted"), reader.Unbox<bool?>("DocumentMenuItemInactive"),
        reader.Unbox<string?>("DocumentCustomData"), reader.Unbox<string?>("DocumentExtensions"), reader.Unbox<string?>("DocumentTags"), reader.Unbox<int?>("DocumentTagGroupID"), reader.Unbox<string?>("DocumentWildcardRule"),
        reader.Unbox<string?>("DocumentWebParts"), reader.Unbox<double?>("DocumentRatingValue"), reader.Unbox<int?>("DocumentRatings"), reader.Unbox<int?>("DocumentPriority"), reader.Unbox<string?>("DocumentType"),
        reader.Unbox<DateTime?>("DocumentLastPublished"), reader.Unbox<bool?>("DocumentUseCustomExtensions"), reader.Unbox<string?>("DocumentGroupWebParts"), reader.Unbox<bool?>("DocumentCheckedOutAutomatically"),
        reader.Unbox<string?>("DocumentTrackConversionName"), reader.Unbox<string?>("DocumentConversionValue"), reader.Unbox<bool?>("DocumentSearchExcluded"), reader.Unbox<string?>("DocumentLastVersionNumber"),
        reader.Unbox<bool?>("DocumentIsArchived"), reader.Unbox<string?>("DocumentHash"), reader.Unbox<bool?>("DocumentLogVisitActivity"), reader.Unbox<Guid?>("DocumentGUID"), reader.Unbox<Guid?>("DocumentWorkflowCycleGUID"),
        reader.Unbox<string?>("DocumentSitemapSettings"), reader.Unbox<bool?>("DocumentIsWaitingForTranslation"), reader.Unbox<string?>("DocumentSKUName"), reader.Unbox<string?>("DocumentSKUDescription"),
        reader.Unbox<string?>("DocumentSKUShortDescription"), reader.Unbox<string?>("DocumentWorkflowActionStatus"), reader.Unbox<bool?>("DocumentMenuRedirectToFirstChild"), reader.Unbox<bool>("DocumentCanBePublished"),
        reader.Unbox<bool>("DocumentInheritsStylesheet"), reader.Unbox<string?>("DocumentPageBuilderWidgets"), reader.Unbox<string?>("DocumentPageTemplateConfiguration"), reader.Unbox<string?>("DocumentABTestConfiguration")
    );

    public static CmsDocumentK12 FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("DocumentID"), reader.Unbox<string>("DocumentName"), reader.Unbox<string?>("DocumentNamePath"), reader.Unbox<DateTime?>("DocumentModifiedWhen"), reader.Unbox<int?>("DocumentModifiedByUserID"),
        reader.Unbox<int?>("DocumentForeignKeyValue"), reader.Unbox<int?>("DocumentCreatedByUserID"), reader.Unbox<DateTime?>("DocumentCreatedWhen"), reader.Unbox<int?>("DocumentCheckedOutByUserID"), reader.Unbox<DateTime?>("DocumentCheckedOutWhen"),
        reader.Unbox<int?>("DocumentCheckedOutVersionHistoryID"), reader.Unbox<int?>("DocumentPublishedVersionHistoryID"), reader.Unbox<int?>("DocumentWorkflowStepID"), reader.Unbox<DateTime?>("DocumentPublishFrom"),
        reader.Unbox<DateTime?>("DocumentPublishTo"), reader.Unbox<string?>("DocumentUrlPath"), reader.Unbox<string>("DocumentCulture"), reader.Unbox<int>("DocumentNodeID"), reader.Unbox<string?>("DocumentPageTitle"),
        reader.Unbox<string?>("DocumentPageKeyWords"), reader.Unbox<string?>("DocumentPageDescription"), reader.Unbox<bool>("DocumentShowInSiteMap"), reader.Unbox<bool>("DocumentMenuItemHideInNavigation"),
        reader.Unbox<string?>("DocumentMenuCaption"), reader.Unbox<string?>("DocumentMenuStyle"), reader.Unbox<string?>("DocumentMenuItemImage"), reader.Unbox<string?>("DocumentMenuItemLeftImage"), reader.Unbox<string?>("DocumentMenuItemRightImage"),
        reader.Unbox<int?>("DocumentPageTemplateID"), reader.Unbox<string?>("DocumentMenuJavascript"), reader.Unbox<string?>("DocumentMenuRedirectUrl"), reader.Unbox<bool?>("DocumentUseNamePathForUrlPath"), reader.Unbox<int?>("DocumentStylesheetID"),
        reader.Unbox<string?>("DocumentContent"), reader.Unbox<string?>("DocumentMenuClass"), reader.Unbox<string?>("DocumentMenuStyleHighlighted"), reader.Unbox<string?>("DocumentMenuClassHighlighted"),
        reader.Unbox<string?>("DocumentMenuItemImageHighlighted"), reader.Unbox<string?>("DocumentMenuItemLeftImageHighlighted"), reader.Unbox<string?>("DocumentMenuItemRightImageHighlighted"), reader.Unbox<bool?>("DocumentMenuItemInactive"),
        reader.Unbox<string?>("DocumentCustomData"), reader.Unbox<string?>("DocumentExtensions"), reader.Unbox<string?>("DocumentTags"), reader.Unbox<int?>("DocumentTagGroupID"), reader.Unbox<string?>("DocumentWildcardRule"),
        reader.Unbox<string?>("DocumentWebParts"), reader.Unbox<double?>("DocumentRatingValue"), reader.Unbox<int?>("DocumentRatings"), reader.Unbox<int?>("DocumentPriority"), reader.Unbox<string?>("DocumentType"),
        reader.Unbox<DateTime?>("DocumentLastPublished"), reader.Unbox<bool?>("DocumentUseCustomExtensions"), reader.Unbox<string?>("DocumentGroupWebParts"), reader.Unbox<bool?>("DocumentCheckedOutAutomatically"),
        reader.Unbox<string?>("DocumentTrackConversionName"), reader.Unbox<string?>("DocumentConversionValue"), reader.Unbox<bool?>("DocumentSearchExcluded"), reader.Unbox<string?>("DocumentLastVersionNumber"),
        reader.Unbox<bool?>("DocumentIsArchived"), reader.Unbox<string?>("DocumentHash"), reader.Unbox<bool?>("DocumentLogVisitActivity"), reader.Unbox<Guid?>("DocumentGUID"), reader.Unbox<Guid?>("DocumentWorkflowCycleGUID"),
        reader.Unbox<string?>("DocumentSitemapSettings"), reader.Unbox<bool?>("DocumentIsWaitingForTranslation"), reader.Unbox<string?>("DocumentSKUName"), reader.Unbox<string?>("DocumentSKUDescription"),
        reader.Unbox<string?>("DocumentSKUShortDescription"), reader.Unbox<string?>("DocumentWorkflowActionStatus"), reader.Unbox<bool?>("DocumentMenuRedirectToFirstChild"), reader.Unbox<bool>("DocumentCanBePublished"),
        reader.Unbox<bool>("DocumentInheritsStylesheet"), reader.Unbox<string?>("DocumentPageBuilderWidgets"), reader.Unbox<string?>("DocumentPageTemplateConfiguration"), reader.Unbox<string?>("DocumentABTestConfiguration")
    );
}

public record CmsDocumentK13(
    int DocumentID,
    string DocumentName,
    DateTime? DocumentModifiedWhen,
    int? DocumentModifiedByUserID,
    int? DocumentForeignKeyValue,
    int? DocumentCreatedByUserID,
    DateTime? DocumentCreatedWhen,
    int? DocumentCheckedOutByUserID,
    DateTime? DocumentCheckedOutWhen,
    int? DocumentCheckedOutVersionHistoryID,
    int? DocumentPublishedVersionHistoryID,
    int? DocumentWorkflowStepID,
    DateTime? DocumentPublishFrom,
    DateTime? DocumentPublishTo,
    string DocumentCulture,
    int DocumentNodeID,
    string? DocumentPageTitle,
    string? DocumentPageKeyWords,
    string? DocumentPageDescription,
    string? DocumentContent,
    string? DocumentCustomData,
    string? DocumentTags,
    int? DocumentTagGroupID,
    DateTime? DocumentLastPublished,
    bool? DocumentSearchExcluded,
    string? DocumentLastVersionNumber,
    bool? DocumentIsArchived,
    Guid? DocumentGUID,
    Guid? DocumentWorkflowCycleGUID,
    bool? DocumentIsWaitingForTranslation,
    string? DocumentSKUName,
    string? DocumentSKUDescription,
    string? DocumentSKUShortDescription,
    string? DocumentWorkflowActionStatus,
    bool DocumentCanBePublished,
    string? DocumentPageBuilderWidgets,
    string? DocumentPageTemplateConfiguration,
    string? DocumentABTestConfiguration,
    bool DocumentShowInMenu) : ICmsDocument, ISourceModel<CmsDocumentK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "DocumentID";
    public static string TableName => "CMS_Document";
    public static string GuidColumnName => "DocumentGUID";

    static CmsDocumentK13 ISourceModel<CmsDocumentK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("DocumentID"), reader.Unbox<string>("DocumentName"), reader.Unbox<DateTime?>("DocumentModifiedWhen"), reader.Unbox<int?>("DocumentModifiedByUserID"), reader.Unbox<int?>("DocumentForeignKeyValue"),
        reader.Unbox<int?>("DocumentCreatedByUserID"), reader.Unbox<DateTime?>("DocumentCreatedWhen"), reader.Unbox<int?>("DocumentCheckedOutByUserID"), reader.Unbox<DateTime?>("DocumentCheckedOutWhen"),
        reader.Unbox<int?>("DocumentCheckedOutVersionHistoryID"), reader.Unbox<int?>("DocumentPublishedVersionHistoryID"), reader.Unbox<int?>("DocumentWorkflowStepID"), reader.Unbox<DateTime?>("DocumentPublishFrom"),
        reader.Unbox<DateTime?>("DocumentPublishTo"), reader.Unbox<string>("DocumentCulture"), reader.Unbox<int>("DocumentNodeID"), reader.Unbox<string?>("DocumentPageTitle"), reader.Unbox<string?>("DocumentPageKeyWords"),
        reader.Unbox<string?>("DocumentPageDescription"), reader.Unbox<string?>("DocumentContent"), reader.Unbox<string?>("DocumentCustomData"), reader.Unbox<string?>("DocumentTags"), reader.Unbox<int?>("DocumentTagGroupID"),
        reader.Unbox<DateTime?>("DocumentLastPublished"), reader.Unbox<bool?>("DocumentSearchExcluded"), reader.Unbox<string?>("DocumentLastVersionNumber"), reader.Unbox<bool?>("DocumentIsArchived"), reader.Unbox<Guid?>("DocumentGUID"),
        reader.Unbox<Guid?>("DocumentWorkflowCycleGUID"), reader.Unbox<bool?>("DocumentIsWaitingForTranslation"), reader.Unbox<string?>("DocumentSKUName"), reader.Unbox<string?>("DocumentSKUDescription"),
        reader.Unbox<string?>("DocumentSKUShortDescription"), reader.Unbox<string?>("DocumentWorkflowActionStatus"), reader.Unbox<bool>("DocumentCanBePublished"), reader.Unbox<string?>("DocumentPageBuilderWidgets"),
        reader.Unbox<string?>("DocumentPageTemplateConfiguration"), reader.Unbox<string?>("DocumentABTestConfiguration"), reader.Unbox<bool>("DocumentShowInMenu")
    );

    public static CmsDocumentK13 FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("DocumentID"), reader.Unbox<string>("DocumentName"), reader.Unbox<DateTime?>("DocumentModifiedWhen"), reader.Unbox<int?>("DocumentModifiedByUserID"), reader.Unbox<int?>("DocumentForeignKeyValue"),
        reader.Unbox<int?>("DocumentCreatedByUserID"), reader.Unbox<DateTime?>("DocumentCreatedWhen"), reader.Unbox<int?>("DocumentCheckedOutByUserID"), reader.Unbox<DateTime?>("DocumentCheckedOutWhen"),
        reader.Unbox<int?>("DocumentCheckedOutVersionHistoryID"), reader.Unbox<int?>("DocumentPublishedVersionHistoryID"), reader.Unbox<int?>("DocumentWorkflowStepID"), reader.Unbox<DateTime?>("DocumentPublishFrom"),
        reader.Unbox<DateTime?>("DocumentPublishTo"), reader.Unbox<string>("DocumentCulture"), reader.Unbox<int>("DocumentNodeID"), reader.Unbox<string?>("DocumentPageTitle"), reader.Unbox<string?>("DocumentPageKeyWords"),
        reader.Unbox<string?>("DocumentPageDescription"), reader.Unbox<string?>("DocumentContent"), reader.Unbox<string?>("DocumentCustomData"), reader.Unbox<string?>("DocumentTags"), reader.Unbox<int?>("DocumentTagGroupID"),
        reader.Unbox<DateTime?>("DocumentLastPublished"), reader.Unbox<bool?>("DocumentSearchExcluded"), reader.Unbox<string?>("DocumentLastVersionNumber"), reader.Unbox<bool?>("DocumentIsArchived"), reader.Unbox<Guid?>("DocumentGUID"),
        reader.Unbox<Guid?>("DocumentWorkflowCycleGUID"), reader.Unbox<bool?>("DocumentIsWaitingForTranslation"), reader.Unbox<string?>("DocumentSKUName"), reader.Unbox<string?>("DocumentSKUDescription"),
        reader.Unbox<string?>("DocumentSKUShortDescription"), reader.Unbox<string?>("DocumentWorkflowActionStatus"), reader.Unbox<bool>("DocumentCanBePublished"), reader.Unbox<string?>("DocumentPageBuilderWidgets"),
        reader.Unbox<string?>("DocumentPageTemplateConfiguration"), reader.Unbox<string?>("DocumentABTestConfiguration"), reader.Unbox<bool>("DocumentShowInMenu")
    );
}
