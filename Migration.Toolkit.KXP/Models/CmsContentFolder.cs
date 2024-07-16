using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models;

[Table("CMS_ContentFolder")]
[Index("ContentFolderCreatedByUserId", Name = "IX_CMS_ContentFolder_ContentFolderCreatedByUserID")]
[Index("ContentFolderParentFolderId", Name = "IX_CMS_ContentFolder_ContentFolderDisplayName_ContentFolderParentID")]
[Index("ContentFolderGuid", Name = "IX_CMS_ContentFolder_ContentFolderGUID_Unique", IsUnique = true)]
[Index("ContentFolderModifiedByUserId", Name = "IX_CMS_ContentFolder_ContentFolderModifiedByUserID")]
[Index("ContentFolderParentFolderId", Name = "IX_CMS_ContentFolder_ContentFolderParentFolderID")]
public class CmsContentFolder
{
    [Key]
    [Column("ContentFolderID")]
    public int ContentFolderId { get; set; }

    [Column("ContentFolderGUID")]
    public Guid ContentFolderGuid { get; set; }

    [StringLength(50)]
    public string ContentFolderName { get; set; } = null!;

    [StringLength(50)]
    public string ContentFolderDisplayName { get; set; } = null!;

    [StringLength(1020)]
    public string? ContentFolderTreePath { get; set; }

    public DateTime ContentFolderCreatedWhen { get; set; }

    [Column("ContentFolderCreatedByUserID")]
    public int? ContentFolderCreatedByUserId { get; set; }

    public DateTime ContentFolderModifiedWhen { get; set; }

    [Column("ContentFolderModifiedByUserID")]
    public int? ContentFolderModifiedByUserId { get; set; }

    [Column("ContentFolderParentFolderID")]
    public int? ContentFolderParentFolderId { get; set; }

    [InverseProperty("ContentItemContentFolder")]
    public virtual ICollection<CmsContentItem> CmsContentItems { get; set; } = new List<CmsContentItem>();

    [ForeignKey("ContentFolderCreatedByUserId")]
    [InverseProperty("CmsContentFolderContentFolderCreatedByUsers")]
    public virtual CmsUser? ContentFolderCreatedByUser { get; set; }

    [ForeignKey("ContentFolderModifiedByUserId")]
    [InverseProperty("CmsContentFolderContentFolderModifiedByUsers")]
    public virtual CmsUser? ContentFolderModifiedByUser { get; set; }

    [ForeignKey("ContentFolderParentFolderId")]
    [InverseProperty("InverseContentFolderParentFolder")]
    public virtual CmsContentFolder? ContentFolderParentFolder { get; set; }

    [InverseProperty("ContentFolderParentFolder")]
    public virtual ICollection<CmsContentFolder> InverseContentFolderParentFolder { get; set; } = new List<CmsContentFolder>();
}
