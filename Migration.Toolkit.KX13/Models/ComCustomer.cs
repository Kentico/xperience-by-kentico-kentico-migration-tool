using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_Customer")]
[Index("CustomerEmail", Name = "IX_COM_Customer_CustomerEmail")]
[Index("CustomerFirstName", Name = "IX_COM_Customer_CustomerFirstName")]
[Index("CustomerLastName", Name = "IX_COM_Customer_CustomerLastName")]
[Index("CustomerSiteId", Name = "IX_COM_Customer_CustomerSiteID")]
[Index("CustomerUserId", Name = "IX_COM_Customer_CustomerUserID")]
public partial class ComCustomer
{
    [Key]
    [Column("CustomerID")]
    public int CustomerId { get; set; }

    [StringLength(200)]
    public string CustomerFirstName { get; set; } = null!;

    [StringLength(200)]
    public string CustomerLastName { get; set; } = null!;

    [StringLength(254)]
    public string? CustomerEmail { get; set; }

    [StringLength(26)]
    public string? CustomerPhone { get; set; }

    [StringLength(50)]
    public string? CustomerFax { get; set; }

    [StringLength(200)]
    public string? CustomerCompany { get; set; }

    [Column("CustomerUserID")]
    public int? CustomerUserId { get; set; }

    [Column("CustomerGUID")]
    public Guid CustomerGuid { get; set; }

    [Column("CustomerTaxRegistrationID")]
    [StringLength(50)]
    public string? CustomerTaxRegistrationId { get; set; }

    [Column("CustomerOrganizationID")]
    [StringLength(50)]
    public string? CustomerOrganizationId { get; set; }

    public DateTime CustomerLastModified { get; set; }

    [Column("CustomerSiteID")]
    public int? CustomerSiteId { get; set; }

    public DateTime? CustomerCreated { get; set; }

    [InverseProperty("AddressCustomer")]
    public virtual ICollection<ComAddress> ComAddresses { get; set; } = new List<ComAddress>();

    [InverseProperty("EventCustomer")]
    public virtual ICollection<ComCustomerCreditHistory> ComCustomerCreditHistories { get; set; } = new List<ComCustomerCreditHistory>();

    [InverseProperty("OrderCustomer")]
    public virtual ICollection<ComOrder> ComOrders { get; set; } = new List<ComOrder>();

    [InverseProperty("ShoppingCartCustomer")]
    public virtual ICollection<ComShoppingCart> ComShoppingCarts { get; set; } = new List<ComShoppingCart>();

    [ForeignKey("CustomerSiteId")]
    [InverseProperty("ComCustomers")]
    public virtual CmsSite? CustomerSite { get; set; }

    [ForeignKey("CustomerUserId")]
    [InverseProperty("ComCustomers")]
    public virtual CmsUser? CustomerUser { get; set; }
}
