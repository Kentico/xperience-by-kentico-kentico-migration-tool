using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_ShippingOption")]
[Index("ShippingOptionCarrierId", Name = "IX_COM_ShippingOption_ShippingOptionCarrierID")]
[Index("ShippingOptionSiteId", Name = "IX_COM_ShippingOption_ShippingOptionSiteID_ShippingOptionDisplayName_ShippingOptionEnabled")]
[Index("ShippingOptionTaxClassId", Name = "IX_COM_ShippingOption_ShippingOptionTaxClassID")]
public partial class ComShippingOption
{
    [Key]
    [Column("ShippingOptionID")]
    public int ShippingOptionId { get; set; }

    [StringLength(200)]
    public string ShippingOptionName { get; set; } = null!;

    [StringLength(200)]
    public string ShippingOptionDisplayName { get; set; } = null!;

    [Required]
    public bool? ShippingOptionEnabled { get; set; }

    [Column("ShippingOptionSiteID")]
    public int? ShippingOptionSiteId { get; set; }

    [Column("ShippingOptionGUID")]
    public Guid ShippingOptionGuid { get; set; }

    public DateTime ShippingOptionLastModified { get; set; }

    [Column("ShippingOptionThumbnailGUID")]
    public Guid? ShippingOptionThumbnailGuid { get; set; }

    public string? ShippingOptionDescription { get; set; }

    [Column("ShippingOptionCarrierID")]
    public int? ShippingOptionCarrierId { get; set; }

    [StringLength(200)]
    public string? ShippingOptionCarrierServiceName { get; set; }

    [Column("ShippingOptionTaxClassID")]
    public int? ShippingOptionTaxClassId { get; set; }

    [InverseProperty("OrderShippingOption")]
    public virtual ICollection<ComOrder> ComOrders { get; set; } = new List<ComOrder>();

    [InverseProperty("ShippingCostShippingOption")]
    public virtual ICollection<ComShippingCost> ComShippingCosts { get; set; } = new List<ComShippingCost>();

    [InverseProperty("ShoppingCartShippingOption")]
    public virtual ICollection<ComShoppingCart> ComShoppingCarts { get; set; } = new List<ComShoppingCart>();

    [ForeignKey("ShippingOptionCarrierId")]
    [InverseProperty("ComShippingOptions")]
    public virtual ComCarrier? ShippingOptionCarrier { get; set; }

    [ForeignKey("ShippingOptionSiteId")]
    [InverseProperty("ComShippingOptions")]
    public virtual CmsSite? ShippingOptionSite { get; set; }

    [ForeignKey("ShippingOptionTaxClassId")]
    [InverseProperty("ComShippingOptions")]
    public virtual ComTaxClass? ShippingOptionTaxClass { get; set; }
}
