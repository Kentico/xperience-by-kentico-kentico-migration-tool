using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Board_Subscription")]
[Index("SubscriptionBoardId", Name = "IX_Board_Subscription_SubscriptionBoardID")]
[Index("SubscriptionUserId", Name = "IX_Board_Subscription_SubscriptionUserID")]
public partial class BoardSubscription
{
    [Key]
    [Column("SubscriptionID")]
    public int SubscriptionId { get; set; }

    [Column("SubscriptionBoardID")]
    public int SubscriptionBoardId { get; set; }

    [Column("SubscriptionUserID")]
    public int? SubscriptionUserId { get; set; }

    [StringLength(254)]
    public string SubscriptionEmail { get; set; } = null!;

    public DateTime SubscriptionLastModified { get; set; }

    [Column("SubscriptionGUID")]
    public Guid SubscriptionGuid { get; set; }

    public bool? SubscriptionApproved { get; set; }

    [StringLength(100)]
    public string? SubscriptionApprovalHash { get; set; }

    [ForeignKey("SubscriptionBoardId")]
    [InverseProperty("BoardSubscriptions")]
    public virtual BoardBoard SubscriptionBoard { get; set; } = null!;

    [ForeignKey("SubscriptionUserId")]
    [InverseProperty("BoardSubscriptions")]
    public virtual CmsUser? SubscriptionUser { get; set; }
}