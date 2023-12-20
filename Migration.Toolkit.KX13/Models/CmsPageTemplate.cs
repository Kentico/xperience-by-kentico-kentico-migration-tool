using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_PageTemplate")]
[Index("PageTemplateCodeName", "PageTemplateDisplayName", Name = "IX_CMS_PageTemplate_PageTemplateCodeName_PageTemplateDisplayName")]
[Index("PageTemplateLayoutId", Name = "IX_CMS_PageTemplate_PageTemplateLayoutID")]
public partial class CmsPageTemplate
{
    [Key]
    [Column("PageTemplateID")]
    public int PageTemplateId { get; set; }

    [StringLength(200)]
    public string PageTemplateDisplayName { get; set; } = null!;

    [StringLength(100)]
    public string PageTemplateCodeName { get; set; } = null!;

    public string? PageTemplateDescription { get; set; }

    [Column("PageTemplateCategoryID")]
    public int? PageTemplateCategoryId { get; set; }

    [Column("PageTemplateLayoutID")]
    public int? PageTemplateLayoutId { get; set; }

    public string? PageTemplateWebParts { get; set; }

    public string? PageTemplateLayout { get; set; }

    [Column("PageTemplateVersionGUID")]
    [StringLength(200)]
    public string? PageTemplateVersionGuid { get; set; }

    [Column("PageTemplateGUID")]
    public Guid PageTemplateGuid { get; set; }

    public DateTime PageTemplateLastModified { get; set; }

    [StringLength(10)]
    public string PageTemplateType { get; set; } = null!;

    [StringLength(50)]
    public string? PageTemplateLayoutType { get; set; }

    [Column("PageTemplateCSS")]
    public string? PageTemplateCss { get; set; }

    [Column("PageTemplateThumbnailGUID")]
    public Guid? PageTemplateThumbnailGuid { get; set; }

    public string? PageTemplateProperties { get; set; }

    public bool? PageTemplateIsLayout { get; set; }

    [StringLength(200)]
    public string? PageTemplateIconClass { get; set; }

    [InverseProperty("ElementPageTemplate")]
    public virtual ICollection<CmsUielement> CmsUielements { get; set; } = new List<CmsUielement>();

    [ForeignKey("PageTemplateCategoryId")]
    [InverseProperty("CmsPageTemplates")]
    public virtual CmsPageTemplateCategory? PageTemplateCategory { get; set; }

    [ForeignKey("PageTemplateLayoutId")]
    [InverseProperty("CmsPageTemplates")]
    public virtual CmsLayout? PageTemplateLayoutNavigation { get; set; }
}
