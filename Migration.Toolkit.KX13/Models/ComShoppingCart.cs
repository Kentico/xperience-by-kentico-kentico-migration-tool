using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_ShoppingCart")]
[Index("ShoppingCartBillingAddressId", Name = "IX_COM_ShoppingCart_ShoppingCartBillingAddressID")]
[Index("ShoppingCartCompanyAddressId", Name = "IX_COM_ShoppingCart_ShoppingCartCompanyAddressID")]
[Index("ShoppingCartCurrencyId", Name = "IX_COM_ShoppingCart_ShoppingCartCurrencyID")]
[Index("ShoppingCartCustomerId", Name = "IX_COM_ShoppingCart_ShoppingCartCustomerID")]
[Index("ShoppingCartLastUpdate", Name = "IX_COM_ShoppingCart_ShoppingCartLastUpdate")]
[Index("ShoppingCartPaymentOptionId", Name = "IX_COM_ShoppingCart_ShoppingCartPaymentOptionID")]
[Index("ShoppingCartShippingAddressId", Name = "IX_COM_ShoppingCart_ShoppingCartShippingAddressID")]
[Index("ShoppingCartShippingOptionId", Name = "IX_COM_ShoppingCart_ShoppingCartShippingOptionID")]
[Index("ShoppingCartSiteId", Name = "IX_COM_ShoppingCart_ShoppingCartSiteID")]
[Index("ShoppingCartGuid", Name = "IX_COM_ShoppingCart_ShoppingCartSiteID_ShoppingCartGUID")]
[Index("ShoppingCartUserId", Name = "IX_COM_ShoppingCart_ShoppingCartUserID")]
public partial class ComShoppingCart
{
    [Key]
    [Column("ShoppingCartID")]
    public int ShoppingCartId { get; set; }

    [Column("ShoppingCartGUID")]
    public Guid ShoppingCartGuid { get; set; }

    [Column("ShoppingCartUserID")]
    public int? ShoppingCartUserId { get; set; }

    [Column("ShoppingCartSiteID")]
    public int ShoppingCartSiteId { get; set; }

    public DateTime ShoppingCartLastUpdate { get; set; }

    [Column("ShoppingCartCurrencyID")]
    public int? ShoppingCartCurrencyId { get; set; }

    [Column("ShoppingCartPaymentOptionID")]
    public int? ShoppingCartPaymentOptionId { get; set; }

    [Column("ShoppingCartShippingOptionID")]
    public int? ShoppingCartShippingOptionId { get; set; }

    [Column("ShoppingCartBillingAddressID")]
    public int? ShoppingCartBillingAddressId { get; set; }

    [Column("ShoppingCartShippingAddressID")]
    public int? ShoppingCartShippingAddressId { get; set; }

    [Column("ShoppingCartCustomerID")]
    public int? ShoppingCartCustomerId { get; set; }

    public string? ShoppingCartNote { get; set; }

    [Column("ShoppingCartCompanyAddressID")]
    public int? ShoppingCartCompanyAddressId { get; set; }

    public string? ShoppingCartCustomData { get; set; }

    [Column("ShoppingCartContactID")]
    public int? ShoppingCartContactId { get; set; }

    [InverseProperty("ShoppingCart")]
    public virtual ICollection<ComShoppingCartCouponCode> ComShoppingCartCouponCodes { get; set; } = new List<ComShoppingCartCouponCode>();

    [InverseProperty("ShoppingCart")]
    public virtual ICollection<ComShoppingCartSku> ComShoppingCartSkus { get; set; } = new List<ComShoppingCartSku>();

    [ForeignKey("ShoppingCartBillingAddressId")]
    [InverseProperty("ComShoppingCartShoppingCartBillingAddresses")]
    public virtual ComAddress? ShoppingCartBillingAddress { get; set; }

    [ForeignKey("ShoppingCartCompanyAddressId")]
    [InverseProperty("ComShoppingCartShoppingCartCompanyAddresses")]
    public virtual ComAddress? ShoppingCartCompanyAddress { get; set; }

    [ForeignKey("ShoppingCartCurrencyId")]
    [InverseProperty("ComShoppingCarts")]
    public virtual ComCurrency? ShoppingCartCurrency { get; set; }

    [ForeignKey("ShoppingCartCustomerId")]
    [InverseProperty("ComShoppingCarts")]
    public virtual ComCustomer? ShoppingCartCustomer { get; set; }

    [ForeignKey("ShoppingCartPaymentOptionId")]
    [InverseProperty("ComShoppingCarts")]
    public virtual ComPaymentOption? ShoppingCartPaymentOption { get; set; }

    [ForeignKey("ShoppingCartShippingAddressId")]
    [InverseProperty("ComShoppingCartShoppingCartShippingAddresses")]
    public virtual ComAddress? ShoppingCartShippingAddress { get; set; }

    [ForeignKey("ShoppingCartShippingOptionId")]
    [InverseProperty("ComShoppingCarts")]
    public virtual ComShippingOption? ShoppingCartShippingOption { get; set; }

    [ForeignKey("ShoppingCartSiteId")]
    [InverseProperty("ComShoppingCarts")]
    public virtual CmsSite ShoppingCartSite { get; set; } = null!;

    [ForeignKey("ShoppingCartUserId")]
    [InverseProperty("ComShoppingCarts")]
    public virtual CmsUser? ShoppingCartUser { get; set; }
}
