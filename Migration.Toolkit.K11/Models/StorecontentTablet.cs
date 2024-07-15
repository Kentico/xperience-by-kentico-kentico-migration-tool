using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("STORECONTENT_tablet")]
public partial class StorecontentTablet
{
    [Key]
    [Column("TabletID")]
    public int TabletId { get; set; }

    [StringLength(100)]
    public string? TabletProcessor { get; set; }

    [StringLength(100)]
    public string? TabletMemory { get; set; }

    [StringLength(100)]
    public string? TabletOperatingSystem { get; set; }

    [StringLength(100)]
    public string? TabletDisplayType { get; set; }

    [StringLength(100)]
    public string? TabletDisplaySize { get; set; }

    [StringLength(100)]
    public string? TabletResolution { get; set; }

    [StringLength(300)]
    public string? TabletInputsOutputs { get; set; }

    [StringLength(100)]
    public string? TabletWifi { get; set; }

    public bool? TabletBluetooth { get; set; }

    [Column("TabletGPS")]
    public bool? TabletGps { get; set; }

    [StringLength(100)]
    public string? TabletKeyboard { get; set; }

    [StringLength(100)]
    public string? TabletBattery { get; set; }

    [StringLength(100)]
    public string? TabletDimensions { get; set; }

    public double? TabletWeight { get; set; }
}