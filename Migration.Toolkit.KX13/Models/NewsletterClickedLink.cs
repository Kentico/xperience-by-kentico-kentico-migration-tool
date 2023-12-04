using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Newsletter_ClickedLink")]
[Index("ClickedLinkNewsletterLinkId", Name = "IX_Newsletter_ClickedLink_ClickedLinkNewsletterLinkID")]
public partial class NewsletterClickedLink
{
    [Key]
    [Column("ClickedLinkID")]
    public int ClickedLinkId { get; set; }

    public Guid ClickedLinkGuid { get; set; }

    [StringLength(254)]
    public string ClickedLinkEmail { get; set; } = null!;

    [Column("ClickedLinkNewsletterLinkID")]
    public int ClickedLinkNewsletterLinkId { get; set; }

    public DateTime? ClickedLinkTime { get; set; }

    [ForeignKey("ClickedLinkNewsletterLinkId")]
    [InverseProperty("NewsletterClickedLinks")]
    public virtual NewsletterLink ClickedLinkNewsletterLink { get; set; } = null!;
}
