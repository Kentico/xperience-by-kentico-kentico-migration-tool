using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CMS_PageTemplate")]
[Index("PageTemplateCodeName", "PageTemplateDisplayName", Name = "IX_CMS_PageTemplate_PageTemplateCodeName_PageTemplateDisplayName")]
[Index("PageTemplateIsReusable", "PageTemplateForAllPages", "PageTemplateShowAsMasterTemplate", Name = "IX_CMS_PageTemplate_PageTemplateIsReusable_PageTemplateForAllPages_PageTemplateShowAsMasterTemplate")]
[Index("PageTemplateLayoutId", Name = "IX_CMS_PageTemplate_PageTemplateLayoutID")]
[Index("PageTemplateSiteId", "PageTemplateCodeName", "PageTemplateGuid", Name = "IX_CMS_PageTemplate_PageTemplateSiteID_PageTemplateCodeName_PageTemplateGUID")]
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

    [StringLength(400)]
    public string? PageTemplateFile { get; set; }

    [Column("PageTemplateCategoryID")]
    public int? PageTemplateCategoryId { get; set; }

    [Column("PageTemplateLayoutID")]
    public int? PageTemplateLayoutId { get; set; }

    public string? PageTemplateWebParts { get; set; }

    public bool? PageTemplateIsReusable { get; set; }

    public bool? PageTemplateShowAsMasterTemplate { get; set; }

    [StringLength(200)]
    public string? PageTemplateInheritPageLevels { get; set; }

    public string? PageTemplateLayout { get; set; }

    [Column("PageTemplateVersionGUID")]
    [StringLength(200)]
    public string? PageTemplateVersionGuid { get; set; }

    public string? PageTemplateHeader { get; set; }

    [Column("PageTemplateGUID")]
    public Guid PageTemplateGuid { get; set; }

    public DateTime PageTemplateLastModified { get; set; }

    [Column("PageTemplateSiteID")]
    public int? PageTemplateSiteId { get; set; }

    public bool? PageTemplateForAllPages { get; set; }

    [StringLength(10)]
    public string PageTemplateType { get; set; } = null!;

    [StringLength(50)]
    public string? PageTemplateLayoutType { get; set; }

    [Column("PageTemplateCSS")]
    public string? PageTemplateCss { get; set; }

    public bool? PageTemplateIsAllowedForProductSection { get; set; }

    public bool? PageTemplateInheritParentHeader { get; set; }

    public bool? PageTemplateAllowInheritHeader { get; set; }

    [Column("PageTemplateThumbnailGUID")]
    public Guid? PageTemplateThumbnailGuid { get; set; }

    public bool? PageTemplateCloneAsAdHoc { get; set; }

    [StringLength(200)]
    public string? PageTemplateDefaultController { get; set; }

    [StringLength(200)]
    public string? PageTemplateDefaultAction { get; set; }

    [Column("PageTemplateNodeGUID")]
    public Guid? PageTemplateNodeGuid { get; set; }

    [Column("PageTemplateMasterPageTemplateID")]
    public int? PageTemplateMasterPageTemplateId { get; set; }

    public string? PageTemplateProperties { get; set; }

    public bool? PageTemplateIsLayout { get; set; }

    [StringLength(200)]
    public string? PageTemplateIconClass { get; set; }

    [InverseProperty("ClassDefaultPageTemplate")]
    public virtual ICollection<CmsClass> CmsClasses { get; set; } = new List<CmsClass>();

    [InverseProperty("DocumentPageTemplate")]
    public virtual ICollection<CmsDocument> CmsDocuments { get; set; } = new List<CmsDocument>();

    [InverseProperty("PageTemplateScopeTemplate")]
    public virtual ICollection<CmsPageTemplateScope> CmsPageTemplateScopes { get; set; } = new List<CmsPageTemplateScope>();

    [InverseProperty("PageTemplate")]
    public virtual ICollection<CmsTemplateDeviceLayout> CmsTemplateDeviceLayouts { get; set; } = new List<CmsTemplateDeviceLayout>();

    [InverseProperty("NodeTemplate")]
    public virtual ICollection<CmsTree> CmsTrees { get; set; } = new List<CmsTree>();

    [InverseProperty("ElementPageTemplate")]
    public virtual ICollection<CmsUielement> CmsUielements { get; set; } = new List<CmsUielement>();

    [InverseProperty("MvtvariantPageTemplate")]
    public virtual ICollection<OmMvtvariant> OmMvtvariants { get; set; } = new List<OmMvtvariant>();

    [InverseProperty("VariantPageTemplate")]
    public virtual ICollection<OmPersonalizationVariant> OmPersonalizationVariants { get; set; } = new List<OmPersonalizationVariant>();

    [ForeignKey("PageTemplateCategoryId")]
    [InverseProperty("CmsPageTemplates")]
    public virtual CmsPageTemplateCategory? PageTemplateCategory { get; set; }

    [ForeignKey("PageTemplateLayoutId")]
    [InverseProperty("CmsPageTemplates")]
    public virtual CmsLayout? PageTemplateLayoutNavigation { get; set; }

    [ForeignKey("PageTemplateSiteId")]
    [InverseProperty("CmsPageTemplates")]
    public virtual CmsSite? PageTemplateSite { get; set; }

    [ForeignKey("PageTemplateId")]
    [InverseProperty("PageTemplates")]
    public virtual ICollection<CmsSite> Sites { get; set; } = new List<CmsSite>();
}
