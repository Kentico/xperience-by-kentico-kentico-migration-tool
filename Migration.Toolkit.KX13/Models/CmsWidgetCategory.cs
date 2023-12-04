using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_WidgetCategory")]
[Index("WidgetCategoryParentId", Name = "IX_CMS_WidgetCategory_WidgetCategoryParentID")]
public partial class CmsWidgetCategory
{
    [Key]
    [Column("WidgetCategoryID")]
    public int WidgetCategoryId { get; set; }

    [StringLength(100)]
    public string WidgetCategoryName { get; set; } = null!;

    [StringLength(100)]
    public string WidgetCategoryDisplayName { get; set; } = null!;

    [Column("WidgetCategoryParentID")]
    public int? WidgetCategoryParentId { get; set; }

    public string WidgetCategoryPath { get; set; } = null!;

    public int WidgetCategoryLevel { get; set; }

    public int? WidgetCategoryChildCount { get; set; }

    public int? WidgetCategoryWidgetChildCount { get; set; }

    [StringLength(450)]
    public string? WidgetCategoryImagePath { get; set; }

    [Column("WidgetCategoryGUID")]
    public Guid WidgetCategoryGuid { get; set; }

    public DateTime WidgetCategoryLastModified { get; set; }

    [InverseProperty("WidgetCategory")]
    public virtual ICollection<CmsWidget> CmsWidgets { get; set; } = new List<CmsWidget>();

    [InverseProperty("WidgetCategoryParent")]
    public virtual ICollection<CmsWidgetCategory> InverseWidgetCategoryParent { get; set; } = new List<CmsWidgetCategory>();

    [ForeignKey("WidgetCategoryParentId")]
    [InverseProperty("InverseWidgetCategoryParent")]
    public virtual CmsWidgetCategory? WidgetCategoryParent { get; set; }
}
