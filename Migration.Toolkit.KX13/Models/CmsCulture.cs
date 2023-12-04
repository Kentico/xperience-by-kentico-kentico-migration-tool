using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_Culture")]
[Index("CultureAlias", Name = "IX_CMS_CulturAlias")]
[Index("CultureCode", Name = "IX_CMS_Culture_CultureCode")]
public partial class CmsCulture
{
    [Key]
    [Column("CultureID")]
    public int CultureId { get; set; }

    [StringLength(200)]
    public string CultureName { get; set; } = null!;

    [StringLength(50)]
    public string CultureCode { get; set; } = null!;

    [StringLength(200)]
    public string CultureShortName { get; set; } = null!;

    [Column("CultureGUID")]
    public Guid CultureGuid { get; set; }

    public DateTime CultureLastModified { get; set; }

    [StringLength(100)]
    public string? CultureAlias { get; set; }

    [Column("CultureIsUICulture")]
    public bool? CultureIsUiculture { get; set; }

    [InverseProperty("TranslationCulture")]
    public virtual ICollection<CmsResourceTranslation> CmsResourceTranslations { get; set; } = new List<CmsResourceTranslation>();

    [InverseProperty("Culture")]
    public virtual ICollection<CmsUserCulture> CmsUserCultures { get; set; } = new List<CmsUserCulture>();

    [InverseProperty("ScopeCulture")]
    public virtual ICollection<CmsWorkflowScope> CmsWorkflowScopes { get; set; } = new List<CmsWorkflowScope>();

    [ForeignKey("IndexCultureId")]
    [InverseProperty("IndexCultures")]
    public virtual ICollection<CmsSearchIndex> Indices { get; set; } = new List<CmsSearchIndex>();

    [ForeignKey("CultureId")]
    [InverseProperty("Cultures")]
    public virtual ICollection<CmsSite> Sites { get; set; } = new List<CmsSite>();
}
