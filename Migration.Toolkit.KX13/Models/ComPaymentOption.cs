using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_PaymentOption")]
[Index("PaymentOptionAuthorizedOrderStatusId", Name = "IX_COM_PaymentOption_PaymentOptionAuthorizedOrderStatusID")]
[Index("PaymentOptionFailedOrderStatusId", Name = "IX_COM_PaymentOption_PaymentOptionFailedOrderStatusID")]
[Index("PaymentOptionSiteId", Name = "IX_COM_PaymentOption_PaymentOptionSiteID")]
[Index("PaymentOptionSucceededOrderStatusId", Name = "IX_COM_PaymentOption_PaymentOptionSucceededOrderStatusID")]
public partial class ComPaymentOption
{
    [Key]
    [Column("PaymentOptionID")]
    public int PaymentOptionId { get; set; }

    [StringLength(200)]
    public string PaymentOptionName { get; set; } = null!;

    [StringLength(200)]
    public string PaymentOptionDisplayName { get; set; } = null!;

    [Required]
    public bool? PaymentOptionEnabled { get; set; }

    [Column("PaymentOptionSiteID")]
    public int? PaymentOptionSiteId { get; set; }

    [StringLength(500)]
    public string? PaymentOptionPaymentGateUrl { get; set; }

    [StringLength(200)]
    public string? PaymentOptionAssemblyName { get; set; }

    [StringLength(200)]
    public string? PaymentOptionClassName { get; set; }

    [Column("PaymentOptionSucceededOrderStatusID")]
    public int? PaymentOptionSucceededOrderStatusId { get; set; }

    [Column("PaymentOptionFailedOrderStatusID")]
    public int? PaymentOptionFailedOrderStatusId { get; set; }

    [Column("PaymentOptionGUID")]
    public Guid PaymentOptionGuid { get; set; }

    public DateTime PaymentOptionLastModified { get; set; }

    public bool? PaymentOptionAllowIfNoShipping { get; set; }

    [Column("PaymentOptionThumbnailGUID")]
    public Guid? PaymentOptionThumbnailGuid { get; set; }

    public string? PaymentOptionDescription { get; set; }

    [Column("PaymentOptionAuthorizedOrderStatusID")]
    public int? PaymentOptionAuthorizedOrderStatusId { get; set; }

    [InverseProperty("OrderPaymentOption")]
    public virtual ICollection<ComOrder> ComOrders { get; set; } = new List<ComOrder>();

    [InverseProperty("ShoppingCartPaymentOption")]
    public virtual ICollection<ComShoppingCart> ComShoppingCarts { get; set; } = new List<ComShoppingCart>();

    [ForeignKey("PaymentOptionAuthorizedOrderStatusId")]
    [InverseProperty("ComPaymentOptionPaymentOptionAuthorizedOrderStatuses")]
    public virtual ComOrderStatus? PaymentOptionAuthorizedOrderStatus { get; set; }

    [ForeignKey("PaymentOptionFailedOrderStatusId")]
    [InverseProperty("ComPaymentOptionPaymentOptionFailedOrderStatuses")]
    public virtual ComOrderStatus? PaymentOptionFailedOrderStatus { get; set; }

    [ForeignKey("PaymentOptionSiteId")]
    [InverseProperty("ComPaymentOptions")]
    public virtual CmsSite? PaymentOptionSite { get; set; }

    [ForeignKey("PaymentOptionSucceededOrderStatusId")]
    [InverseProperty("ComPaymentOptionPaymentOptionSucceededOrderStatuses")]
    public virtual ComOrderStatus? PaymentOptionSucceededOrderStatus { get; set; }
}
