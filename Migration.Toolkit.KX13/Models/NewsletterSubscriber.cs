using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Newsletter_Subscriber")]
[Index("SubscriberEmail", Name = "IX_Newsletter_Subscriber_SubscriberEmail")]
[Index("SubscriberType", "SubscriberRelatedId", Name = "IX_Newsletter_Subscriber_SubscriberType_SubscriberRelatedID")]
public partial class NewsletterSubscriber
{
    [Key]
    [Column("SubscriberID")]
    public int SubscriberId { get; set; }

    [StringLength(254)]
    public string? SubscriberEmail { get; set; }

    [StringLength(200)]
    public string? SubscriberFirstName { get; set; }

    [StringLength(200)]
    public string? SubscriberLastName { get; set; }

    [Column("SubscriberSiteID")]
    public int SubscriberSiteId { get; set; }

    [Column("SubscriberGUID")]
    public Guid SubscriberGuid { get; set; }

    public string? SubscriberCustomData { get; set; }

    [StringLength(100)]
    public string? SubscriberType { get; set; }

    [Column("SubscriberRelatedID")]
    public int SubscriberRelatedId { get; set; }

    public DateTime SubscriberLastModified { get; set; }

    [StringLength(440)]
    public string? SubscriberFullName { get; set; }

    public int? SubscriberBounces { get; set; }

    [InverseProperty("EmailSubscriber")]
    public virtual ICollection<NewsletterEmail> NewsletterEmails { get; set; } = new List<NewsletterEmail>();

    [InverseProperty("Subscriber")]
    public virtual ICollection<NewsletterSubscriberNewsletter> NewsletterSubscriberNewsletters { get; set; } = new List<NewsletterSubscriberNewsletter>();

    [ForeignKey("SubscriberSiteId")]
    [InverseProperty("NewsletterSubscribers")]
    public virtual CmsSite SubscriberSite { get; set; } = null!;
}
