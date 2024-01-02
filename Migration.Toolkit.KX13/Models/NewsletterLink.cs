using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Newsletter_Link")]
[Index("LinkIssueId", Name = "IX_Newsletter_Link_LinkIssueID")]
public partial class NewsletterLink
{
    [Key]
    [Column("LinkID")]
    public int LinkId { get; set; }

    [Column("LinkIssueID")]
    public int LinkIssueId { get; set; }

    public string LinkTarget { get; set; } = null!;

    [StringLength(450)]
    public string LinkDescription { get; set; } = null!;

    [Column("LinkGUID")]
    public Guid LinkGuid { get; set; }

    [ForeignKey("LinkIssueId")]
    [InverseProperty("NewsletterLinks")]
    public virtual NewsletterNewsletterIssue LinkIssue { get; set; } = null!;

    [InverseProperty("ClickedLinkNewsletterLink")]
    public virtual ICollection<NewsletterClickedLink> NewsletterClickedLinks { get; set; } = new List<NewsletterClickedLink>();
}
