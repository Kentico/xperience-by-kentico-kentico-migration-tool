namespace Migration.Toolkit.KXP.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("COM_MultiBuyDiscount")]
    [Index("MultiBuyDiscountApplyToSkuid", Name = "IX_COM_MultiBuyDiscount_MultiBuyDiscountApplyToSKUID")]
    [Index("MultiBuyDiscountSiteId", Name = "IX_COM_MultiBuyDiscount_MultiBuyDiscountSiteID")]
    public partial class ComMultiBuyDiscount
    {
        public ComMultiBuyDiscount()
        {
            ComMultiBuyCouponCodes = new HashSet<ComMultiBuyCouponCode>();
            ComMultiBuyDiscountBrands = new HashSet<ComMultiBuyDiscountBrand>();
            ComMultiBuyDiscountCollections = new HashSet<ComMultiBuyDiscountCollection>();
            ComMultiBuyDiscountTrees = new HashSet<ComMultiBuyDiscountTree>();
            Departments = new HashSet<ComDepartment>();
            Skus = new HashSet<ComSku>();
        }

        [Key]
        [Column("MultiBuyDiscountID")]
        public int MultiBuyDiscountId { get; set; }
        [StringLength(200)]
        public string MultiBuyDiscountDisplayName { get; set; } = null!;
        [StringLength(200)]
        public string MultiBuyDiscountName { get; set; } = null!;
        public string? MultiBuyDiscountDescription { get; set; }
        [Required]
        public bool? MultiBuyDiscountEnabled { get; set; }
        [Column("MultiBuyDiscountGUID")]
        public Guid MultiBuyDiscountGuid { get; set; }
        public DateTime MultiBuyDiscountLastModified { get; set; }
        [Column("MultiBuyDiscountSiteID")]
        public int MultiBuyDiscountSiteId { get; set; }
        [Required]
        public bool? MultiBuyDiscountApplyFurtherDiscounts { get; set; }
        public int MultiBuyDiscountMinimumBuyCount { get; set; }
        public DateTime? MultiBuyDiscountValidFrom { get; set; }
        public DateTime? MultiBuyDiscountValidTo { get; set; }
        [StringLength(200)]
        public string MultiBuyDiscountCustomerRestriction { get; set; } = null!;
        [StringLength(400)]
        public string? MultiBuyDiscountRoles { get; set; }
        [Column("MultiBuyDiscountApplyToSKUID")]
        public int? MultiBuyDiscountApplyToSkuid { get; set; }
        public int? MultiBuyDiscountLimitPerOrder { get; set; }
        public bool? MultiBuyDiscountUsesCoupons { get; set; }
        [Column(TypeName = "decimal(18, 9)")]
        public decimal? MultiBuyDiscountValue { get; set; }
        public bool? MultiBuyDiscountIsFlat { get; set; }
        [Required]
        public bool? MultiBuyDiscountAutoAddEnabled { get; set; }
        public int? MultiBuyDiscountPriority { get; set; }
        public bool MultiBuyDiscountIsProductCoupon { get; set; }

        [ForeignKey("MultiBuyDiscountApplyToSkuid")]
        [InverseProperty("ComMultiBuyDiscounts")]
        public virtual ComSku? MultiBuyDiscountApplyToSku { get; set; }
        [ForeignKey("MultiBuyDiscountSiteId")]
        [InverseProperty("ComMultiBuyDiscounts")]
        public virtual CmsSite MultiBuyDiscountSite { get; set; } = null!;
        [InverseProperty("MultiBuyCouponCodeMultiBuyDiscount")]
        public virtual ICollection<ComMultiBuyCouponCode> ComMultiBuyCouponCodes { get; set; }
        [InverseProperty("MultiBuyDiscount")]
        public virtual ICollection<ComMultiBuyDiscountBrand> ComMultiBuyDiscountBrands { get; set; }
        [InverseProperty("MultibuyDiscount")]
        public virtual ICollection<ComMultiBuyDiscountCollection> ComMultiBuyDiscountCollections { get; set; }
        [InverseProperty("MultiBuyDiscount")]
        public virtual ICollection<ComMultiBuyDiscountTree> ComMultiBuyDiscountTrees { get; set; }

        [ForeignKey("MultiBuyDiscountId")]
        [InverseProperty("MultiBuyDiscounts")]
        public virtual ICollection<ComDepartment> Departments { get; set; }
        [ForeignKey("MultiBuyDiscountId")]
        [InverseProperty("MultiBuyDiscounts")]
        public virtual ICollection<ComSku> Skus { get; set; }
    }
}
