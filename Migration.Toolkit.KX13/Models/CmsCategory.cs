using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_Category")]
[Index("CategorySiteId", Name = "IX_CMS_Category_CategorySiteID")]
[Index("CategoryUserId", Name = "IX_CMS_Category_CategoryUserID")]
public partial class CmsCategory
{
    [Key]
    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [StringLength(250)]
    public string CategoryDisplayName { get; set; } = null!;

    [StringLength(250)]
    public string? CategoryName { get; set; }

    public string? CategoryDescription { get; set; }

    public int? CategoryCount { get; set; }

    [Required]
    public bool? CategoryEnabled { get; set; }

    [Column("CategoryUserID")]
    public int? CategoryUserId { get; set; }

    [Column("CategoryGUID")]
    public Guid CategoryGuid { get; set; }

    public DateTime CategoryLastModified { get; set; }

    [Column("CategorySiteID")]
    public int? CategorySiteId { get; set; }

    [Column("CategoryParentID")]
    public int? CategoryParentId { get; set; }

    [Column("CategoryIDPath")]
    [StringLength(450)]
    public string? CategoryIdpath { get; set; }

    [StringLength(1500)]
    public string? CategoryNamePath { get; set; }

    public int? CategoryLevel { get; set; }

    public int? CategoryOrder { get; set; }

    [ForeignKey("CategorySiteId")]
    [InverseProperty("CmsCategories")]
    public virtual CmsSite? CategorySite { get; set; }

    [ForeignKey("CategoryUserId")]
    [InverseProperty("CmsCategories")]
    public virtual CmsUser? CategoryUser { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Categories")]
    public virtual ICollection<CmsDocument> Documents { get; set; } = new List<CmsDocument>();
}
