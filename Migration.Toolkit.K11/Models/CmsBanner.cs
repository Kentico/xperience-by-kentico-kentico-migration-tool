using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CMS_Banner")]
[Index("BannerCategoryId", Name = "IX_CMS_Banner_BannerCategoryID")]
[Index("BannerSiteId", Name = "IX_CMS_Banner_BannerSiteID")]
public partial class CmsBanner
{
    [Key]
    [Column("BannerID")]
    public int BannerId { get; set; }

    [StringLength(256)]
    public string BannerName { get; set; } = null!;

    [StringLength(256)]
    public string BannerDisplayName { get; set; } = null!;

    [Column("BannerCategoryID")]
    public int BannerCategoryId { get; set; }

    [Required]
    public bool? BannerEnabled { get; set; }

    public DateTime? BannerFrom { get; set; }

    public DateTime? BannerTo { get; set; }

    public Guid BannerGuid { get; set; }

    public DateTime BannerLastModified { get; set; }

    public int BannerType { get; set; }

    [Column("BannerURL")]
    [StringLength(2083)]
    public string BannerUrl { get; set; } = null!;

    public bool BannerBlank { get; set; }

    public double BannerWeight { get; set; }

    public int? BannerHitsLeft { get; set; }

    public int? BannerClicksLeft { get; set; }

    [Column("BannerSiteID")]
    public int? BannerSiteId { get; set; }

    public string BannerContent { get; set; } = null!;

    [ForeignKey("BannerCategoryId")]
    [InverseProperty("CmsBanners")]
    public virtual CmsBannerCategory BannerCategory { get; set; } = null!;

    [ForeignKey("BannerSiteId")]
    [InverseProperty("CmsBanners")]
    public virtual CmsSite? BannerSite { get; set; }
}
