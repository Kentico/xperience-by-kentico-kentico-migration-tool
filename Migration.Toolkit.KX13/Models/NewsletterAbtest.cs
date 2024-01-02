using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Newsletter_ABTest")]
[Index("TestIssueId", Name = "IX_Newsletter_ABTest_TestIssueID", IsUnique = true)]
[Index("TestWinnerIssueId", Name = "IX_Newsletter_ABTest_TestWinnerIssueID")]
[Index("TestWinnerScheduledTaskId", Name = "IX_Newsletter_ABTest_TestWinnerScheduledTaskID")]
public partial class NewsletterAbtest
{
    [Key]
    [Column("TestID")]
    public int TestId { get; set; }

    [Column("TestIssueID")]
    public int TestIssueId { get; set; }

    public int TestWinnerOption { get; set; }

    public int? TestSelectWinnerAfter { get; set; }

    [Column("TestWinnerIssueID")]
    public int? TestWinnerIssueId { get; set; }

    public DateTime? TestWinnerSelected { get; set; }

    public DateTime TestLastModified { get; set; }

    [Column("TestGUID")]
    public Guid TestGuid { get; set; }

    [Column("TestWinnerScheduledTaskID")]
    public int? TestWinnerScheduledTaskId { get; set; }

    public int TestSizePercentage { get; set; }

    public int? TestNumberPerVariantEmails { get; set; }

    [ForeignKey("TestIssueId")]
    [InverseProperty("NewsletterAbtestTestIssue")]
    public virtual NewsletterNewsletterIssue TestIssue { get; set; } = null!;

    [ForeignKey("TestWinnerIssueId")]
    [InverseProperty("NewsletterAbtestTestWinnerIssues")]
    public virtual NewsletterNewsletterIssue? TestWinnerIssue { get; set; }

    [ForeignKey("TestWinnerScheduledTaskId")]
    [InverseProperty("NewsletterAbtests")]
    public virtual CmsScheduledTask? TestWinnerScheduledTask { get; set; }
}
