using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[Table("CMS_AttachmentHistory")]
[Index("AttachmentGuid", Name = "IX_CMS_AttachmentHistory_AttachmentGUID")]
[Index("AttachmentIsUnsorted", "AttachmentGroupGuid", "AttachmentOrder", Name = "IX_CMS_AttachmentHistory_AttachmentIsUnsorted_AttachmentGroupGUID_AttachmentOrder")]
[Index("AttachmentSiteId", Name = "IX_CMS_AttachmentHistory_AttachmentSiteID")]
[Index("AttachmentVariantParentId", Name = "IX_CMS_AttachmentHistory_AttachmentVariantParentID")]
public class CmsAttachmentHistory
{
    [Key]
    [Column("AttachmentHistoryID")]
    public int AttachmentHistoryId { get; set; }

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
    public int AttachmentDocumentId { get; set; }

    [Column("AttachmentGUID")]
    public Guid AttachmentGuid { get; set; }

    public bool? AttachmentIsUnsorted { get; set; }

    public int? AttachmentOrder { get; set; }

    [Column("AttachmentGroupGUID")]
    public Guid? AttachmentGroupGuid { get; set; }

    [StringLength(32)]
    public string? AttachmentHash { get; set; }

    [StringLength(250)]
    public string? AttachmentTitle { get; set; }

    public string? AttachmentDescription { get; set; }

    public string? AttachmentCustomData { get; set; }

    public DateTime? AttachmentLastModified { get; set; }

    [Column("AttachmentHistoryGUID")]
    public Guid AttachmentHistoryGuid { get; set; }

    [Column("AttachmentSiteID")]
    public int AttachmentSiteId { get; set; }

    public string? AttachmentSearchContent { get; set; }

    [StringLength(50)]
    public string? AttachmentVariantDefinitionIdentifier { get; set; }

    [Column("AttachmentVariantParentID")]
    public int? AttachmentVariantParentId { get; set; }

    [ForeignKey("AttachmentSiteId")]
    [InverseProperty("CmsAttachmentHistories")]
    public virtual CmsSite AttachmentSite { get; set; } = null!;

    [ForeignKey("AttachmentVariantParentId")]
    [InverseProperty("InverseAttachmentVariantParent")]
    public virtual CmsAttachmentHistory? AttachmentVariantParent { get; set; }

    [InverseProperty("AttachmentVariantParent")]
    public virtual ICollection<CmsAttachmentHistory> InverseAttachmentVariantParent { get; set; } = new List<CmsAttachmentHistory>();

    [ForeignKey("AttachmentHistoryId")]
    [InverseProperty("AttachmentHistories")]
    public virtual ICollection<CmsVersionHistory> VersionHistories { get; set; } = new List<CmsVersionHistory>();
}
