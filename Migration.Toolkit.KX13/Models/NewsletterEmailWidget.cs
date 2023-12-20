using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Newsletter_EmailWidget")]
[Index("EmailWidgetSiteId", Name = "IX_Newsletter_EmailWidget_EmailWidgetSiteID")]
public partial class NewsletterEmailWidget
{
    [Key]
    [Column("EmailWidgetID")]
    public int EmailWidgetId { get; set; }

    public Guid EmailWidgetGuid { get; set; }

    public DateTime EmailWidgetLastModified { get; set; }

    [StringLength(250)]
    public string EmailWidgetDisplayName { get; set; } = null!;

    [StringLength(250)]
    public string EmailWidgetName { get; set; } = null!;

    public string? EmailWidgetDescription { get; set; }

    public string? EmailWidgetCode { get; set; }

    [Column("EmailWidgetSiteID")]
    public int EmailWidgetSiteId { get; set; }

    [Column("EmailWidgetThumbnailGUID")]
    public Guid? EmailWidgetThumbnailGuid { get; set; }

    [StringLength(200)]
    public string? EmailWidgetIconCssClass { get; set; }

    public string? EmailWidgetProperties { get; set; }

    [ForeignKey("EmailWidgetSiteId")]
    [InverseProperty("NewsletterEmailWidgets")]
    public virtual CmsSite EmailWidgetSite { get; set; } = null!;

    [InverseProperty("EmailWidget")]
    public virtual ICollection<NewsletterEmailWidgetTemplate> NewsletterEmailWidgetTemplates { get; set; } = new List<NewsletterEmailWidgetTemplate>();
}
