using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("CMS_BannerCategory")]
[Index("BannerCategoryName", "BannerCategorySiteId", Name = "IX_CMS_BannerCategory_BannerCategoryName_BannerCategorySiteID", IsUnique = true)]
[Index("BannerCategorySiteId", Name = "IX_CMS_BannerCategory_BannerCategorySiteID")]
public class CmsBannerCategory
{
    [Key]
    [Column("BannerCategoryID")]
    public int BannerCategoryId { get; set; }

    [StringLength(100)]
    public string BannerCategoryName { get; set; } = null!;

    [StringLength(200)]
    public string BannerCategoryDisplayName { get; set; } = null!;

    [Column("BannerCategorySiteID")]
    public int? BannerCategorySiteId { get; set; }

    public Guid BannerCategoryGuid { get; set; }

    public DateTime BannerCategoryLastModified { get; set; }

    [Required]
    public bool? BannerCategoryEnabled { get; set; }

    [ForeignKey("BannerCategorySiteId")]
    [InverseProperty("CmsBannerCategories")]
    public virtual CmsSite? BannerCategorySite { get; set; }

    [InverseProperty("BannerCategory")]
    public virtual ICollection<CmsBanner> CmsBanners { get; set; } = [];
}
