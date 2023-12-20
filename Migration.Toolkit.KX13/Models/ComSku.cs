using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_SKU")]
[Index("SkubrandId", Name = "IX_COM_SKU_SKUBrandID")]
[Index("SkucollectionId", Name = "IX_COM_SKU_SKUCollectionID")]
[Index("SkudepartmentId", Name = "IX_COM_SKU_SKUDepartmentID")]
[Index("SkuinternalStatusId", Name = "IX_COM_SKU_SKUInternalStatusID")]
[Index("SkumanufacturerId", Name = "IX_COM_SKU_SKUManufacturerID")]
[Index("Skuname", Name = "IX_COM_SKU_SKUName")]
[Index("SkuoptionCategoryId", Name = "IX_COM_SKU_SKUOptionCategoryID")]
[Index("SkuparentSkuid", Name = "IX_COM_SKU_SKUParentSKUID")]
[Index("Skuprice", Name = "IX_COM_SKU_SKUPrice")]
[Index("SkupublicStatusId", Name = "IX_COM_SKU_SKUPublicStatusID")]
[Index("SkusiteId", Name = "IX_COM_SKU_SKUSiteID")]
[Index("SkusupplierId", Name = "IX_COM_SKU_SKUSupplierID")]
[Index("SkutaxClassId", Name = "IX_COM_SKU_SKUTaxClassID")]
public partial class ComSku
{
    [Key]
    [Column("SKUID")]
    public int Skuid { get; set; }

    [Column("SKUNumber")]
    [StringLength(200)]
    public string? Skunumber { get; set; }

    [Column("SKUName")]
    [StringLength(440)]
    public string Skuname { get; set; } = null!;

    [Column("SKUDescription")]
    public string? Skudescription { get; set; }

    [Column("SKUPrice", TypeName = "decimal(18, 9)")]
    public decimal Skuprice { get; set; }

    [Required]
    [Column("SKUEnabled")]
    public bool? Skuenabled { get; set; }

    [Column("SKUDepartmentID")]
    public int? SkudepartmentId { get; set; }

    [Column("SKUManufacturerID")]
    public int? SkumanufacturerId { get; set; }

    [Column("SKUInternalStatusID")]
    public int? SkuinternalStatusId { get; set; }

    [Column("SKUPublicStatusID")]
    public int? SkupublicStatusId { get; set; }

    [Column("SKUSupplierID")]
    public int? SkusupplierId { get; set; }

    [Column("SKUAvailableInDays")]
    public int? SkuavailableInDays { get; set; }

    [Column("SKUGUID")]
    public Guid Skuguid { get; set; }

    [Column("SKUImagePath")]
    [StringLength(450)]
    public string? SkuimagePath { get; set; }

    [Column("SKUWeight")]
    public double? Skuweight { get; set; }

    [Column("SKUWidth")]
    public double? Skuwidth { get; set; }

    [Column("SKUDepth")]
    public double? Skudepth { get; set; }

    [Column("SKUHeight")]
    public double? Skuheight { get; set; }

    [Column("SKUAvailableItems")]
    public int? SkuavailableItems { get; set; }

    [Column("SKUSellOnlyAvailable")]
    public bool? SkusellOnlyAvailable { get; set; }

    [Column("SKUCustomData")]
    public string? SkucustomData { get; set; }

    [Column("SKUOptionCategoryID")]
    public int? SkuoptionCategoryId { get; set; }

    [Column("SKUOrder")]
    public int? Skuorder { get; set; }

    [Column("SKULastModified")]
    public DateTime SkulastModified { get; set; }

    [Column("SKUCreated")]
    public DateTime? Skucreated { get; set; }

    [Column("SKUSiteID")]
    public int? SkusiteId { get; set; }

    [Column("SKUNeedsShipping")]
    public bool? SkuneedsShipping { get; set; }

    [Column("SKUValidUntil")]
    public DateTime? SkuvalidUntil { get; set; }

    [Column("SKUProductType")]
    [StringLength(50)]
    public string? SkuproductType { get; set; }

    [Column("SKUMaxItemsInOrder")]
    public int? SkumaxItemsInOrder { get; set; }

    [Column("SKUValidity")]
    [StringLength(50)]
    public string? Skuvalidity { get; set; }

    [Column("SKUValidFor")]
    public int? SkuvalidFor { get; set; }

    [Column("SKUMembershipGUID")]
    public Guid? SkumembershipGuid { get; set; }

    [Column("SKUBundleInventoryType")]
    [StringLength(50)]
    public string? SkubundleInventoryType { get; set; }

    [Column("SKUMinItemsInOrder")]
    public int? SkuminItemsInOrder { get; set; }

    [Column("SKURetailPrice", TypeName = "decimal(18, 9)")]
    public decimal? SkuretailPrice { get; set; }

    [Column("SKUParentSKUID")]
    public int? SkuparentSkuid { get; set; }

    [Column("SKUShortDescription")]
    public string? SkushortDescription { get; set; }

    [Column("SKUEproductFilesCount")]
    public int? SkueproductFilesCount { get; set; }

    [Column("SKUBundleItemsCount")]
    public int? SkubundleItemsCount { get; set; }

    [Column("SKUInStoreFrom")]
    public DateTime? SkuinStoreFrom { get; set; }

    [Column("SKUReorderAt")]
    public int? SkureorderAt { get; set; }

    [Column("SKUTrackInventory")]
    [StringLength(50)]
    public string? SkutrackInventory { get; set; }

    [Column("SKUTaxClassID")]
    public int? SkutaxClassId { get; set; }

    [Column("SKUBrandID")]
    public int? SkubrandId { get; set; }

    [Column("SKUCollectionID")]
    public int? SkucollectionId { get; set; }

    [InverseProperty("NodeSku")]
    public virtual ICollection<CmsTree> CmsTrees { get; set; } = new List<CmsTree>();

    [InverseProperty("MultiBuyDiscountApplyToSku")]
    public virtual ICollection<ComMultiBuyDiscount> ComMultiBuyDiscounts { get; set; } = new List<ComMultiBuyDiscount>();

    [InverseProperty("OrderItemSku")]
    public virtual ICollection<ComOrderItem> ComOrderItems { get; set; } = new List<ComOrderItem>();

    [InverseProperty("Sku")]
    public virtual ICollection<ComShoppingCartSku> ComShoppingCartSkus { get; set; } = new List<ComShoppingCartSku>();

    [InverseProperty("FileSku")]
    public virtual ICollection<ComSkufile> ComSkufiles { get; set; } = new List<ComSkufile>();

    [InverseProperty("Sku")]
    public virtual ICollection<ComSkuoptionCategory> ComSkuoptionCategories { get; set; } = new List<ComSkuoptionCategory>();

    [InverseProperty("VolumeDiscountSku")]
    public virtual ICollection<ComVolumeDiscount> ComVolumeDiscounts { get; set; } = new List<ComVolumeDiscount>();

    [InverseProperty("Sku")]
    public virtual ICollection<ComWishlist> ComWishlists { get; set; } = new List<ComWishlist>();

    [InverseProperty("SkuparentSku")]
    public virtual ICollection<ComSku> InverseSkuparentSku { get; set; } = new List<ComSku>();

    [ForeignKey("SkubrandId")]
    [InverseProperty("ComSkus")]
    public virtual ComBrand? Skubrand { get; set; }

    [ForeignKey("SkucollectionId")]
    [InverseProperty("ComSkus")]
    public virtual ComCollection? Skucollection { get; set; }

    [ForeignKey("SkudepartmentId")]
    [InverseProperty("ComSkus")]
    public virtual ComDepartment? Skudepartment { get; set; }

    [ForeignKey("SkuinternalStatusId")]
    [InverseProperty("ComSkus")]
    public virtual ComInternalStatus? SkuinternalStatus { get; set; }

    [ForeignKey("SkumanufacturerId")]
    [InverseProperty("ComSkus")]
    public virtual ComManufacturer? Skumanufacturer { get; set; }

    [ForeignKey("SkuoptionCategoryId")]
    [InverseProperty("ComSkus")]
    public virtual ComOptionCategory? SkuoptionCategory { get; set; }

    [ForeignKey("SkuparentSkuid")]
    [InverseProperty("InverseSkuparentSku")]
    public virtual ComSku? SkuparentSku { get; set; }

    [ForeignKey("SkupublicStatusId")]
    [InverseProperty("ComSkus")]
    public virtual ComPublicStatus? SkupublicStatus { get; set; }

    [ForeignKey("SkusiteId")]
    [InverseProperty("ComSkus")]
    public virtual CmsSite? Skusite { get; set; }

    [ForeignKey("SkusupplierId")]
    [InverseProperty("ComSkus")]
    public virtual ComSupplier? Skusupplier { get; set; }

    [ForeignKey("SkutaxClassId")]
    [InverseProperty("ComSkus")]
    public virtual ComTaxClass? SkutaxClass { get; set; }

    [ForeignKey("Skuid")]
    [InverseProperty("Skus")]
    public virtual ICollection<ComSku> Bundles { get; set; } = new List<ComSku>();

    [ForeignKey("Skuid")]
    [InverseProperty("Skus")]
    public virtual ICollection<ComMultiBuyDiscount> MultiBuyDiscounts { get; set; } = new List<ComMultiBuyDiscount>();

    [ForeignKey("Skuid")]
    [InverseProperty("SkusNavigation")]
    public virtual ICollection<ComSku> OptionSkus { get; set; } = new List<ComSku>();

    [ForeignKey("VariantSkuid")]
    [InverseProperty("VariantSkus")]
    public virtual ICollection<ComSku> OptionSkusNavigation { get; set; } = new List<ComSku>();

    [ForeignKey("BundleId")]
    [InverseProperty("Bundles")]
    public virtual ICollection<ComSku> Skus { get; set; } = new List<ComSku>();

    [ForeignKey("OptionSkuid")]
    [InverseProperty("OptionSkus")]
    public virtual ICollection<ComSku> SkusNavigation { get; set; } = new List<ComSku>();

    [ForeignKey("OptionSkuid")]
    [InverseProperty("OptionSkusNavigation")]
    public virtual ICollection<ComSku> VariantSkus { get; set; } = new List<ComSku>();
}
