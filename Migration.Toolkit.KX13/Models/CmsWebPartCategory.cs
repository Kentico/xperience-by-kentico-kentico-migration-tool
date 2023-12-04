using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_WebPartCategory")]
[Index("CategoryParentId", Name = "IX_CMS_WebPartCategory_CategoryParentID")]
public partial class CmsWebPartCategory
{
    [Key]
    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [StringLength(100)]
    public string CategoryDisplayName { get; set; } = null!;

    [Column("CategoryParentID")]
    public int? CategoryParentId { get; set; }

    [StringLength(100)]
    public string CategoryName { get; set; } = null!;

    [Column("CategoryGUID")]
    public Guid CategoryGuid { get; set; }

    public DateTime CategoryLastModified { get; set; }

    [StringLength(450)]
    public string? CategoryImagePath { get; set; }

    public string CategoryPath { get; set; } = null!;

    public int? CategoryLevel { get; set; }

    public int? CategoryChildCount { get; set; }

    public int? CategoryWebPartChildCount { get; set; }

    [ForeignKey("CategoryParentId")]
    [InverseProperty("InverseCategoryParent")]
    public virtual CmsWebPartCategory? CategoryParent { get; set; }

    [InverseProperty("WebPartCategory")]
    public virtual ICollection<CmsWebPart> CmsWebParts { get; set; } = new List<CmsWebPart>();

    [InverseProperty("CategoryParent")]
    public virtual ICollection<CmsWebPartCategory> InverseCategoryParent { get; set; } = new List<CmsWebPartCategory>();
}
