using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_MultiBuyCouponCode")]
[Index("MultiBuyCouponCodeMultiBuyDiscountId", Name = "IX_COM_MultiBuyCouponCode_MultiBuyCouponCodeMultiBuyDiscountID")]
public partial class ComMultiBuyCouponCode
{
    [Key]
    [Column("MultiBuyCouponCodeID")]
    public int MultiBuyCouponCodeId { get; set; }

    [StringLength(200)]
    public string MultiBuyCouponCodeCode { get; set; } = null!;

    public int? MultiBuyCouponCodeUseLimit { get; set; }

    public int? MultiBuyCouponCodeUseCount { get; set; }

    [Column("MultiBuyCouponCodeMultiBuyDiscountID")]
    public int MultiBuyCouponCodeMultiBuyDiscountId { get; set; }

    public DateTime MultiBuyCouponCodeLastModified { get; set; }

    [Column("MultiBuyCouponCodeGUID")]
    public Guid MultiBuyCouponCodeGuid { get; set; }

    [ForeignKey("MultiBuyCouponCodeMultiBuyDiscountId")]
    [InverseProperty("ComMultiBuyCouponCodes")]
    public virtual ComMultiBuyDiscount MultiBuyCouponCodeMultiBuyDiscount { get; set; } = null!;
}
