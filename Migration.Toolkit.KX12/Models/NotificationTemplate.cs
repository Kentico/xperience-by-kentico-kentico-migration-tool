using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("Notification_Template")]
[Index("TemplateSiteId", Name = "IX_Notification_Template_TemplateSiteID")]
public partial class NotificationTemplate
{
    [Key]
    [Column("TemplateID")]
    public int TemplateId { get; set; }

    [StringLength(200)]
    public string TemplateName { get; set; } = null!;

    [StringLength(200)]
    public string TemplateDisplayName { get; set; } = null!;

    [Column("TemplateSiteID")]
    public int? TemplateSiteId { get; set; }

    public DateTime TemplateLastModified { get; set; }

    [Column("TemplateGUID")]
    public Guid TemplateGuid { get; set; }

    [InverseProperty("SubscriptionTemplate")]
    public virtual ICollection<NotificationSubscription> NotificationSubscriptions { get; set; } = new List<NotificationSubscription>();

    [InverseProperty("Template")]
    public virtual ICollection<NotificationTemplateText> NotificationTemplateTexts { get; set; } = new List<NotificationTemplateText>();

    [ForeignKey("TemplateSiteId")]
    [InverseProperty("NotificationTemplates")]
    public virtual CmsSite? TemplateSite { get; set; }
}