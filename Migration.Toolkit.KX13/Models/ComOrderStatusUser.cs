using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_OrderStatusUser")]
[Index("ChangedByUserId", Name = "IX_COM_OrderStatusUser_ChangedByUserID")]
[Index("FromStatusId", Name = "IX_COM_OrderStatusUser_FromStatusID")]
[Index("OrderId", "Date", Name = "IX_COM_OrderStatusUser_OrderID_Date")]
[Index("ToStatusId", Name = "IX_COM_OrderStatusUser_ToStatusID")]
public partial class ComOrderStatusUser
{
    [Key]
    [Column("OrderStatusUserID")]
    public int OrderStatusUserId { get; set; }

    [Column("OrderID")]
    public int OrderId { get; set; }

    [Column("FromStatusID")]
    public int? FromStatusId { get; set; }

    [Column("ToStatusID")]
    public int ToStatusId { get; set; }

    [Column("ChangedByUserID")]
    public int? ChangedByUserId { get; set; }

    public DateTime Date { get; set; }

    public string? Note { get; set; }

    [ForeignKey("ChangedByUserId")]
    [InverseProperty("ComOrderStatusUsers")]
    public virtual CmsUser? ChangedByUser { get; set; }

    [ForeignKey("FromStatusId")]
    [InverseProperty("ComOrderStatusUserFromStatuses")]
    public virtual ComOrderStatus? FromStatus { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("ComOrderStatusUsers")]
    public virtual ComOrder Order { get; set; } = null!;

    [ForeignKey("ToStatusId")]
    [InverseProperty("ComOrderStatusUserToStatuses")]
    public virtual ComOrderStatus ToStatus { get; set; } = null!;
}
