using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("Forums_ForumSubscription")]
[Index("SubscriptionForumId", Name = "IX_Forums_ForumSubscription_SubscriptionForumID")]
[Index("SubscriptionPostId", Name = "IX_Forums_ForumSubscription_SubscriptionPostID")]
[Index("SubscriptionUserId", Name = "IX_Forums_ForumSubscription_SubscriptionUserID")]
public partial class ForumsForumSubscription
{
    [Key]
    [Column("SubscriptionID")]
    public int SubscriptionId { get; set; }

    [Column("SubscriptionUserID")]
    public int? SubscriptionUserId { get; set; }

    [StringLength(254)]
    public string? SubscriptionEmail { get; set; }

    [Column("SubscriptionForumID")]
    public int SubscriptionForumId { get; set; }

    [Column("SubscriptionPostID")]
    public int? SubscriptionPostId { get; set; }

    [Column("SubscriptionGUID")]
    public Guid SubscriptionGuid { get; set; }

    public DateTime SubscriptionLastModified { get; set; }

    public bool? SubscriptionApproved { get; set; }

    [StringLength(100)]
    public string? SubscriptionApprovalHash { get; set; }

    [ForeignKey("SubscriptionForumId")]
    [InverseProperty("ForumsForumSubscriptions")]
    public virtual ForumsForum SubscriptionForum { get; set; } = null!;

    [ForeignKey("SubscriptionPostId")]
    [InverseProperty("ForumsForumSubscriptions")]
    public virtual ForumsForumPost? SubscriptionPost { get; set; }

    [ForeignKey("SubscriptionUserId")]
    [InverseProperty("ForumsForumSubscriptions")]
    public virtual CmsUser? SubscriptionUser { get; set; }
}