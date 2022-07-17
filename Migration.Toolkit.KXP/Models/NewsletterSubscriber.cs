namespace Migration.Toolkit.KXP.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("Newsletter_Subscriber")]
    [Index("SubscriberEmail", Name = "IX_Newsletter_Subscriber_SubscriberEmail")]
    [Index("SubscriberType", "SubscriberRelatedId", Name = "IX_Newsletter_Subscriber_SubscriberType_SubscriberRelatedID")]
    public partial class NewsletterSubscriber
    {
        public NewsletterSubscriber()
        {
            NewsletterEmails = new HashSet<NewsletterEmail>();
            NewsletterSubscriberNewsletters = new HashSet<NewsletterSubscriberNewsletter>();
        }

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

        [ForeignKey("SubscriberSiteId")]
        [InverseProperty("NewsletterSubscribers")]
        public virtual CmsSite SubscriberSite { get; set; } = null!;
        [InverseProperty("EmailSubscriber")]
        public virtual ICollection<NewsletterEmail> NewsletterEmails { get; set; }
        [InverseProperty("Subscriber")]
        public virtual ICollection<NewsletterSubscriberNewsletter> NewsletterSubscriberNewsletters { get; set; }
    }
}
