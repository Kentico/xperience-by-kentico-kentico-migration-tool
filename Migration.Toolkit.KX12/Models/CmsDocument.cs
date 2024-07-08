using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("CMS_Document")]
[Index("DocumentCheckedOutByUserId", Name = "IX_CMS_Document_DocumentCheckedOutByUserID")]
[Index("DocumentCheckedOutVersionHistoryId", Name = "IX_CMS_Document_DocumentCheckedOutVersionHistoryID")]
[Index("DocumentCreatedByUserId", Name = "IX_CMS_Document_DocumentCreatedByUserID")]
[Index("DocumentCulture", Name = "IX_CMS_Document_DocumentCulture")]
[Index("DocumentForeignKeyValue", "DocumentId", "DocumentNodeId", Name = "IX_CMS_Document_DocumentForeignKeyValue_DocumentID_DocumentNodeID")]
[Index("DocumentModifiedByUserId", Name = "IX_CMS_Document_DocumentModifiedByUserID")]
[Index("DocumentNodeId", "DocumentId", "DocumentCulture", Name = "IX_CMS_Document_DocumentNodeID_DocumentID_DocumentCulture", IsUnique = true)]
[Index("DocumentPageTemplateId", Name = "IX_CMS_Document_DocumentPageTemplateID")]
[Index("DocumentPublishedVersionHistoryId", Name = "IX_CMS_Document_DocumentPublishedVersionHistoryID")]
[Index("DocumentTagGroupId", Name = "IX_CMS_Document_DocumentTagGroupID")]
[Index("DocumentUrlPath", Name = "IX_CMS_Document_DocumentUrlPath_DocumentID_DocumentNodeID")]
[Index("DocumentWildcardRule", "DocumentPriority", Name = "IX_CMS_Document_DocumentWildcardRule_DocumentPriority")]
[Index("DocumentWorkflowStepId", Name = "IX_CMS_Document_DocumentWorkflowStepID")]
public partial class CmsDocument
{
    [Key]
    [Column("DocumentID")]
    public int DocumentId { get; set; }

    [StringLength(100)]
    public string DocumentName { get; set; } = null!;

    [StringLength(1500)]
    public string? DocumentNamePath { get; set; }

    public DateTime? DocumentModifiedWhen { get; set; }

    [Column("DocumentModifiedByUserID")]
    public int? DocumentModifiedByUserId { get; set; }

    public int? DocumentForeignKeyValue { get; set; }

    [Column("DocumentCreatedByUserID")]
    public int? DocumentCreatedByUserId { get; set; }

    public DateTime? DocumentCreatedWhen { get; set; }

    [Column("DocumentCheckedOutByUserID")]
    public int? DocumentCheckedOutByUserId { get; set; }

    public DateTime? DocumentCheckedOutWhen { get; set; }

    [Column("DocumentCheckedOutVersionHistoryID")]
    public int? DocumentCheckedOutVersionHistoryId { get; set; }

    [Column("DocumentPublishedVersionHistoryID")]
    public int? DocumentPublishedVersionHistoryId { get; set; }

    [Column("DocumentWorkflowStepID")]
    public int? DocumentWorkflowStepId { get; set; }

    public DateTime? DocumentPublishFrom { get; set; }

    public DateTime? DocumentPublishTo { get; set; }

    public string? DocumentUrlPath { get; set; }

    [StringLength(10)]
    public string DocumentCulture { get; set; } = null!;

    [Column("DocumentNodeID")]
    public int DocumentNodeId { get; set; }

    public string? DocumentPageTitle { get; set; }

    public string? DocumentPageKeyWords { get; set; }

    public string? DocumentPageDescription { get; set; }

    public bool DocumentShowInSiteMap { get; set; }

    public bool DocumentMenuItemHideInNavigation { get; set; }

    [StringLength(200)]
    public string? DocumentMenuCaption { get; set; }

    [StringLength(100)]
    public string? DocumentMenuStyle { get; set; }

    [StringLength(200)]
    public string? DocumentMenuItemImage { get; set; }

    [StringLength(200)]
    public string? DocumentMenuItemLeftImage { get; set; }

    [StringLength(200)]
    public string? DocumentMenuItemRightImage { get; set; }

    [Column("DocumentPageTemplateID")]
    public int? DocumentPageTemplateId { get; set; }

    [StringLength(450)]
    public string? DocumentMenuJavascript { get; set; }

    [StringLength(450)]
    public string? DocumentMenuRedirectUrl { get; set; }

    public bool? DocumentUseNamePathForUrlPath { get; set; }

    [Column("DocumentStylesheetID")]
    public int? DocumentStylesheetId { get; set; }

    public string? DocumentContent { get; set; }

    [StringLength(100)]
    public string? DocumentMenuClass { get; set; }

    [StringLength(200)]
    public string? DocumentMenuStyleHighlighted { get; set; }

    [StringLength(100)]
    public string? DocumentMenuClassHighlighted { get; set; }

    [StringLength(200)]
    public string? DocumentMenuItemImageHighlighted { get; set; }

    [StringLength(200)]
    public string? DocumentMenuItemLeftImageHighlighted { get; set; }

    [StringLength(200)]
    public string? DocumentMenuItemRightImageHighlighted { get; set; }

    public bool? DocumentMenuItemInactive { get; set; }

    public string? DocumentCustomData { get; set; }

    [StringLength(100)]
    public string? DocumentExtensions { get; set; }

    public string? DocumentTags { get; set; }

    [Column("DocumentTagGroupID")]
    public int? DocumentTagGroupId { get; set; }

    [StringLength(440)]
    public string? DocumentWildcardRule { get; set; }

    public string? DocumentWebParts { get; set; }

    public double? DocumentRatingValue { get; set; }

    public int? DocumentRatings { get; set; }

    public int? DocumentPriority { get; set; }

    [StringLength(50)]
    public string? DocumentType { get; set; }

    public DateTime? DocumentLastPublished { get; set; }

    public bool? DocumentUseCustomExtensions { get; set; }

    public string? DocumentGroupWebParts { get; set; }

    public bool? DocumentCheckedOutAutomatically { get; set; }

    [StringLength(200)]
    public string? DocumentTrackConversionName { get; set; }

    [StringLength(100)]
    public string? DocumentConversionValue { get; set; }

    public bool? DocumentSearchExcluded { get; set; }

    [StringLength(50)]
    public string? DocumentLastVersionNumber { get; set; }

    public bool? DocumentIsArchived { get; set; }

    [StringLength(32)]
    public string? DocumentHash { get; set; }

    public bool? DocumentLogVisitActivity { get; set; }

    [Column("DocumentGUID")]
    public Guid? DocumentGuid { get; set; }

    [Column("DocumentWorkflowCycleGUID")]
    public Guid? DocumentWorkflowCycleGuid { get; set; }

    [StringLength(100)]
    public string? DocumentSitemapSettings { get; set; }

    public bool? DocumentIsWaitingForTranslation { get; set; }

    [Column("DocumentSKUName")]
    [StringLength(440)]
    public string? DocumentSkuname { get; set; }

    [Column("DocumentSKUDescription")]
    public string? DocumentSkudescription { get; set; }

    [Column("DocumentSKUShortDescription")]
    public string? DocumentSkushortDescription { get; set; }

    [StringLength(450)]
    public string? DocumentWorkflowActionStatus { get; set; }

    public bool? DocumentMenuRedirectToFirstChild { get; set; }

    [Required]
    public bool? DocumentCanBePublished { get; set; }

    [Required]
    public bool? DocumentInheritsStylesheet { get; set; }

    public string? DocumentPageBuilderWidgets { get; set; }

    public string? DocumentPageTemplateConfiguration { get; set; }

    [Column("DocumentABTestConfiguration")]
    public string? DocumentAbtestConfiguration { get; set; }

    [InverseProperty("CommentPostDocument")]
    public virtual ICollection<BlogComment> BlogComments { get; set; } = new List<BlogComment>();

    [InverseProperty("SubscriptionPostDocument")]
    public virtual ICollection<BlogPostSubscription> BlogPostSubscriptions { get; set; } = new List<BlogPostSubscription>();

    [InverseProperty("BoardDocument")]
    public virtual ICollection<BoardBoard> BoardBoards { get; set; } = new List<BoardBoard>();

    [InverseProperty("AlternativeUrlDocument")]
    public virtual ICollection<CmsAlternativeUrl> CmsAlternativeUrls { get; set; } = new List<CmsAlternativeUrl>();

    [InverseProperty("AttachmentDocument")]
    public virtual ICollection<CmsAttachment> CmsAttachments { get; set; } = new List<CmsAttachment>();

    [InverseProperty("PersonalizationDocument")]
    public virtual ICollection<CmsPersonalization> CmsPersonalizations { get; set; } = new List<CmsPersonalization>();

    [ForeignKey("DocumentCheckedOutByUserId")]
    [InverseProperty("CmsDocumentDocumentCheckedOutByUsers")]
    public virtual CmsUser? DocumentCheckedOutByUser { get; set; }

    [ForeignKey("DocumentCheckedOutVersionHistoryId")]
    [InverseProperty("CmsDocumentDocumentCheckedOutVersionHistories")]
    public virtual CmsVersionHistory? DocumentCheckedOutVersionHistory { get; set; }

    [ForeignKey("DocumentCreatedByUserId")]
    [InverseProperty("CmsDocumentDocumentCreatedByUsers")]
    public virtual CmsUser? DocumentCreatedByUser { get; set; }

    [ForeignKey("DocumentModifiedByUserId")]
    [InverseProperty("CmsDocumentDocumentModifiedByUsers")]
    public virtual CmsUser? DocumentModifiedByUser { get; set; }

    [ForeignKey("DocumentNodeId")]
    [InverseProperty("CmsDocuments")]
    public virtual CmsTree DocumentNode { get; set; } = null!;

    [ForeignKey("DocumentPageTemplateId")]
    [InverseProperty("CmsDocuments")]
    public virtual CmsPageTemplate? DocumentPageTemplate { get; set; }

    [ForeignKey("DocumentPublishedVersionHistoryId")]
    [InverseProperty("CmsDocumentDocumentPublishedVersionHistories")]
    public virtual CmsVersionHistory? DocumentPublishedVersionHistory { get; set; }

    [ForeignKey("DocumentStylesheetId")]
    [InverseProperty("CmsDocuments")]
    public virtual CmsCssStylesheet? DocumentStylesheet { get; set; }

    [ForeignKey("DocumentTagGroupId")]
    [InverseProperty("CmsDocuments")]
    public virtual CmsTagGroup? DocumentTagGroup { get; set; }

    [ForeignKey("DocumentWorkflowStepId")]
    [InverseProperty("CmsDocuments")]
    public virtual CmsWorkflowStep? DocumentWorkflowStep { get; set; }

    [InverseProperty("ForumDocument")]
    public virtual ICollection<ForumsForum> ForumsForums { get; set; } = new List<ForumsForum>();

    [InverseProperty("VariantDocument")]
    public virtual ICollection<OmPersonalizationVariant> OmPersonalizationVariants { get; set; } = new List<OmPersonalizationVariant>();

    [ForeignKey("DocumentId")]
    [InverseProperty("Documents")]
    public virtual ICollection<CmsCategory> Categories { get; set; } = new List<CmsCategory>();

    [ForeignKey("DocumentId")]
    [InverseProperty("Documents")]
    public virtual ICollection<CmsTag> Tags { get; set; } = new List<CmsTag>();
}