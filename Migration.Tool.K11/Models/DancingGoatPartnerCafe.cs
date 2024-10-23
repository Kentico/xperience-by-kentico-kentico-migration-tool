using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("DancingGoat_PartnerCafe")]
public class DancingGoatPartnerCafe
{
    [Key]
    [Column("PartnerCafeID")]
    public int PartnerCafeId { get; set; }

    [StringLength(80)]
    public string PartnerCafeName { get; set; } = null!;

    [StringLength(50)]
    public string PartnerCafeStreet { get; set; } = null!;

    [StringLength(15)]
    public string PartnerCafeZipCode { get; set; } = null!;

    [StringLength(20)]
    public string? PartnerCafePhone { get; set; }
}
