using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("STORECONTENT_Perfume")]
public partial class StorecontentPerfume
{
    [Key]
    [Column("PerfumeID")]
    public int PerfumeId { get; set; }

    [StringLength(250)]
    public string? PerfumeIngredients { get; set; }

    [StringLength(100)]
    public string? PerfumeSize { get; set; }
}
