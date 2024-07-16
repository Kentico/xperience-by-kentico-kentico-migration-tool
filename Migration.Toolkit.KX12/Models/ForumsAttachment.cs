using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("Forums_Attachment")]
[Index("AttachmentSiteId", "AttachmentGuid", Name = "IX_Forums_Attachment_AttachmentGUID", IsUnique = true)]
[Index("AttachmentPostId", Name = "IX_Forums_Attachment_AttachmentPostID")]
public partial class ForumsAttachment
{
    [Key]
    [Column("AttachmentID")]
    public int AttachmentId { get; set; }

    [StringLength(200)]
    public string AttachmentFileName { get; set; } = null!;

    [StringLength(10)]
    public string AttachmentFileExtension { get; set; } = null!;

    public byte[]? AttachmentBinary { get; set; }

    [Column("AttachmentGUID")]
    public Guid AttachmentGuid { get; set; }

    public DateTime AttachmentLastModified { get; set; }

    [StringLength(100)]
    public string AttachmentMimeType { get; set; } = null!;

    public int AttachmentFileSize { get; set; }

    public int? AttachmentImageHeight { get; set; }

    public int? AttachmentImageWidth { get; set; }

    [Column("AttachmentPostID")]
    public int AttachmentPostId { get; set; }

    [Column("AttachmentSiteID")]
    public int AttachmentSiteId { get; set; }

    [ForeignKey("AttachmentPostId")]
    [InverseProperty("ForumsAttachments")]
    public virtual ForumsForumPost AttachmentPost { get; set; } = null!;

    [ForeignKey("AttachmentSiteId")]
    [InverseProperty("ForumsAttachments")]
    public virtual CmsSite AttachmentSite { get; set; } = null!;
}
