using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("CONTENT_Product")]
public class ContentProduct
{
    [Key]
    [Column("ProductID")]
    public int ProductId { get; set; }

    [StringLength(440)]
    public string? ProductName { get; set; }
}
