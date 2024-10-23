using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[PrimaryKey("MultiBuyDiscountId", "NodeId")]
[Table("COM_MultiBuyDiscountTree")]
[Index("NodeId", Name = "IX_COM_MultiBuyDiscountTree_NodeID")]
public class ComMultiBuyDiscountTree
{
    [Key]
    [Column("MultiBuyDiscountID")]
    public int MultiBuyDiscountId { get; set; }

    [Key]
    [Column("NodeID")]
    public int NodeId { get; set; }

    [Required]
    public bool? NodeIncluded { get; set; }

    [ForeignKey("MultiBuyDiscountId")]
    [InverseProperty("ComMultiBuyDiscountTrees")]
    public virtual ComMultiBuyDiscount MultiBuyDiscount { get; set; } = null!;

    [ForeignKey("NodeId")]
    [InverseProperty("ComMultiBuyDiscountTrees")]
    public virtual CmsTree Node { get; set; } = null!;
}
