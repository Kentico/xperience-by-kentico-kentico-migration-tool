using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models;

[Table("CMS_ContentItemLanguageMetadata")]
[Index("ContentItemLanguageMetadataContentItemId", "ContentItemLanguageMetadataContentLanguageId", "ContentItemLanguageMetadataLatestVersionStatus", Name = "IX_CMS_ContentItemLanguageMetadata_ContentItemID_ContentLanguageID_LatestVersionStatus", IsUnique = true)]
[Index("ContentItemLanguageMetadataContentWorkflowStepId", Name = "IX_CMS_ContentItemLanguageMetadata_ContentItemLanguageMetadataContentWorkflowStepID")]
[Index("ContentItemLanguageMetadataCreatedByUserId", Name = "IX_CMS_ContentItemLanguageMetadata_ContentItemLanguageMetadataCreatedByUserID")]
[Index("ContentItemLanguageMetadataModifiedByUserId", Name = "IX_CMS_ContentItemLanguageMetadata_ContentItemLanguageMetadataModifiedByUserID")]
public partial class CmsContentItemLanguageMetadatum
{
    [Key]
    [Column("ContentItemLanguageMetadataID")]
    public int ContentItemLanguageMetadataId { get; set; }

    [Column("ContentItemLanguageMetadataContentItemID")]
    public int ContentItemLanguageMetadataContentItemId { get; set; }

    [StringLength(100)]
    public string ContentItemLanguageMetadataDisplayName { get; set; } = null!;

    public int ContentItemLanguageMetadataLatestVersionStatus { get; set; }

    [Column("ContentItemLanguageMetadataGUID")]
    public Guid ContentItemLanguageMetadataGuid { get; set; }

    public DateTime ContentItemLanguageMetadataCreatedWhen { get; set; }

    [Column("ContentItemLanguageMetadataCreatedByUserID")]
    public int? ContentItemLanguageMetadataCreatedByUserId { get; set; }

    public DateTime ContentItemLanguageMetadataModifiedWhen { get; set; }

    [Column("ContentItemLanguageMetadataModifiedByUserID")]
    public int? ContentItemLanguageMetadataModifiedByUserId { get; set; }

    public bool ContentItemLanguageMetadataHasImageAsset { get; set; }

    [Column("ContentItemLanguageMetadataContentLanguageID")]
    public int ContentItemLanguageMetadataContentLanguageId { get; set; }

    [Column("ContentItemLanguageMetadataContentWorkflowStepID")]
    public int? ContentItemLanguageMetadataContentWorkflowStepId { get; set; }

    public DateTime? ContentItemLanguageMetadataScheduledPublishWhen { get; set; }

    [InverseProperty("ContentItemTagContentItemLanguageMetadata")]
    public virtual ICollection<CmsContentItemTag> CmsContentItemTags { get; set; } = new List<CmsContentItemTag>();

    [ForeignKey("ContentItemLanguageMetadataContentItemId")]
    [InverseProperty("CmsContentItemLanguageMetadata")]
    public virtual CmsContentItem ContentItemLanguageMetadataContentItem { get; set; } = null!;

    [ForeignKey("ContentItemLanguageMetadataContentLanguageId")]
    [InverseProperty("CmsContentItemLanguageMetadata")]
    public virtual CmsContentLanguage ContentItemLanguageMetadataContentLanguage { get; set; } = null!;

    [ForeignKey("ContentItemLanguageMetadataContentWorkflowStepId")]
    [InverseProperty("CmsContentItemLanguageMetadata")]
    public virtual CmsContentWorkflowStep? ContentItemLanguageMetadataContentWorkflowStep { get; set; }

    [ForeignKey("ContentItemLanguageMetadataCreatedByUserId")]
    [InverseProperty("CmsContentItemLanguageMetadatumContentItemLanguageMetadataCreatedByUsers")]
    public virtual CmsUser? ContentItemLanguageMetadataCreatedByUser { get; set; }

    [ForeignKey("ContentItemLanguageMetadataModifiedByUserId")]
    [InverseProperty("CmsContentItemLanguageMetadatumContentItemLanguageMetadataModifiedByUsers")]
    public virtual CmsUser? ContentItemLanguageMetadataModifiedByUser { get; set; }
}
