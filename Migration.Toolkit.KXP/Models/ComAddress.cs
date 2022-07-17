namespace Migration.Toolkit.KXP.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("COM_Address")]
    [Index("AddressCountryId", Name = "IX_COM_Address_AddressCountryID")]
    [Index("AddressCustomerId", Name = "IX_COM_Address_AddressCustomerID")]
    [Index("AddressStateId", Name = "IX_COM_Address_AddressStateID")]
    public partial class ComAddress
    {
        public ComAddress()
        {
            ComShoppingCartShoppingCartBillingAddresses = new HashSet<ComShoppingCart>();
            ComShoppingCartShoppingCartCompanyAddresses = new HashSet<ComShoppingCart>();
            ComShoppingCartShoppingCartShippingAddresses = new HashSet<ComShoppingCart>();
        }

        [Key]
        [Column("AddressID")]
        public int AddressId { get; set; }
        [StringLength(200)]
        public string AddressName { get; set; } = null!;
        [StringLength(100)]
        public string AddressLine1 { get; set; } = null!;
        [StringLength(100)]
        public string? AddressLine2 { get; set; }
        [StringLength(100)]
        public string AddressCity { get; set; } = null!;
        [StringLength(20)]
        public string AddressZip { get; set; } = null!;
        [StringLength(26)]
        public string? AddressPhone { get; set; }
        [Column("AddressCustomerID")]
        public int AddressCustomerId { get; set; }
        [Column("AddressCountryID")]
        public int AddressCountryId { get; set; }
        [Column("AddressStateID")]
        public int? AddressStateId { get; set; }
        [StringLength(200)]
        public string AddressPersonalName { get; set; } = null!;
        [Column("AddressGUID")]
        public Guid? AddressGuid { get; set; }
        public DateTime AddressLastModified { get; set; }

        [ForeignKey("AddressCountryId")]
        [InverseProperty("ComAddresses")]
        public virtual CmsCountry AddressCountry { get; set; } = null!;
        [ForeignKey("AddressCustomerId")]
        [InverseProperty("ComAddresses")]
        public virtual ComCustomer AddressCustomer { get; set; } = null!;
        [ForeignKey("AddressStateId")]
        [InverseProperty("ComAddresses")]
        public virtual CmsState? AddressState { get; set; }
        [InverseProperty("ShoppingCartBillingAddress")]
        public virtual ICollection<ComShoppingCart> ComShoppingCartShoppingCartBillingAddresses { get; set; }
        [InverseProperty("ShoppingCartCompanyAddress")]
        public virtual ICollection<ComShoppingCart> ComShoppingCartShoppingCartCompanyAddresses { get; set; }
        [InverseProperty("ShoppingCartShippingAddress")]
        public virtual ICollection<ComShoppingCart> ComShoppingCartShoppingCartShippingAddresses { get; set; }
    }
}
