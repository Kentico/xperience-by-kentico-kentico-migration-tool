using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[Table("Media_Library")]
[Index("LibraryGroupId", Name = "IX_Media_Library_LibraryGroupID")]
[Index("LibrarySiteId", "LibraryName", "LibraryGuid", Name = "IX_Media_Library_LibrarySiteID_LibraryName_LibraryGUID", IsUnique = true)]
public class MediaLibrary
{
    [Key]
    [Column("LibraryID")]
    public int LibraryId { get; set; }

    [StringLength(250)]
    public string LibraryName { get; set; } = null!;

    [StringLength(250)]
    public string LibraryDisplayName { get; set; } = null!;

    public string? LibraryDescription { get; set; }

    [StringLength(250)]
    public string LibraryFolder { get; set; } = null!;

    public int? LibraryAccess { get; set; }

    [Column("LibraryGroupID")]
    public int? LibraryGroupId { get; set; }

    [Column("LibrarySiteID")]
    public int LibrarySiteId { get; set; }

    [Column("LibraryGUID")]
    public Guid? LibraryGuid { get; set; }

    public DateTime? LibraryLastModified { get; set; }

    [StringLength(450)]
    public string? LibraryTeaserPath { get; set; }

    [Column("LibraryTeaserGUID")]
    public Guid? LibraryTeaserGuid { get; set; }

    [ForeignKey("LibraryGroupId")]
    [InverseProperty("MediaLibraries")]
    public virtual CommunityGroup? LibraryGroup { get; set; }

    [ForeignKey("LibrarySiteId")]
    [InverseProperty("MediaLibraries")]
    public virtual CmsSite LibrarySite { get; set; } = null!;

    [InverseProperty("FileLibrary")]
    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();

    [InverseProperty("Library")]
    public virtual ICollection<MediaLibraryRolePermission> MediaLibraryRolePermissions { get; set; } = new List<MediaLibraryRolePermission>();
}
