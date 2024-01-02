using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_Discount")]
[Index("DiscountSiteId", Name = "IX_COM_Discount_DiscountSiteID")]
public partial class ComDiscount
{
    [Key]
    [Column("DiscountID")]
    public int DiscountId { get; set; }

    [StringLength(200)]
    public string DiscountDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string DiscountName { get; set; } = null!;

    [Column(TypeName = "decimal(18, 9)")]
    public decimal DiscountValue { get; set; }

    [Required]
    public bool? DiscountEnabled { get; set; }

    [Column("DiscountGUID")]
    public Guid DiscountGuid { get; set; }

    public DateTime DiscountLastModified { get; set; }

    [Column("DiscountSiteID")]
    public int DiscountSiteId { get; set; }

    public string? DiscountDescription { get; set; }

    public DateTime? DiscountValidFrom { get; set; }

    public DateTime? DiscountValidTo { get; set; }

    public double DiscountOrder { get; set; }

    public string? DiscountProductCondition { get; set; }

    [StringLength(400)]
    public string? DiscountRoles { get; set; }

    [StringLength(200)]
    public string? DiscountCustomerRestriction { get; set; }

    public bool DiscountIsFlat { get; set; }

    public string? DiscountCartCondition { get; set; }

    [StringLength(100)]
    public string DiscountApplyTo { get; set; } = null!;

    [Required]
    public bool? DiscountApplyFurtherDiscounts { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal? DiscountOrderAmount { get; set; }

    public bool DiscountUsesCoupons { get; set; }

    [InverseProperty("CouponCodeDiscount")]
    public virtual ICollection<ComCouponCode> ComCouponCodes { get; set; } = new List<ComCouponCode>();

    [ForeignKey("DiscountSiteId")]
    [InverseProperty("ComDiscounts")]
    public virtual CmsSite DiscountSite { get; set; } = null!;
}
