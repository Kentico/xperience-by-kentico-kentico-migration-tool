using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("STORECONTENT_CellPhone")]
public partial class StorecontentCellPhone
{
    [Key]
    [Column("CellPhoneID")]
    public int CellPhoneId { get; set; }

    [Column("CellPhoneCPU")]
    [StringLength(100)]
    public string? CellPhoneCpu { get; set; }

    [Column("CellPhoneRAM")]
    [StringLength(100)]
    public string? CellPhoneRam { get; set; }

    [Column("CellPhoneOS")]
    [StringLength(100)]
    public string? CellPhoneOs { get; set; }

    [StringLength(100)]
    public string? CellPhoneCamera { get; set; }

    [StringLength(100)]
    public string? CellPhoneDimensions { get; set; }

    public double? CellPhoneWeight { get; set; }

    [StringLength(100)]
    public string? CellPhoneDisplayType { get; set; }

    [StringLength(50)]
    public string? CellPhoneDisplaySize { get; set; }

    [StringLength(50)]
    public string? CellPhoneDisplayResolution { get; set; }

    [StringLength(100)]
    public string? CellPhoneInternalStorage { get; set; }

    [StringLength(100)]
    public string? CellPhoneRemovableStorage { get; set; }

    [StringLength(100)]
    public string? CellPhoneWiFi { get; set; }

    public bool? CellPhoneBluetooth { get; set; }

    [Column("CellPhoneIrDA")]
    public bool? CellPhoneIrDa { get; set; }

    [Column("CellPhoneGPRS")]
    public bool? CellPhoneGprs { get; set; }

    [Column("CellPhoneEDGE")]
    public bool? CellPhoneEdge { get; set; }

    [Column("CellPhoneHSCSD")]
    public bool? CellPhoneHscsd { get; set; }

    public bool? CellPhone3G { get; set; }

    [Column("CellPhoneGPS")]
    public bool? CellPhoneGps { get; set; }
}