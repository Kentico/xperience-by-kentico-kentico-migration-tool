using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Newsletter_SubscriberNewsletter")]
[Index("NewsletterId", "SubscriptionApproved", Name = "IX_Newsletter_SubscriberNewsletter_NewsletterID_SubscriptionApproved")]
[Index("SubscriberId", "NewsletterId", Name = "UQ_Newsletter_SubscriberNewsletter", IsUnique = true)]
public partial class NewsletterSubscriberNewsletter
{
    [Column("SubscriberID")]
    public int SubscriberId { get; set; }

    [Column("NewsletterID")]
    public int NewsletterId { get; set; }

    public DateTime SubscribedWhen { get; set; }

    public bool? SubscriptionApproved { get; set; }

    public DateTime? SubscriptionApprovedWhen { get; set; }

    [StringLength(100)]
    public string? SubscriptionApprovalHash { get; set; }

    [Key]
    [Column("SubscriberNewsletterID")]
    public int SubscriberNewsletterId { get; set; }

    [ForeignKey("NewsletterId")]
    [InverseProperty("NewsletterSubscriberNewsletters")]
    public virtual NewsletterNewsletter Newsletter { get; set; } = null!;

    [ForeignKey("SubscriberId")]
    [InverseProperty("NewsletterSubscriberNewsletters")]
    public virtual NewsletterSubscriber Subscriber { get; set; } = null!;
}
