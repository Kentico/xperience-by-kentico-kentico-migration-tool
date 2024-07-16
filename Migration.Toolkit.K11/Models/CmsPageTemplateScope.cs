using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CMS_PageTemplateScope")]
[Index("PageTemplateScopeClassId", Name = "IX_CMS_PageTemplateScope_PageTemplateScopeClassID")]
[Index("PageTemplateScopeCultureId", Name = "IX_CMS_PageTemplateScope_PageTemplateScopeCultureID")]
[Index("PageTemplateScopeLevels", Name = "IX_CMS_PageTemplateScope_PageTemplateScopeLevels")]
[Index("PageTemplateScopeSiteId", Name = "IX_CMS_PageTemplateScope_PageTemplateScopeSiteID")]
[Index("PageTemplateScopeTemplateId", Name = "IX_CMS_PageTemplateScope_PageTemplateScopeTemplateID")]
public class CmsPageTemplateScope
{
    [Key]
    [Column("PageTemplateScopeID")]
    public int PageTemplateScopeId { get; set; }

    public string PageTemplateScopePath { get; set; } = null!;

    public string? PageTemplateScopeLevels { get; set; }

    [Column("PageTemplateScopeCultureID")]
    public int? PageTemplateScopeCultureId { get; set; }

    [Column("PageTemplateScopeClassID")]
    public int? PageTemplateScopeClassId { get; set; }

    [Column("PageTemplateScopeTemplateID")]
    public int PageTemplateScopeTemplateId { get; set; }

    [Column("PageTemplateScopeSiteID")]
    public int? PageTemplateScopeSiteId { get; set; }

    public DateTime PageTemplateScopeLastModified { get; set; }

    [Column("PageTemplateScopeGUID")]
    public Guid PageTemplateScopeGuid { get; set; }

    [ForeignKey("PageTemplateScopeClassId")]
    [InverseProperty("CmsPageTemplateScopes")]
    public virtual CmsClass? PageTemplateScopeClass { get; set; }

    [ForeignKey("PageTemplateScopeCultureId")]
    [InverseProperty("CmsPageTemplateScopes")]
    public virtual CmsCulture? PageTemplateScopeCulture { get; set; }

    [ForeignKey("PageTemplateScopeSiteId")]
    [InverseProperty("CmsPageTemplateScopes")]
    public virtual CmsSite? PageTemplateScopeSite { get; set; }

    [ForeignKey("PageTemplateScopeTemplateId")]
    [InverseProperty("CmsPageTemplateScopes")]
    public virtual CmsPageTemplate PageTemplateScopeTemplate { get; set; } = null!;
}
