using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CMS_TemplateDeviceLayout")]
[Index("LayoutId", Name = "IX_CMS_TemplateDeviceLayout_LayoutID")]
[Index("PageTemplateId", "ProfileId", Name = "IX_CMS_TemplateDeviceLayout_PageTemplateID_ProfileID", IsUnique = true)]
[Index("ProfileId", Name = "IX_CMS_TemplateDeviceLayout_ProfileID")]
public partial class CmsTemplateDeviceLayout
{
    [Key]
    [Column("TemplateDeviceLayoutID")]
    public int TemplateDeviceLayoutId { get; set; }

    [Column("PageTemplateID")]
    public int PageTemplateId { get; set; }

    [Column("ProfileID")]
    public int ProfileId { get; set; }

    [Column("LayoutID")]
    public int? LayoutId { get; set; }

    public string? LayoutCode { get; set; }

    [StringLength(50)]
    public string? LayoutType { get; set; }

    [Column("LayoutCSS")]
    public string? LayoutCss { get; set; }

    public DateTime LayoutLastModified { get; set; }

    [Column("LayoutGUID")]
    public Guid LayoutGuid { get; set; }

    [Column("LayoutVersionGUID")]
    [StringLength(50)]
    public string? LayoutVersionGuid { get; set; }

    [ForeignKey("LayoutId")]
    [InverseProperty("CmsTemplateDeviceLayouts")]
    public virtual CmsLayout? Layout { get; set; }

    [ForeignKey("PageTemplateId")]
    [InverseProperty("CmsTemplateDeviceLayouts")]
    public virtual CmsPageTemplate PageTemplate { get; set; } = null!;

    [ForeignKey("ProfileId")]
    [InverseProperty("CmsTemplateDeviceLayouts")]
    public virtual CmsDeviceProfile Profile { get; set; } = null!;
}