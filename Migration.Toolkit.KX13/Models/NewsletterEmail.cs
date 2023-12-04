using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Newsletter_Emails")]
[Index("EmailGuid", Name = "IX_Newsletter_Emails_EmailGUID", IsUnique = true)]
[Index("EmailNewsletterIssueId", Name = "IX_Newsletter_Emails_EmailNewsletterIssueID")]
[Index("EmailSending", Name = "IX_Newsletter_Emails_EmailSending")]
[Index("EmailSiteId", Name = "IX_Newsletter_Emails_EmailSiteID")]
[Index("EmailSubscriberId", Name = "IX_Newsletter_Emails_EmailSubscriberID")]
public partial class NewsletterEmail
{
    [Key]
    [Column("EmailID")]
    public int EmailId { get; set; }

    [Column("EmailNewsletterIssueID")]
    public int EmailNewsletterIssueId { get; set; }

    [Column("EmailSubscriberID")]
    public int? EmailSubscriberId { get; set; }

    [Column("EmailSiteID")]
    public int EmailSiteId { get; set; }

    public string? EmailLastSendResult { get; set; }

    public DateTime? EmailLastSendAttempt { get; set; }

    public bool? EmailSending { get; set; }

    [Column("EmailGUID")]
    public Guid EmailGuid { get; set; }

    [Column("EmailContactID")]
    public int? EmailContactId { get; set; }

    [StringLength(254)]
    public string? EmailAddress { get; set; }

    [ForeignKey("EmailNewsletterIssueId")]
    [InverseProperty("NewsletterEmails")]
    public virtual NewsletterNewsletterIssue EmailNewsletterIssue { get; set; } = null!;

    [ForeignKey("EmailSiteId")]
    [InverseProperty("NewsletterEmails")]
    public virtual CmsSite EmailSite { get; set; } = null!;

    [ForeignKey("EmailSubscriberId")]
    [InverseProperty("NewsletterEmails")]
    public virtual NewsletterSubscriber? EmailSubscriber { get; set; }
}
