using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_Widget")]
[Index("WidgetCategoryId", Name = "IX_CMS_Widget_WidgetCategoryID")]
[Index("WidgetLayoutId", Name = "IX_CMS_Widget_WidgetLayoutID")]
[Index("WidgetWebPartId", Name = "IX_CMS_Widget_WidgetWebPartID")]
public partial class CmsWidget
{
    [Key]
    [Column("WidgetID")]
    public int WidgetId { get; set; }

    [Column("WidgetWebPartID")]
    public int WidgetWebPartId { get; set; }

    [StringLength(100)]
    public string WidgetDisplayName { get; set; } = null!;

    [StringLength(100)]
    public string WidgetName { get; set; } = null!;

    public string? WidgetDescription { get; set; }

    [Column("WidgetCategoryID")]
    public int WidgetCategoryId { get; set; }

    public string? WidgetProperties { get; set; }

    public int WidgetSecurity { get; set; }

    [Column("WidgetGUID")]
    public Guid WidgetGuid { get; set; }

    public DateTime WidgetLastModified { get; set; }

    public bool WidgetIsEnabled { get; set; }

    public string? WidgetDocumentation { get; set; }

    public string? WidgetDefaultValues { get; set; }

    [Column("WidgetLayoutID")]
    public int? WidgetLayoutId { get; set; }

    public bool? WidgetSkipInsertProperties { get; set; }

    [Column("WidgetThumbnailGUID")]
    public Guid? WidgetThumbnailGuid { get; set; }

    [StringLength(200)]
    public string? WidgetIconClass { get; set; }

    [InverseProperty("Widget")]
    public virtual ICollection<CmsWidgetRole> CmsWidgetRoles { get; set; } = new List<CmsWidgetRole>();

    [ForeignKey("WidgetCategoryId")]
    [InverseProperty("CmsWidgets")]
    public virtual CmsWidgetCategory WidgetCategory { get; set; } = null!;

    [ForeignKey("WidgetLayoutId")]
    [InverseProperty("CmsWidgets")]
    public virtual CmsWebPartLayout? WidgetLayout { get; set; }

    [ForeignKey("WidgetWebPartId")]
    [InverseProperty("CmsWidgets")]
    public virtual CmsWebPart WidgetWebPart { get; set; } = null!;
}
