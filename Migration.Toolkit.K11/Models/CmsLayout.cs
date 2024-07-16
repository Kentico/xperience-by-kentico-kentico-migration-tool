using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CMS_Layout")]
[Index("LayoutDisplayName", Name = "IX_CMS_Layout_LayoutDisplayName")]
public partial class CmsLayout
{
    [Key]
    [Column("LayoutID")]
    public int LayoutId { get; set; }

    [StringLength(100)]
    public string LayoutCodeName { get; set; } = null!;

    [StringLength(200)]
    public string LayoutDisplayName { get; set; } = null!;

    public string? LayoutDescription { get; set; }

    public string LayoutCode { get; set; } = null!;

    [Column("LayoutVersionGUID")]
    [StringLength(50)]
    public string? LayoutVersionGuid { get; set; }

    [Column("LayoutGUID")]
    public Guid LayoutGuid { get; set; }

    public DateTime LayoutLastModified { get; set; }

    [StringLength(50)]
    public string? LayoutType { get; set; }

    [Column("LayoutCSS")]
    public string? LayoutCss { get; set; }

    [Column("LayoutThumbnailGUID")]
    public Guid? LayoutThumbnailGuid { get; set; }

    public int? LayoutZoneCount { get; set; }

    public bool? LayoutIsConvertible { get; set; }

    [StringLength(200)]
    public string? LayoutIconClass { get; set; }

    [InverseProperty("SourceLayout")]
    public virtual ICollection<CmsDeviceProfileLayout> CmsDeviceProfileLayoutSourceLayouts { get; set; } = new List<CmsDeviceProfileLayout>();

    [InverseProperty("TargetLayout")]
    public virtual ICollection<CmsDeviceProfileLayout> CmsDeviceProfileLayoutTargetLayouts { get; set; } = new List<CmsDeviceProfileLayout>();

    [InverseProperty("PageTemplateLayoutNavigation")]
    public virtual ICollection<CmsPageTemplate> CmsPageTemplates { get; set; } = new List<CmsPageTemplate>();

    [InverseProperty("Layout")]
    public virtual ICollection<CmsTemplateDeviceLayout> CmsTemplateDeviceLayouts { get; set; } = new List<CmsTemplateDeviceLayout>();
}