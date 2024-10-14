using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.KX12.Models;

[Table("CMS_Badge")]
public class CmsBadge
{
    [Key]
    [Column("BadgeID")]
    public int BadgeId { get; set; }

    [StringLength(100)]
    public string BadgeName { get; set; } = null!;

    [StringLength(200)]
    public string BadgeDisplayName { get; set; } = null!;

    [Column("BadgeImageURL")]
    [StringLength(200)]
    public string? BadgeImageUrl { get; set; }

    public bool BadgeIsAutomatic { get; set; }

    public int? BadgeTopLimit { get; set; }

    [Column("BadgeGUID")]
    public Guid BadgeGuid { get; set; }

    public DateTime BadgeLastModified { get; set; }

    [InverseProperty("UserBadge")]
    public virtual ICollection<CmsUserSetting> CmsUserSettings { get; set; } = new List<CmsUserSetting>();
}
