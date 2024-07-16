using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("CONTENT_Product")]
public partial class ContentProduct
{
    [Key]
    [Column("ProductID")]
    public int ProductId { get; set; }

    [StringLength(440)]
    public string? ProductName { get; set; }
}
