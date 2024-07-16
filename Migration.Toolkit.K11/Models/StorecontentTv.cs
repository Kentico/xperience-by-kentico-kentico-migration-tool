using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("STORECONTENT_TV")]
public partial class StorecontentTv
{
    [Key]
    [Column("TVID")]
    public int Tvid { get; set; }

    [Column("TVScreenSize")]
    [StringLength(100)]
    public string? TvscreenSize { get; set; }

    [Column("TVPixelResolution")]
    [StringLength(100)]
    public string? TvpixelResolution { get; set; }

    [Column("TVContrastRatio")]
    [StringLength(100)]
    public string? TvcontrastRatio { get; set; }

    [Column("TVVideoSystem")]
    [StringLength(100)]
    public string? TvvideoSystem { get; set; }

    [Column("TVSound")]
    [StringLength(100)]
    public string? Tvsound { get; set; }

    [Column("TVInputsOutputs")]
    [StringLength(300)]
    public string? TvinputsOutputs { get; set; }

    [Column("TVPowerConsumption")]
    [StringLength(200)]
    public string? TvpowerConsumption { get; set; }

    [Column("TVDimensions")]
    [StringLength(100)]
    public string? Tvdimensions { get; set; }

    [Column("TVWeight")]
    public double? Tvweight { get; set; }
}
