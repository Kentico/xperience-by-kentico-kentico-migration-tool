using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_Attachment")]
[Index("AttachmentDocumentId", Name = "IX_CMS_Attachment_AttachmentDocumentID")]
[Index("AttachmentGuid", "AttachmentSiteId", Name = "IX_CMS_Attachment_AttachmentGUID_AttachmentSiteID")]
[Index("AttachmentIsUnsorted", "AttachmentGroupGuid", "AttachmentFormGuid", "AttachmentOrder", Name = "IX_CMS_Attachment_AttachmentIsUnsorted_AttachmentGroupGUID_AttachmentFormGUID_AttachmentOrder")]
[Index("AttachmentSiteId", Name = "IX_CMS_Attachment_AttachmentSiteID")]
[Index("AttachmentVariantParentId", Name = "IX_CMS_Attachment_AttachmentVariantParentID")]
public partial class CmsAttachment
{
    [Key]
    [Column("AttachmentID")]
    public int AttachmentId { get; set; }

    [StringLength(255)]
    public string AttachmentName { get; set; } = null!;

    [StringLength(50)]
    public string AttachmentExtension { get; set; } = null!;

    public int AttachmentSize { get; set; }

    [StringLength(100)]
    public string AttachmentMimeType { get; set; } = null!;

    public byte[]? AttachmentBinary { get; set; }

    public int? AttachmentImageWidth { get; set; }

    public int? AttachmentImageHeight { get; set; }

    [Column("AttachmentDocumentID")]
    public int? AttachmentDocumentId { get; set; }

    [Column("AttachmentGUID")]
    public Guid AttachmentGuid { get; set; }

    [Column("AttachmentSiteID")]
    public int AttachmentSiteId { get; set; }

    public DateTime AttachmentLastModified { get; set; }

    public bool? AttachmentIsUnsorted { get; set; }

    public int? AttachmentOrder { get; set; }

    [Column("AttachmentGroupGUID")]
    public Guid? AttachmentGroupGuid { get; set; }

    [Column("AttachmentFormGUID")]
    public Guid? AttachmentFormGuid { get; set; }

    [StringLength(32)]
    public string? AttachmentHash { get; set; }

    [StringLength(250)]
    public string? AttachmentTitle { get; set; }

    public string? AttachmentDescription { get; set; }

    public string? AttachmentCustomData { get; set; }

    public string? AttachmentSearchContent { get; set; }

    [StringLength(50)]
    public string? AttachmentVariantDefinitionIdentifier { get; set; }

    [Column("AttachmentVariantParentID")]
    public int? AttachmentVariantParentId { get; set; }

    [ForeignKey("AttachmentDocumentId")]
    [InverseProperty("CmsAttachments")]
    public virtual CmsDocument? AttachmentDocument { get; set; }

    [ForeignKey("AttachmentSiteId")]
    [InverseProperty("CmsAttachments")]
    public virtual CmsSite AttachmentSite { get; set; } = null!;

    [ForeignKey("AttachmentVariantParentId")]
    [InverseProperty("InverseAttachmentVariantParent")]
    public virtual CmsAttachment? AttachmentVariantParent { get; set; }

    [InverseProperty("AttachmentVariantParent")]
    public virtual ICollection<CmsAttachment> InverseAttachmentVariantParent { get; set; } = new List<CmsAttachment>();
}
