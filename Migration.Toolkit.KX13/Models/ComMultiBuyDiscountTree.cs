using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[PrimaryKey("MultiBuyDiscountId", "NodeId")]
[Table("COM_MultiBuyDiscountTree")]
[Index("NodeId", Name = "IX_COM_MultiBuyDiscountTree_NodeID")]
public partial class ComMultiBuyDiscountTree
{
    [Key]
    [Column("MultiBuyDiscountID")]
    public int MultiBuyDiscountId { get; set; }

    [Key]
    [Column("NodeID")]
    public int NodeId { get; set; }

    [Required]
    public bool? NodeIncluded { get; set; }

    [ForeignKey("MultiBuyDiscountId")]
    [InverseProperty("ComMultiBuyDiscountTrees")]
    public virtual ComMultiBuyDiscount MultiBuyDiscount { get; set; } = null!;

    [ForeignKey("NodeId")]
    [InverseProperty("ComMultiBuyDiscountTrees")]
    public virtual CmsTree Node { get; set; } = null!;
}
