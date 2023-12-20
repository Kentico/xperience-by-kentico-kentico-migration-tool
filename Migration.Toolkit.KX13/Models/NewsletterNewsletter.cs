using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Newsletter_Newsletter")]
[Index("NewsletterDynamicScheduledTaskId", Name = "IX_Newsletter_Newsletter_NewsletterDynamicScheduledTaskID")]
[Index("NewsletterOptInTemplateId", Name = "IX_Newsletter_Newsletter_NewsletterOptInTemplateID")]
[Index("NewsletterSiteId", "NewsletterName", Name = "IX_Newsletter_Newsletter_NewsletterSiteID_NewsletterName", IsUnique = true)]
[Index("NewsletterSubscriptionTemplateId", Name = "IX_Newsletter_Newsletter_NewsletterSubscriptionTemplateID")]
[Index("NewsletterUnsubscriptionTemplateId", Name = "IX_Newsletter_Newsletter_NewsletterUnsubscriptionTemplateID")]
public partial class NewsletterNewsletter
{
    [Key]
    [Column("NewsletterID")]
    public int NewsletterId { get; set; }

    [StringLength(250)]
    public string NewsletterDisplayName { get; set; } = null!;

    [StringLength(250)]
    public string NewsletterName { get; set; } = null!;

    [Column("NewsletterSubscriptionTemplateID")]
    public int? NewsletterSubscriptionTemplateId { get; set; }

    [Column("NewsletterUnsubscriptionTemplateID")]
    public int NewsletterUnsubscriptionTemplateId { get; set; }

    [StringLength(200)]
    public string NewsletterSenderName { get; set; } = null!;

    [StringLength(254)]
    public string NewsletterSenderEmail { get; set; } = null!;

    [StringLength(100)]
    public string? NewsletterDynamicSubject { get; set; }

    [Column("NewsletterDynamicURL")]
    [StringLength(500)]
    public string? NewsletterDynamicUrl { get; set; }

    [Column("NewsletterDynamicScheduledTaskID")]
    public int? NewsletterDynamicScheduledTaskId { get; set; }

    [Column("NewsletterSiteID")]
    public int NewsletterSiteId { get; set; }

    [Column("NewsletterGUID")]
    public Guid NewsletterGuid { get; set; }

    [StringLength(1000)]
    public string? NewsletterUnsubscribeUrl { get; set; }

    [StringLength(500)]
    public string? NewsletterBaseUrl { get; set; }

    public DateTime NewsletterLastModified { get; set; }

    public bool? NewsletterEnableOptIn { get; set; }

    [Column("NewsletterOptInTemplateID")]
    public int? NewsletterOptInTemplateId { get; set; }

    public bool? NewsletterSendOptInConfirmation { get; set; }

    [Column("NewsletterOptInApprovalURL")]
    [StringLength(450)]
    public string? NewsletterOptInApprovalUrl { get; set; }

    public bool? NewsletterTrackOpenEmails { get; set; }

    public bool? NewsletterTrackClickedLinks { get; set; }

    [StringLength(998)]
    public string? NewsletterDraftEmails { get; set; }

    public bool? NewsletterLogActivity { get; set; }

    [StringLength(5)]
    public string NewsletterSource { get; set; } = null!;

    public int NewsletterType { get; set; }

    [ForeignKey("NewsletterDynamicScheduledTaskId")]
    [InverseProperty("NewsletterNewsletters")]
    public virtual CmsScheduledTask? NewsletterDynamicScheduledTask { get; set; }

    [InverseProperty("IssueNewsletter")]
    public virtual ICollection<NewsletterNewsletterIssue> NewsletterNewsletterIssues { get; set; } = new List<NewsletterNewsletterIssue>();

    [ForeignKey("NewsletterOptInTemplateId")]
    [InverseProperty("NewsletterNewsletterNewsletterOptInTemplates")]
    public virtual NewsletterEmailTemplate? NewsletterOptInTemplate { get; set; }

    [ForeignKey("NewsletterSiteId")]
    [InverseProperty("NewsletterNewsletters")]
    public virtual CmsSite NewsletterSite { get; set; } = null!;

    [InverseProperty("Newsletter")]
    public virtual ICollection<NewsletterSubscriberNewsletter> NewsletterSubscriberNewsletters { get; set; } = new List<NewsletterSubscriberNewsletter>();

    [ForeignKey("NewsletterUnsubscriptionTemplateId")]
    [InverseProperty("NewsletterNewsletterNewsletterUnsubscriptionTemplates")]
    public virtual NewsletterEmailTemplate NewsletterUnsubscriptionTemplate { get; set; } = null!;

    [InverseProperty("UnsubscriptionNewsletter")]
    public virtual ICollection<NewsletterUnsubscription> NewsletterUnsubscriptions { get; set; } = new List<NewsletterUnsubscription>();

    [ForeignKey("NewsletterId")]
    [InverseProperty("Newsletters")]
    public virtual ICollection<NewsletterEmailTemplate> Templates { get; set; } = new List<NewsletterEmailTemplate>();
}
