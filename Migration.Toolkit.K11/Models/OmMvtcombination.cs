using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("OM_MVTCombination")]
[Index("MvtcombinationPageTemplateId", Name = "IX_OM_MVTCombination_MVTCombinationPageTemplateID")]
public partial class OmMvtcombination
{
    [Key]
    [Column("MVTCombinationID")]
    public int MvtcombinationId { get; set; }

    [Column("MVTCombinationName")]
    [StringLength(200)]
    public string MvtcombinationName { get; set; } = null!;

    [Column("MVTCombinationCustomName")]
    [StringLength(200)]
    public string? MvtcombinationCustomName { get; set; }

    [Column("MVTCombinationPageTemplateID")]
    public int MvtcombinationPageTemplateId { get; set; }

    [Column("MVTCombinationEnabled")]
    public bool MvtcombinationEnabled { get; set; }

    [Column("MVTCombinationGUID")]
    public Guid MvtcombinationGuid { get; set; }

    [Column("MVTCombinationLastModified")]
    public DateTime MvtcombinationLastModified { get; set; }

    [Column("MVTCombinationIsDefault")]
    public bool? MvtcombinationIsDefault { get; set; }

    [Column("MVTCombinationConversions")]
    public int? MvtcombinationConversions { get; set; }

    [Column("MVTCombinationDocumentID")]
    public int? MvtcombinationDocumentId { get; set; }

    [ForeignKey("MvtcombinationId")]
    [InverseProperty("Mvtcombinations")]
    public virtual ICollection<OmMvtvariant> Mvtvariants { get; set; } = new List<OmMvtvariant>();
}
