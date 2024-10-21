using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[PrimaryKey("MultibuyDiscountId", "CollectionId")]
[Table("COM_MultiBuyDiscountCollection")]
[Index("CollectionId", Name = "IX_COM_MultiBuyDiscountCollection_CollectionID")]
public class ComMultiBuyDiscountCollection
{
    [Key]
    [Column("MultibuyDiscountID")]
    public int MultibuyDiscountId { get; set; }

    [Key]
    [Column("CollectionID")]
    public int CollectionId { get; set; }

    [Required]
    public bool? CollectionIncluded { get; set; }

    [ForeignKey("CollectionId")]
    [InverseProperty("ComMultiBuyDiscountCollections")]
    public virtual ComCollection Collection { get; set; } = null!;

    [ForeignKey("MultibuyDiscountId")]
    [InverseProperty("ComMultiBuyDiscountCollections")]
    public virtual ComMultiBuyDiscount MultibuyDiscount { get; set; } = null!;
}
