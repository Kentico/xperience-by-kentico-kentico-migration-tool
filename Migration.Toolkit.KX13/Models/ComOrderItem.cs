using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_OrderItem")]
[Index("OrderItemOrderId", Name = "IX_COM_OrderItem_OrderItemOrderID")]
[Index("OrderItemSkuid", Name = "IX_COM_OrderItem_OrderItemSKUID")]
public partial class ComOrderItem
{
    [Key]
    [Column("OrderItemID")]
    public int OrderItemId { get; set; }

    [Column("OrderItemOrderID")]
    public int OrderItemOrderId { get; set; }

    [Column("OrderItemSKUID")]
    public int OrderItemSkuid { get; set; }

    [Column("OrderItemSKUName")]
    [StringLength(450)]
    public string OrderItemSkuname { get; set; } = null!;

    [Column(TypeName = "decimal(18, 9)")]
    public decimal OrderItemUnitPrice { get; set; }

    public int OrderItemUnitCount { get; set; }

    public string? OrderItemCustomData { get; set; }

    public Guid OrderItemGuid { get; set; }

    public Guid? OrderItemParentGuid { get; set; }

    public DateTime OrderItemLastModified { get; set; }

    public DateTime? OrderItemValidTo { get; set; }

    [Column("OrderItemBundleGUID")]
    public Guid? OrderItemBundleGuid { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal? OrderItemTotalPriceInMainCurrency { get; set; }

    public bool? OrderItemSendNotification { get; set; }

    public string? OrderItemText { get; set; }

    public string? OrderItemProductDiscounts { get; set; }

    public string? OrderItemDiscountSummary { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal OrderItemTotalPrice { get; set; }

    [InverseProperty("OrderItem")]
    public virtual ICollection<ComOrderItemSkufile> ComOrderItemSkufiles { get; set; } = new List<ComOrderItemSkufile>();

    [ForeignKey("OrderItemOrderId")]
    [InverseProperty("ComOrderItems")]
    public virtual ComOrder OrderItemOrder { get; set; } = null!;

    [ForeignKey("OrderItemSkuid")]
    [InverseProperty("ComOrderItems")]
    public virtual ComSku OrderItemSku { get; set; } = null!;
}
