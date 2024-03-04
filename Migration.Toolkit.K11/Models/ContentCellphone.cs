using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CONTENT_Cellphone")]
public partial class ContentCellphone
{
    [Key]
    [Column("CellphoneID")]
    public int CellphoneId { get; set; }

    [StringLength(100)]
    public string? CellDisplayType { get; set; }

    public int? CellDisplayWidth { get; set; }

    public int? CellDisplayHeight { get; set; }

    [StringLength(100)]
    public string? CellDisplayResolution { get; set; }

    public bool? CellBluetooth { get; set; }

    [Column("CellIrDA")]
    public bool? CellIrDa { get; set; }

    [Column("CellGPRS")]
    public bool? CellGprs { get; set; }

    [Column("CellEDGE")]
    public bool? CellEdge { get; set; }

    [Column("CellHSCSD")]
    public bool? CellHscsd { get; set; }

    public bool? Cell3G { get; set; }

    public bool? CellWiFi { get; set; }

    public bool? CellJava { get; set; }

    public bool? CellCamera { get; set; }

    [Column("CellMP3")]
    public bool? CellMp3 { get; set; }
}
