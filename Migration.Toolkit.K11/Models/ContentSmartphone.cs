using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("CONTENT_Smartphone")]
public partial class ContentSmartphone
{
    [Key]
    [Column("SmartphoneID")]
    public int SmartphoneId { get; set; }

    [Column("SmartphoneOS")]
    [StringLength(100)]
    public string? SmartphoneOs { get; set; }

    [StringLength(100)]
    public string? SmartphoneDimensions { get; set; }

    [StringLength(50)]
    public string? SmartphoneWeight { get; set; }

    [StringLength(50)]
    public string? SmartphoneDisplayType { get; set; }

    [StringLength(50)]
    public string? SmartphoneDisplaySize { get; set; }

    [StringLength(50)]
    public string? SmartphoneDisplayResolution { get; set; }

    [Column("SmartphoneCPU")]
    [StringLength(100)]
    public string? SmartphoneCpu { get; set; }

    [Column("SmartphoneRAM")]
    [StringLength(50)]
    public string? SmartphoneRam { get; set; }

    [StringLength(50)]
    public string? SmartphoneInternalStorage { get; set; }

    [StringLength(100)]
    public string? SmartphoneRemovableStorage { get; set; }

    [StringLength(100)]
    public string? SmartphoneBatteryType { get; set; }

    [StringLength(50)]
    public string? SmartphoneCamera { get; set; }

    [Column("SmartphoneGPS")]
    public bool? SmartphoneGps { get; set; }
}
