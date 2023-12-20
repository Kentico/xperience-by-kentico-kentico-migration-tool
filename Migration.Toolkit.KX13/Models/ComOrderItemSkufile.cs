using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_OrderItemSKUFile")]
[Index("FileId", Name = "IX_COM_OrderItemSKUFile_FileID")]
[Index("OrderItemId", Name = "IX_COM_OrderItemSKUFile_OrderItemID")]
public partial class ComOrderItemSkufile
{
    [Key]
    [Column("OrderItemSKUFileID")]
    public int OrderItemSkufileId { get; set; }

    public Guid Token { get; set; }

    [Column("OrderItemID")]
    public int OrderItemId { get; set; }

    [Column("FileID")]
    public int FileId { get; set; }

    [ForeignKey("FileId")]
    [InverseProperty("ComOrderItemSkufiles")]
    public virtual ComSkufile File { get; set; } = null!;

    [ForeignKey("OrderItemId")]
    [InverseProperty("ComOrderItemSkufiles")]
    public virtual ComOrderItem OrderItem { get; set; } = null!;
}
