using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KXP.Models;

[Table("CMS_SmartFolder")]
[Index("SmartFolderCreatedByUserId", Name = "IX_CMS_SmartFolder_SmartFolderCreatedByUserID")]
[Index("SmartFolderGuid", Name = "IX_CMS_SmartFolder_SmartFolderGUID_Unique", IsUnique = true)]
[Index("SmartFolderModifiedByUserId", Name = "IX_CMS_SmartFolder_SmartFolderModifiedByUserID")]
public class CmsSmartFolder
{
    [Key]
    [Column("SmartFolderID")]
    public int SmartFolderId { get; set; }

    [Column("SmartFolderGUID")]
    public Guid SmartFolderGuid { get; set; }

    [StringLength(50)]
    public string SmartFolderName { get; set; } = null!;

    [StringLength(50)]
    public string SmartFolderDisplayName { get; set; } = null!;

    public DateTime SmartFolderCreatedWhen { get; set; }

    [Column("SmartFolderCreatedByUserID")]
    public int? SmartFolderCreatedByUserId { get; set; }

    public DateTime? SmartFolderModifiedWhen { get; set; }

    [Column("SmartFolderModifiedByUserID")]
    public int? SmartFolderModifiedByUserId { get; set; }

    public string SmartFolderFilter { get; set; } = null!;

    [ForeignKey("SmartFolderCreatedByUserId")]
    [InverseProperty("CmsSmartFolderSmartFolderCreatedByUsers")]
    public virtual CmsUser? SmartFolderCreatedByUser { get; set; }

    [ForeignKey("SmartFolderModifiedByUserId")]
    [InverseProperty("CmsSmartFolderSmartFolderModifiedByUsers")]
    public virtual CmsUser? SmartFolderModifiedByUser { get; set; }
}
