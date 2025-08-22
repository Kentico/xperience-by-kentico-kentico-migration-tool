using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("STORECONTENT_Pants")]
public class StorecontentPant
{
    [Key]
    [Column("PantsID")]
    public int PantsId { get; set; }

    [StringLength(100)]
    public string? PantsColor { get; set; }

    [StringLength(100)]
    public string? PantsStyle { get; set; }
}
