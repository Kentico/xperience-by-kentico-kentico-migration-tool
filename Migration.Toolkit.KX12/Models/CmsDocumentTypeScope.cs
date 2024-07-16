using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("CMS_DocumentTypeScope")]
[Index("ScopeSiteId", Name = "IX_CMS_DocumentTypeScope_ScopeSiteID")]
public class CmsDocumentTypeScope
{
    [Key]
    [Column("ScopeID")]
    public int ScopeId { get; set; }

    public string ScopePath { get; set; } = null!;

    [Column("ScopeSiteID")]
    public int? ScopeSiteId { get; set; }

    public DateTime ScopeLastModified { get; set; }

    [Column("ScopeGUID")]
    public Guid? ScopeGuid { get; set; }

    public bool? ScopeIncludeChildren { get; set; }

    public bool? ScopeAllowAllTypes { get; set; }

    public bool? ScopeAllowLinks { get; set; }

    [Column("ScopeAllowABVariant")]
    public bool? ScopeAllowAbvariant { get; set; }

    public string? ScopeMacroCondition { get; set; }

    [ForeignKey("ScopeSiteId")]
    [InverseProperty("CmsDocumentTypeScopes")]
    public virtual CmsSite? ScopeSite { get; set; }

    [ForeignKey("ScopeId")]
    [InverseProperty("Scopes")]
    public virtual ICollection<CmsClass> Classes { get; set; } = new List<CmsClass>();
}
