using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("STORECONTENT_Shoes")]
public partial class StorecontentShoe
{
    [Key]
    [Column("ShoesID")]
    public int ShoesId { get; set; }

    [StringLength(100)]
    public string? ShoesColor { get; set; }

    [StringLength(100)]
    public string? ShoesStyle { get; set; }
}
