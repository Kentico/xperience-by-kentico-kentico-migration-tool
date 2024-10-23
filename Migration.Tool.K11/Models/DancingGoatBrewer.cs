using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("DancingGoat_Brewer")]
public class DancingGoatBrewer
{
    [Key]
    [Column("BrewerID")]
    public int BrewerId { get; set; }

    [StringLength(200)]
    public string? BrewerPromotionTitle { get; set; }

    [StringLength(200)]
    public string? BrewerPromotionDescription { get; set; }

    [StringLength(200)]
    public string? BrewerBannerText { get; set; }
}
