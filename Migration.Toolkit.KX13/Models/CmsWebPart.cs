using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_WebPart")]
[Index("WebPartCategoryId", Name = "IX_CMS_WebPart_WebPartCategoryID")]
[Index("WebPartName", Name = "IX_CMS_WebPart_WebPartName")]
[Index("WebPartParentId", Name = "IX_CMS_WebPart_WebPartParentID")]
[Index("WebPartResourceId", Name = "IX_CMS_WebPart_WebPartResourceID")]
public partial class CmsWebPart
{
    [Key]
    [Column("WebPartID")]
    public int WebPartId { get; set; }

    [StringLength(100)]
    public string WebPartName { get; set; } = null!;

    [StringLength(100)]
    public string WebPartDisplayName { get; set; } = null!;

    public string? WebPartDescription { get; set; }

    [StringLength(100)]
    public string WebPartFileName { get; set; } = null!;

    public string WebPartProperties { get; set; } = null!;

    [Column("WebPartCategoryID")]
    public int WebPartCategoryId { get; set; }

    [Column("WebPartParentID")]
    public int? WebPartParentId { get; set; }

    public string? WebPartDocumentation { get; set; }

    [Column("WebPartGUID")]
    public Guid WebPartGuid { get; set; }

    public DateTime WebPartLastModified { get; set; }

    public int? WebPartType { get; set; }

    public string? WebPartDefaultValues { get; set; }

    [Column("WebPartResourceID")]
    public int? WebPartResourceId { get; set; }

    [Column("WebPartCSS")]
    public string? WebPartCss { get; set; }

    public bool? WebPartSkipInsertProperties { get; set; }

    [Column("WebPartThumbnailGUID")]
    public Guid? WebPartThumbnailGuid { get; set; }

    [StringLength(200)]
    public string? WebPartIconClass { get; set; }

    [InverseProperty("WebPartLayoutWebPart")]
    public virtual ICollection<CmsWebPartLayout> CmsWebPartLayouts { get; set; } = new List<CmsWebPartLayout>();

    [InverseProperty("WidgetWebPart")]
    public virtual ICollection<CmsWidget> CmsWidgets { get; set; } = new List<CmsWidget>();

    [InverseProperty("WebPartParent")]
    public virtual ICollection<CmsWebPart> InverseWebPartParent { get; set; } = new List<CmsWebPart>();

    [ForeignKey("WebPartCategoryId")]
    [InverseProperty("CmsWebParts")]
    public virtual CmsWebPartCategory WebPartCategory { get; set; } = null!;

    [ForeignKey("WebPartParentId")]
    [InverseProperty("InverseWebPartParent")]
    public virtual CmsWebPart? WebPartParent { get; set; }

    [ForeignKey("WebPartResourceId")]
    [InverseProperty("CmsWebParts")]
    public virtual CmsResource? WebPartResource { get; set; }
}
