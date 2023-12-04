using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_ShoppingCartSKU")]
[Index("Skuid", Name = "IX_COM_ShoppingCartSKU_SKUID")]
[Index("ShoppingCartId", Name = "IX_COM_ShoppingCartSKU_ShoppingCartID")]
public partial class ComShoppingCartSku
{
    [Key]
    [Column("CartItemID")]
    public int CartItemId { get; set; }

    [Column("ShoppingCartID")]
    public int ShoppingCartId { get; set; }

    [Column("SKUID")]
    public int Skuid { get; set; }

    [Column("SKUUnits")]
    public int Skuunits { get; set; }

    public string? CartItemCustomData { get; set; }

    public Guid? CartItemGuid { get; set; }

    public Guid? CartItemParentGuid { get; set; }

    public DateTime? CartItemValidTo { get; set; }

    [Column("CartItemBundleGUID")]
    public Guid? CartItemBundleGuid { get; set; }

    public string? CartItemText { get; set; }

    public int? CartItemAutoAddedUnits { get; set; }

    [ForeignKey("ShoppingCartId")]
    [InverseProperty("ComShoppingCartSkus")]
    public virtual ComShoppingCart ShoppingCart { get; set; } = null!;

    [ForeignKey("Skuid")]
    [InverseProperty("ComShoppingCartSkus")]
    public virtual ComSku Sku { get; set; } = null!;
}
