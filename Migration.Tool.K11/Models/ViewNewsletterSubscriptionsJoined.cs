using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Keyless]
public class ViewNewsletterSubscriptionsJoined
{
    [Column("SubscriberID")]
    public int SubscriberId { get; set; }

    [StringLength(440)]
    public string? SubscriberFullName { get; set; }

    [StringLength(254)]
    public string? SubscriberEmail { get; set; }

    public bool? SubscriptionApproved { get; set; }

    [Column("NewsletterID")]
    public int NewsletterId { get; set; }

    [StringLength(100)]
    public string? SubscriberType { get; set; }

    public int? SubscriberBounces { get; set; }

    [StringLength(250)]
    public string NewsletterDisplayName { get; set; } = null!;

    [Column("SubscriberRelatedID")]
    public int SubscriberRelatedId { get; set; }

    [Column("SubscriberNewsletterID")]
    public int SubscriberNewsletterId { get; set; }
}
