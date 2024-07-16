using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CMS_DocumentAlias")]
[Index("AliasNodeId", Name = "IX_CMS_DocumentAlias_AliasNodeID")]
[Index("AliasSiteId", Name = "IX_CMS_DocumentAlias_AliasSiteID")]
[Index("AliasWildcardRule", "AliasPriority", Name = "IX_CMS_DocumentAlias_AliasWildcardRule_AliasPriority")]
[Index("AliasCulture", Name = "IX_CMS_Document_AliasCulture")]
public partial class CmsDocumentAlias
{
    [Key]
    [Column("AliasID")]
    public int AliasId { get; set; }

    [Column("AliasNodeID")]
    public int AliasNodeId { get; set; }

    [StringLength(20)]
    public string? AliasCulture { get; set; }

    [Column("AliasURLPath")]
    public string? AliasUrlpath { get; set; }

    [StringLength(100)]
    public string? AliasExtensions { get; set; }

    [StringLength(440)]
    public string? AliasWildcardRule { get; set; }

    public int? AliasPriority { get; set; }

    [Column("AliasGUID")]
    public Guid? AliasGuid { get; set; }

    public DateTime AliasLastModified { get; set; }

    [Column("AliasSiteID")]
    public int AliasSiteId { get; set; }

    [StringLength(50)]
    public string? AliasActionMode { get; set; }

    [ForeignKey("AliasNodeId")]
    [InverseProperty("CmsDocumentAliases")]
    public virtual CmsTree AliasNode { get; set; } = null!;

    [ForeignKey("AliasSiteId")]
    [InverseProperty("CmsDocumentAliases")]
    public virtual CmsSite AliasSite { get; set; } = null!;
}
