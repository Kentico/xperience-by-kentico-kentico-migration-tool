using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("STORECONTENT_Watch")]
public class StorecontentWatch
{
    [Key]
    [Column("WatchID")]
    public int WatchId { get; set; }

    [StringLength(100)]
    public string? WatchGender { get; set; }

    [StringLength(100)]
    public string? WatchDisplay { get; set; }

    [StringLength(100)]
    public string? WatchDialColour { get; set; }

    [StringLength(100)]
    public string? WatchStrapMaterial { get; set; }

    [StringLength(100)]
    public string? WatchWaterResistance { get; set; }
}
