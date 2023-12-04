using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Newsletter_OpenedEmail")]
[Index("OpenedEmailIssueId", Name = "IX_Newsletter_OpenedEmail_OpenedEmailIssueID")]
public partial class NewsletterOpenedEmail
{
    [Key]
    [Column("OpenedEmailID")]
    public int OpenedEmailId { get; set; }

    [StringLength(254)]
    public string OpenedEmailEmail { get; set; } = null!;

    public Guid OpenedEmailGuid { get; set; }

    public DateTime? OpenedEmailTime { get; set; }

    [Column("OpenedEmailIssueID")]
    public int OpenedEmailIssueId { get; set; }

    [ForeignKey("OpenedEmailIssueId")]
    [InverseProperty("NewsletterOpenedEmails")]
    public virtual NewsletterNewsletterIssue OpenedEmailIssue { get; set; } = null!;
}
