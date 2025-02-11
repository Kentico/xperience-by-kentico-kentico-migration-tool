using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("STORECONTENT_Perfume")]
public class StorecontentPerfume
{
    [Key]
    [Column("PerfumeID")]
    public int PerfumeId { get; set; }

    [StringLength(250)]
    public string? PerfumeIngredients { get; set; }

    [StringLength(100)]
    public string? PerfumeSize { get; set; }
}
