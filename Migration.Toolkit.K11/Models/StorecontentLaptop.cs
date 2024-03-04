using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("STORECONTENT_Laptop")]
public partial class StorecontentLaptop
{
    [Key]
    [Column("LaptopID")]
    public int LaptopId { get; set; }

    [StringLength(100)]
    public string? LaptopProcessorType { get; set; }

    [StringLength(200)]
    public string? LaptopMemory { get; set; }

    [StringLength(100)]
    public string? LaptopGraphicsCard { get; set; }

    [StringLength(100)]
    public string? LaptopHardDrive { get; set; }

    [StringLength(100)]
    public string? LaptopOpticalDrive { get; set; }

    [StringLength(100)]
    public string? LaptopDisplayType { get; set; }

    [StringLength(100)]
    public string? LaptopDisplaySize { get; set; }

    [StringLength(50)]
    public string? LaptopDisplayResolution { get; set; }

    [StringLength(400)]
    public string? LaptopInputsOutputs { get; set; }

    [Column("LaptopWirelessLAN")]
    [StringLength(100)]
    public string? LaptopWirelessLan { get; set; }

    [StringLength(100)]
    public string? LaptopNetwork { get; set; }

    public bool? LaptopBluetooth { get; set; }

    public bool? LaptopWebcam { get; set; }

    [StringLength(100)]
    public string? LaptopOperatingSystem { get; set; }

    [StringLength(100)]
    public string? LaptopBattery { get; set; }

    [StringLength(100)]
    public string? LaptopDimensions { get; set; }

    public double? LaptopWeight { get; set; }
}
