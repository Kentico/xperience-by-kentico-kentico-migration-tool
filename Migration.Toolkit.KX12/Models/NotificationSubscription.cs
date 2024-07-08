using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("Notification_Subscription")]
[Index("SubscriptionEventSource", "SubscriptionEventCode", "SubscriptionEventObjectId", Name = "IX_Notification_Subscription_SubscriptionEventSource_SubscriptionEventCode_SubscriptionEventObjectID")]
[Index("SubscriptionGatewayId", Name = "IX_Notification_Subscription_SubscriptionGatewayID")]
[Index("SubscriptionSiteId", Name = "IX_Notification_Subscription_SubscriptionSiteID")]
[Index("SubscriptionTemplateId", Name = "IX_Notification_Subscription_SubscriptionTemplateID")]
[Index("SubscriptionUserId", Name = "IX_Notification_Subscription_SubscriptionUserID")]
public partial class NotificationSubscription
{
    [Key]
    [Column("SubscriptionID")]
    public int SubscriptionId { get; set; }

    [Column("SubscriptionGatewayID")]
    public int SubscriptionGatewayId { get; set; }

    [Column("SubscriptionTemplateID")]
    public int SubscriptionTemplateId { get; set; }

    [StringLength(100)]
    public string? SubscriptionEventSource { get; set; }

    [StringLength(100)]
    public string? SubscriptionEventCode { get; set; }

    [StringLength(250)]
    public string SubscriptionEventDisplayName { get; set; } = null!;

    [Column("SubscriptionEventObjectID")]
    public int? SubscriptionEventObjectId { get; set; }

    public DateTime SubscriptionTime { get; set; }

    [Column("SubscriptionUserID")]
    public int SubscriptionUserId { get; set; }

    [StringLength(250)]
    public string SubscriptionTarget { get; set; } = null!;

    public DateTime SubscriptionLastModified { get; set; }

    [Column("SubscriptionGUID")]
    public Guid SubscriptionGuid { get; set; }

    public string? SubscriptionEventData1 { get; set; }

    public string? SubscriptionEventData2 { get; set; }

    [Column("SubscriptionUseHTML")]
    public bool? SubscriptionUseHtml { get; set; }

    [Column("SubscriptionSiteID")]
    public int? SubscriptionSiteId { get; set; }

    [ForeignKey("SubscriptionGatewayId")]
    [InverseProperty("NotificationSubscriptions")]
    public virtual NotificationGateway SubscriptionGateway { get; set; } = null!;

    [ForeignKey("SubscriptionSiteId")]
    [InverseProperty("NotificationSubscriptions")]
    public virtual CmsSite? SubscriptionSite { get; set; }

    [ForeignKey("SubscriptionTemplateId")]
    [InverseProperty("NotificationSubscriptions")]
    public virtual NotificationTemplate SubscriptionTemplate { get; set; } = null!;

    [ForeignKey("SubscriptionUserId")]
    [InverseProperty("NotificationSubscriptions")]
    public virtual CmsUser SubscriptionUser { get; set; } = null!;
}