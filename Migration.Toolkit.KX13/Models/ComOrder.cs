using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_Order")]
[Index("OrderCreatedByUserId", Name = "IX_COM_Order_OrderCreatedByUserID")]
[Index("OrderCurrencyId", Name = "IX_COM_Order_OrderCurrencyID")]
[Index("OrderCustomerId", Name = "IX_COM_Order_OrderCustomerID")]
[Index("OrderPaymentOptionId", Name = "IX_COM_Order_OrderPaymentOptionID")]
[Index("OrderShippingOptionId", Name = "IX_COM_Order_OrderShippingOptionID")]
[Index("OrderSiteId", "OrderDate", Name = "IX_COM_Order_OrderSiteID_OrderDate", IsDescending = new[] { false, true })]
[Index("OrderStatusId", Name = "IX_COM_Order_OrderStatusID")]
public partial class ComOrder
{
    [Key]
    [Column("OrderID")]
    public int OrderId { get; set; }

    [Column("OrderShippingOptionID")]
    public int? OrderShippingOptionId { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal? OrderTotalShipping { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal OrderTotalPrice { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal OrderTotalTax { get; set; }

    public DateTime OrderDate { get; set; }

    [Column("OrderStatusID")]
    public int? OrderStatusId { get; set; }

    [Column("OrderCurrencyID")]
    public int? OrderCurrencyId { get; set; }

    [Column("OrderCustomerID")]
    public int OrderCustomerId { get; set; }

    [Column("OrderCreatedByUserID")]
    public int? OrderCreatedByUserId { get; set; }

    public string? OrderNote { get; set; }

    [Column("OrderSiteID")]
    public int OrderSiteId { get; set; }

    [Column("OrderPaymentOptionID")]
    public int? OrderPaymentOptionId { get; set; }

    public string? OrderInvoice { get; set; }

    [StringLength(200)]
    public string? OrderInvoiceNumber { get; set; }

    [StringLength(100)]
    public string? OrderTrackingNumber { get; set; }

    public string? OrderCustomData { get; set; }

    public string? OrderPaymentResult { get; set; }

    [Column("OrderGUID")]
    public Guid OrderGuid { get; set; }

    public DateTime OrderLastModified { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal? OrderTotalPriceInMainCurrency { get; set; }

    public bool? OrderIsPaid { get; set; }

    [StringLength(50)]
    public string? OrderCulture { get; set; }

    public string? OrderDiscounts { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal OrderGrandTotal { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal? OrderGrandTotalInMainCurrency { get; set; }

    public string? OrderOtherPayments { get; set; }

    public string? OrderTaxSummary { get; set; }

    public string? OrderCouponCodes { get; set; }

    [InverseProperty("AddressOrder")]
    public virtual ICollection<ComOrderAddress> ComOrderAddresses { get; set; } = new List<ComOrderAddress>();

    [InverseProperty("OrderItemOrder")]
    public virtual ICollection<ComOrderItem> ComOrderItems { get; set; } = new List<ComOrderItem>();

    [InverseProperty("Order")]
    public virtual ICollection<ComOrderStatusUser> ComOrderStatusUsers { get; set; } = new List<ComOrderStatusUser>();

    [ForeignKey("OrderCreatedByUserId")]
    [InverseProperty("ComOrders")]
    public virtual CmsUser? OrderCreatedByUser { get; set; }

    [ForeignKey("OrderCurrencyId")]
    [InverseProperty("ComOrders")]
    public virtual ComCurrency? OrderCurrency { get; set; }

    [ForeignKey("OrderCustomerId")]
    [InverseProperty("ComOrders")]
    public virtual ComCustomer OrderCustomer { get; set; } = null!;

    [ForeignKey("OrderPaymentOptionId")]
    [InverseProperty("ComOrders")]
    public virtual ComPaymentOption? OrderPaymentOption { get; set; }

    [ForeignKey("OrderShippingOptionId")]
    [InverseProperty("ComOrders")]
    public virtual ComShippingOption? OrderShippingOption { get; set; }

    [ForeignKey("OrderSiteId")]
    [InverseProperty("ComOrders")]
    public virtual CmsSite OrderSite { get; set; } = null!;

    [ForeignKey("OrderStatusId")]
    [InverseProperty("ComOrders")]
    public virtual ComOrderStatus? OrderStatus { get; set; }
}
