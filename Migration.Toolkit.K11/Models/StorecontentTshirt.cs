using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("STORECONTENT_Tshirt")]
public partial class StorecontentTshirt
{
    [Key]
    [Column("TshirtID")]
    public int TshirtId { get; set; }

    [StringLength(100)]
    public string? TshirtColor { get; set; }

    [StringLength(100)]
    public string? TshirtStyle { get; set; }
}
