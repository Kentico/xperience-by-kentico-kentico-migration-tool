using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("OM_MVTVariant")]
[Index("MvtvariantPageTemplateId", Name = "IX_OM_MVTVariant_MVTVariantPageTemplateID")]
public class OmMvtvariant
{
    [Key]
    [Column("MVTVariantID")]
    public int MvtvariantId { get; set; }

    [Column("MVTVariantName")]
    [StringLength(100)]
    public string MvtvariantName { get; set; } = null!;

    [Column("MVTVariantDisplayName")]
    [StringLength(200)]
    public string MvtvariantDisplayName { get; set; } = null!;

    [Column("MVTVariantInstanceGUID")]
    public Guid? MvtvariantInstanceGuid { get; set; }

    [Column("MVTVariantZoneID")]
    [StringLength(200)]
    public string? MvtvariantZoneId { get; set; }

    [Column("MVTVariantPageTemplateID")]
    public int MvtvariantPageTemplateId { get; set; }

    [Required]
    [Column("MVTVariantEnabled")]
    public bool? MvtvariantEnabled { get; set; }

    [Column("MVTVariantWebParts")]
    public string? MvtvariantWebParts { get; set; }

    [Column("MVTVariantGUID")]
    public Guid MvtvariantGuid { get; set; }

    [Column("MVTVariantLastModified")]
    public DateTime MvtvariantLastModified { get; set; }

    [Column("MVTVariantDescription")]
    public string? MvtvariantDescription { get; set; }

    [Column("MVTVariantDocumentID")]
    public int? MvtvariantDocumentId { get; set; }

    [ForeignKey("MvtvariantPageTemplateId")]
    [InverseProperty("OmMvtvariants")]
    public virtual CmsPageTemplate MvtvariantPageTemplate { get; set; } = null!;

    [ForeignKey("MvtvariantId")]
    [InverseProperty("Mvtvariants")]
    public virtual ICollection<OmMvtcombination> Mvtcombinations { get; set; } = new List<OmMvtcombination>();
}
