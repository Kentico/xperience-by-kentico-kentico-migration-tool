using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_GiftCardCouponCode")]
[Index("GiftCardCouponCodeGiftCardId", Name = "IX_COM_GiftCardCouponCodeGiftCardID")]
public partial class ComGiftCardCouponCode
{
    [Key]
    [Column("GiftCardCouponCodeID")]
    public int GiftCardCouponCodeId { get; set; }

    [StringLength(200)]
    public string GiftCardCouponCodeCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 9)")]
    public decimal GiftCardCouponCodeRemainingValue { get; set; }

    [Column("GiftCardCouponCodeGiftCardID")]
    public int GiftCardCouponCodeGiftCardId { get; set; }

    public Guid GiftCardCouponCodeGuid { get; set; }

    public DateTime GiftCardCouponCodeLastModified { get; set; }

    [ForeignKey("GiftCardCouponCodeGiftCardId")]
    [InverseProperty("ComGiftCardCouponCodes")]
    public virtual ComGiftCard GiftCardCouponCodeGiftCard { get; set; } = null!;
}
