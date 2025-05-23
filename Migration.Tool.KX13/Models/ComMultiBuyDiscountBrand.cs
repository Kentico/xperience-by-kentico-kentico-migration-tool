using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX13.Models;

[PrimaryKey("MultiBuyDiscountId", "BrandId")]
[Table("COM_MultiBuyDiscountBrand")]
[Index("BrandId", Name = "IX_COM_MultiBuyDiscountBrand_BrandID")]
public class ComMultiBuyDiscountBrand
{
    [Key]
    [Column("MultiBuyDiscountID")]
    public int MultiBuyDiscountId { get; set; }

    [Key]
    [Column("BrandID")]
    public int BrandId { get; set; }

    [Required]
    public bool? BrandIncluded { get; set; }

    [ForeignKey("BrandId")]
    [InverseProperty("ComMultiBuyDiscountBrands")]
    public virtual ComBrand Brand { get; set; } = null!;

    [ForeignKey("MultiBuyDiscountId")]
    [InverseProperty("ComMultiBuyDiscountBrands")]
    public virtual ComMultiBuyDiscount MultiBuyDiscount { get; set; } = null!;
}
