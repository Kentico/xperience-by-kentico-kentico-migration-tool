using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Notification_Gateway")]
public partial class NotificationGateway
{
    [Key]
    [Column("GatewayID")]
    public int GatewayId { get; set; }

    [StringLength(200)]
    public string GatewayName { get; set; } = null!;

    [StringLength(200)]
    public string GatewayDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string GatewayAssemblyName { get; set; } = null!;

    [StringLength(200)]
    public string GatewayClassName { get; set; } = null!;

    public string? GatewayDescription { get; set; }

    public bool? GatewaySupportsEmail { get; set; }

    public bool? GatewaySupportsPlainText { get; set; }

    [Column("GatewaySupportsHTMLText")]
    public bool? GatewaySupportsHtmltext { get; set; }

    public DateTime GatewayLastModified { get; set; }

    [Column("GatewayGUID")]
    public Guid GatewayGuid { get; set; }

    public bool? GatewayEnabled { get; set; }

    [InverseProperty("SubscriptionGateway")]
    public virtual ICollection<NotificationSubscription> NotificationSubscriptions { get; set; } = new List<NotificationSubscription>();

    [InverseProperty("Gateway")]
    public virtual ICollection<NotificationTemplateText> NotificationTemplateTexts { get; set; } = new List<NotificationTemplateText>();
}
