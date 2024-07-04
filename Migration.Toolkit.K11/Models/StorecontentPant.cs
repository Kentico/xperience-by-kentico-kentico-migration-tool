using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("STORECONTENT_Pants")]
public partial class StorecontentPant
{
    [Key]
    [Column("PantsID")]
    public int PantsId { get; set; }

    [StringLength(100)]
    public string? PantsColor { get; set; }

    [StringLength(100)]
    public string? PantsStyle { get; set; }
}