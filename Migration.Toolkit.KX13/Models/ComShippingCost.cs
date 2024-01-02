using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_ShippingCost")]
[Index("ShippingCostShippingOptionId", Name = "IX_COM_ShippingCost_ShippingCostShippingOptionID")]
public partial class ComShippingCost
{
    [Key]
    [Column("ShippingCostID")]
    public int ShippingCostId { get; set; }

    [Column("ShippingCostShippingOptionID")]
    public int ShippingCostShippingOptionId { get; set; }

    public double ShippingCostMinWeight { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal ShippingCostValue { get; set; }

    [Column("ShippingCostGUID")]
    public Guid ShippingCostGuid { get; set; }

    public DateTime ShippingCostLastModified { get; set; }

    [ForeignKey("ShippingCostShippingOptionId")]
    [InverseProperty("ComShippingCosts")]
    public virtual ComShippingOption ShippingCostShippingOption { get; set; } = null!;
}
