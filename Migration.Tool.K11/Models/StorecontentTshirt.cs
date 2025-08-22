using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("STORECONTENT_Tshirt")]
public class StorecontentTshirt
{
    [Key]
    [Column("TshirtID")]
    public int TshirtId { get; set; }

    [StringLength(100)]
    public string? TshirtColor { get; set; }

    [StringLength(100)]
    public string? TshirtStyle { get; set; }
}
