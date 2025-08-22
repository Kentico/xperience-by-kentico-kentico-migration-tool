using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("CONTENT_Laptop")]
public class ContentLaptop
{
    [Key]
    [Column("LaptopID")]
    public int LaptopId { get; set; }

    [StringLength(100)]
    public string? LaptopProcessorType { get; set; }

    [StringLength(50)]
    public string? LaptopDisplayType { get; set; }

    [StringLength(50)]
    public string? LaptopResolution { get; set; }

    [StringLength(100)]
    public string? LaptopGraphicsCard { get; set; }

    [StringLength(50)]
    public string? LaptopMemoryType { get; set; }

    public int? LaptopMemorySize { get; set; }

    [StringLength(100)]
    public string? LaptopOpticalDrive { get; set; }

    [StringLength(100)]
    public string? LaptopHardDrive { get; set; }

    [Column("LaptopWirelessLAN")]
    public bool? LaptopWirelessLan { get; set; }

    public bool? LaptopBluetooth { get; set; }

    public bool? LaptopInfraport { get; set; }

    [StringLength(100)]
    public string? LaptopBatteryType { get; set; }

    [StringLength(100)]
    public string? LaptopOperatingSystem { get; set; }

    public string? LaptopAccessories { get; set; }

    public double? LaptopWeight { get; set; }
}
