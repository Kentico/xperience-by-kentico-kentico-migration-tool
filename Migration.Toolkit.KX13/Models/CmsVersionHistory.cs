using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_VersionHistory")]
[Index("ModifiedByUserId", Name = "IX_CMS_VersionHistory_ModifiedByUserID")]
[Index("NodeSiteId", Name = "IX_CMS_VersionHistory_NodeSiteID")]
[Index("ToBePublished", "PublishFrom", "PublishTo", Name = "IX_CMS_VersionHistory_ToBePublished_PublishFrom_PublishTo")]
[Index("VersionClassId", Name = "IX_CMS_VersionHistory_VersionClassID")]
[Index("VersionDeletedByUserId", "VersionDeletedWhen", Name = "IX_CMS_VersionHistory_VersionDeletedByUserID_VersionDeletedWhen", IsDescending = new[] { false, true })]
[Index("VersionWorkflowId", Name = "IX_CMS_VersionHistory_VersionWorkflowID")]
[Index("VersionWorkflowStepId", Name = "IX_CMS_VersionHistory_VersionWorkflowStepID")]
public partial class CmsVersionHistory
{
    [Key]
    [Column("VersionHistoryID")]
    public int VersionHistoryId { get; set; }

    [Column("NodeSiteID")]
    public int NodeSiteId { get; set; }

    [Column("DocumentID")]
    public int? DocumentId { get; set; }

    [Column("NodeXML")]
    public string NodeXml { get; set; } = null!;

    [Column("ModifiedByUserID")]
    public int? ModifiedByUserId { get; set; }

    public DateTime ModifiedWhen { get; set; }

    [StringLength(50)]
    public string? VersionNumber { get; set; }

    public string? VersionComment { get; set; }

    public bool ToBePublished { get; set; }

    public DateTime? PublishFrom { get; set; }

    public DateTime? PublishTo { get; set; }

    public DateTime? WasPublishedFrom { get; set; }

    public DateTime? WasPublishedTo { get; set; }

    [StringLength(100)]
    public string? VersionDocumentName { get; set; }

    [Column("VersionClassID")]
    public int? VersionClassId { get; set; }

    [Column("VersionWorkflowID")]
    public int? VersionWorkflowId { get; set; }

    [Column("VersionWorkflowStepID")]
    public int? VersionWorkflowStepId { get; set; }

    [StringLength(450)]
    public string? VersionNodeAliasPath { get; set; }

    [Column("VersionDeletedByUserID")]
    public int? VersionDeletedByUserId { get; set; }

    public DateTime? VersionDeletedWhen { get; set; }

    [InverseProperty("DocumentCheckedOutVersionHistory")]
    public virtual ICollection<CmsDocument> CmsDocumentDocumentCheckedOutVersionHistories { get; set; } = new List<CmsDocument>();

    [InverseProperty("DocumentPublishedVersionHistory")]
    public virtual ICollection<CmsDocument> CmsDocumentDocumentPublishedVersionHistories { get; set; } = new List<CmsDocument>();

    [InverseProperty("VersionHistory")]
    public virtual ICollection<CmsWorkflowHistory> CmsWorkflowHistories { get; set; } = new List<CmsWorkflowHistory>();

    [ForeignKey("ModifiedByUserId")]
    [InverseProperty("CmsVersionHistoryModifiedByUsers")]
    public virtual CmsUser? ModifiedByUser { get; set; }

    [ForeignKey("NodeSiteId")]
    [InverseProperty("CmsVersionHistories")]
    public virtual CmsSite NodeSite { get; set; } = null!;

    [ForeignKey("VersionClassId")]
    [InverseProperty("CmsVersionHistories")]
    public virtual CmsClass? VersionClass { get; set; }

    [ForeignKey("VersionDeletedByUserId")]
    [InverseProperty("CmsVersionHistoryVersionDeletedByUsers")]
    public virtual CmsUser? VersionDeletedByUser { get; set; }

    [ForeignKey("VersionWorkflowId")]
    [InverseProperty("CmsVersionHistories")]
    public virtual CmsWorkflow? VersionWorkflow { get; set; }

    [ForeignKey("VersionWorkflowStepId")]
    [InverseProperty("CmsVersionHistories")]
    public virtual CmsWorkflowStep? VersionWorkflowStep { get; set; }

    [ForeignKey("VersionHistoryId")]
    [InverseProperty("VersionHistories")]
    public virtual ICollection<CmsAttachmentHistory> AttachmentHistories { get; set; } = new List<CmsAttachmentHistory>();
}
