using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("OM_ABVariant")]
[Index("AbvariantSiteId", Name = "IX_OM_ABVariant_ABVariantSiteID")]
[Index("AbvariantTestId", Name = "IX_OM_ABVariant_ABVariantTestID")]
public partial class OmAbvariant
{
    [Key]
    [Column("ABVariantID")]
    public int AbvariantId { get; set; }

    [Column("ABVariantDisplayName")]
    [StringLength(110)]
    public string AbvariantDisplayName { get; set; } = null!;

    [Column("ABVariantName")]
    [StringLength(50)]
    public string AbvariantName { get; set; } = null!;

    [Column("ABVariantTestID")]
    public int AbvariantTestId { get; set; }

    [Column("ABVariantPath")]
    [StringLength(450)]
    public string AbvariantPath { get; set; } = null!;

    [Column("ABVariantGUID")]
    public Guid AbvariantGuid { get; set; }

    [Column("ABVariantLastModified")]
    public DateTime AbvariantLastModified { get; set; }

    [Column("ABVariantSiteID")]
    public int AbvariantSiteId { get; set; }

    [ForeignKey("AbvariantSiteId")]
    [InverseProperty("OmAbvariants")]
    public virtual CmsSite AbvariantSite { get; set; } = null!;

    [ForeignKey("AbvariantTestId")]
    [InverseProperty("OmAbvariants")]
    public virtual OmAbtest AbvariantTest { get; set; } = null!;
}
