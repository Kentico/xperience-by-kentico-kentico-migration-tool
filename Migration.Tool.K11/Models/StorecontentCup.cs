using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("STORECONTENT_Cup")]
public class StorecontentCup
{
    [Key]
    [Column("CupID")]
    public int CupId { get; set; }

    [StringLength(100)]
    public string? CupSize { get; set; }

    [StringLength(100)]
    public string? CupType { get; set; }

    [StringLength(100)]
    public string? CupStyle { get; set; }
}
