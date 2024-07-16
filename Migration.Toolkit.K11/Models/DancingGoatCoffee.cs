using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("DancingGoat_Coffee")]
public partial class DancingGoatCoffee
{
    [Key]
    [Column("CoffeeID")]
    public int CoffeeId { get; set; }

    [StringLength(100)]
    public string? CoffeeFarm { get; set; }

    [StringLength(100)]
    public string? CoffeeCountry { get; set; }

    [StringLength(40)]
    public string CoffeeVariety { get; set; } = null!;

    [StringLength(20)]
    public string CoffeeProcessing { get; set; } = null!;

    public int? CoffeeAltitude { get; set; }

    [StringLength(200)]
    public string? CoffeePromotionTitle { get; set; }

    [StringLength(200)]
    public string? CoffeePromotionDescription { get; set; }

    [StringLength(200)]
    public string? CoffeeBannerText { get; set; }
}
