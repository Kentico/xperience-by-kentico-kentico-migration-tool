using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("DancingGoat_Grinder")]
public partial class DancingGoatGrinder
{
    [Key]
    [Column("GrinderID")]
    public int GrinderId { get; set; }

    [StringLength(200)]
    public string? GrinderPromotionTitle { get; set; }

    [StringLength(200)]
    public string? GrinderPromotionDescription { get; set; }

    [StringLength(200)]
    public string? GrinderBannerText { get; set; }
}
