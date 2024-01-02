using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_GiftCard")]
[Index("GiftCardSiteId", Name = "IX_COM_GiftCard_GiftCardSiteID")]
public partial class ComGiftCard
{
    [Key]
    [Column("GiftCardID")]
    public int GiftCardId { get; set; }

    public Guid GiftCardGuid { get; set; }

    [StringLength(200)]
    public string GiftCardDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string GiftCardName { get; set; } = null!;

    public string? GiftCardDescription { get; set; }

    [Required]
    public bool? GiftCardEnabled { get; set; }

    public DateTime GiftCardLastModified { get; set; }

    [Column("GiftCardSiteID")]
    public int GiftCardSiteId { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal GiftCardValue { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal? GiftCardMinimumOrderPrice { get; set; }

    public string? GiftCardCartCondition { get; set; }

    public DateTime? GiftCardValidFrom { get; set; }

    public DateTime? GiftCardValidTo { get; set; }

    [StringLength(200)]
    public string? GiftCardCustomerRestriction { get; set; }

    [StringLength(400)]
    public string? GiftCardRoles { get; set; }

    [InverseProperty("GiftCardCouponCodeGiftCard")]
    public virtual ICollection<ComGiftCardCouponCode> ComGiftCardCouponCodes { get; set; } = new List<ComGiftCardCouponCode>();

    [ForeignKey("GiftCardSiteId")]
    [InverseProperty("ComGiftCards")]
    public virtual CmsSite GiftCardSite { get; set; } = null!;
}
