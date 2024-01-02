using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_WebPartLayout")]
[Index("WebPartLayoutWebPartId", Name = "IX_CMS_WebPartLayout_WebPartLayoutWebPartID")]
public partial class CmsWebPartLayout
{
    [Key]
    [Column("WebPartLayoutID")]
    public int WebPartLayoutId { get; set; }

    [StringLength(200)]
    public string WebPartLayoutCodeName { get; set; } = null!;

    [StringLength(200)]
    public string WebPartLayoutDisplayName { get; set; } = null!;

    public string? WebPartLayoutDescription { get; set; }

    public string? WebPartLayoutCode { get; set; }

    [Column("WebPartLayoutVersionGUID")]
    [StringLength(100)]
    public string? WebPartLayoutVersionGuid { get; set; }

    [Column("WebPartLayoutWebPartID")]
    public int WebPartLayoutWebPartId { get; set; }

    [Column("WebPartLayoutGUID")]
    public Guid WebPartLayoutGuid { get; set; }

    public DateTime WebPartLayoutLastModified { get; set; }

    [Column("WebPartLayoutCSS")]
    public string? WebPartLayoutCss { get; set; }

    public bool? WebPartLayoutIsDefault { get; set; }

    [InverseProperty("WidgetLayout")]
    public virtual ICollection<CmsWidget> CmsWidgets { get; set; } = new List<CmsWidget>();

    [ForeignKey("WebPartLayoutWebPartId")]
    [InverseProperty("CmsWebPartLayouts")]
    public virtual CmsWebPart WebPartLayoutWebPart { get; set; } = null!;
}
