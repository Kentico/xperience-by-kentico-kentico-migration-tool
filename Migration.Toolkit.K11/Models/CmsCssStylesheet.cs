using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CMS_CssStylesheet")]
[Index("StylesheetName", Name = "IX_CMS_CssStylesheet_StylesheetName")]
public partial class CmsCssStylesheet
{
    [Key]
    [Column("StylesheetID")]
    public int StylesheetId { get; set; }

    [StringLength(200)]
    public string StylesheetDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string StylesheetName { get; set; } = null!;

    public string? StylesheetText { get; set; }

    [Column("StylesheetVersionGUID")]
    public Guid? StylesheetVersionGuid { get; set; }

    [Column("StylesheetGUID")]
    public Guid? StylesheetGuid { get; set; }

    public DateTime StylesheetLastModified { get; set; }

    public string? StylesheetDynamicCode { get; set; }

    [StringLength(200)]
    public string? StylesheetDynamicLanguage { get; set; }

    [InverseProperty("DocumentStylesheet")]
    public virtual ICollection<CmsDocument> CmsDocuments { get; set; } = new List<CmsDocument>();

    [InverseProperty("SiteDefaultEditorStylesheetNavigation")]
    public virtual ICollection<CmsSite> CmsSiteSiteDefaultEditorStylesheetNavigations { get; set; } = new List<CmsSite>();

    [InverseProperty("SiteDefaultStylesheet")]
    public virtual ICollection<CmsSite> CmsSiteSiteDefaultStylesheets { get; set; } = new List<CmsSite>();

    [ForeignKey("StylesheetId")]
    [InverseProperty("Stylesheets")]
    public virtual ICollection<CmsSite> Sites { get; set; } = new List<CmsSite>();
}
