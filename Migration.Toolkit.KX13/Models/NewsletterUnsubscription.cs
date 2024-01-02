using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Newsletter_Unsubscription")]
[Index("UnsubscriptionEmail", "UnsubscriptionNewsletterId", Name = "IX_Newsletter_Unsubscription_Email_NewsletterID")]
[Index("UnsubscriptionNewsletterId", Name = "IX_Newsletter_Unsubscription_NewsletterID")]
[Index("UnsubscriptionFromIssueId", Name = "IX_Newsletter_Unsubscription_UnsubscriptionFromIssueID")]
public partial class NewsletterUnsubscription
{
    [Key]
    [Column("UnsubscriptionID")]
    public int UnsubscriptionId { get; set; }

    [StringLength(254)]
    public string UnsubscriptionEmail { get; set; } = null!;

    public DateTime UnsubscriptionCreated { get; set; }

    [Column("UnsubscriptionNewsletterID")]
    public int? UnsubscriptionNewsletterId { get; set; }

    [Column("UnsubscriptionFromIssueID")]
    public int? UnsubscriptionFromIssueId { get; set; }

    [Column("UnsubscriptionGUID")]
    public Guid UnsubscriptionGuid { get; set; }

    [ForeignKey("UnsubscriptionFromIssueId")]
    [InverseProperty("NewsletterUnsubscriptions")]
    public virtual NewsletterNewsletterIssue? UnsubscriptionFromIssue { get; set; }

    [ForeignKey("UnsubscriptionNewsletterId")]
    [InverseProperty("NewsletterUnsubscriptions")]
    public virtual NewsletterNewsletter? UnsubscriptionNewsletter { get; set; }
}
