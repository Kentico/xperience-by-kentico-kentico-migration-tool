using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("OM_PersonalizationVariant")]
[Index("VariantPageTemplateId", Name = "IX_OM_PersonalizationVariant_VariantDocumentID")]
[Index("VariantDocumentId", Name = "IX_OM_PersonalizationVariant_VariantPageTemplateID")]
public partial class OmPersonalizationVariant
{
    [Key]
    [Column("VariantID")]
    public int VariantId { get; set; }

    [Required]
    public bool? VariantEnabled { get; set; }

    [StringLength(200)]
    public string VariantName { get; set; } = null!;

    [StringLength(200)]
    public string VariantDisplayName { get; set; } = null!;

    [Column("VariantInstanceGUID")]
    public Guid? VariantInstanceGuid { get; set; }

    [Column("VariantZoneID")]
    [StringLength(200)]
    public string? VariantZoneId { get; set; }

    [Column("VariantPageTemplateID")]
    public int VariantPageTemplateId { get; set; }

    public string VariantWebParts { get; set; } = null!;

    public int? VariantPosition { get; set; }

    [Column("VariantGUID")]
    public Guid VariantGuid { get; set; }

    public DateTime VariantLastModified { get; set; }

    public string? VariantDescription { get; set; }

    [Column("VariantDocumentID")]
    public int? VariantDocumentId { get; set; }

    public string VariantDisplayCondition { get; set; } = null!;

    [ForeignKey("VariantDocumentId")]
    [InverseProperty("OmPersonalizationVariants")]
    public virtual CmsDocument? VariantDocument { get; set; }

    [ForeignKey("VariantPageTemplateId")]
    [InverseProperty("OmPersonalizationVariants")]
    public virtual CmsPageTemplate VariantPageTemplate { get; set; } = null!;
}
