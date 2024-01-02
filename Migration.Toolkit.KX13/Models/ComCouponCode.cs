using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_CouponCode")]
[Index("CouponCodeDiscountId", Name = "IX_COM_CouponCode_CouponCodeDiscountID")]
public partial class ComCouponCode
{
    [Key]
    [Column("CouponCodeID")]
    public int CouponCodeId { get; set; }

    [StringLength(200)]
    public string CouponCodeCode { get; set; } = null!;

    public int? CouponCodeUseCount { get; set; }

    public int? CouponCodeUseLimit { get; set; }

    [Column("CouponCodeDiscountID")]
    public int CouponCodeDiscountId { get; set; }

    public DateTime CouponCodeLastModified { get; set; }

    [Column("CouponCodeGUID")]
    public Guid CouponCodeGuid { get; set; }

    [ForeignKey("CouponCodeDiscountId")]
    [InverseProperty("ComCouponCodes")]
    public virtual ComDiscount CouponCodeDiscount { get; set; } = null!;
}
