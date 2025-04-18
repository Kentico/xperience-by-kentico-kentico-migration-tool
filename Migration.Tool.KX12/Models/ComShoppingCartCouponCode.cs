using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[Table("COM_ShoppingCartCouponCode")]
[Index("ShoppingCartId", Name = "IX_COM_ShoppingCartCouponCode_ShoppingCartID")]
public class ComShoppingCartCouponCode
{
    [Key]
    [Column("ShoppingCartCouponCodeID")]
    public int ShoppingCartCouponCodeId { get; set; }

    [Column("ShoppingCartID")]
    public int ShoppingCartId { get; set; }

    [StringLength(200)]
    public string CouponCode { get; set; } = null!;

    [ForeignKey("ShoppingCartId")]
    [InverseProperty("ComShoppingCartCouponCodes")]
    public virtual ComShoppingCart ShoppingCart { get; set; } = null!;
}
