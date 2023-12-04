using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Newsletter_EmailTemplate")]
[Index("TemplateSiteId", "TemplateName", Name = "IX_Newsletter_EmailTemplate_TemplateSiteID_TemplateName", IsUnique = true)]
public partial class NewsletterEmailTemplate
{
    [Key]
    [Column("TemplateID")]
    public int TemplateId { get; set; }

    [StringLength(250)]
    public string TemplateDisplayName { get; set; } = null!;

    [StringLength(250)]
    public string TemplateName { get; set; } = null!;

    [Column("TemplateSiteID")]
    public int TemplateSiteId { get; set; }

    [StringLength(50)]
    public string TemplateType { get; set; } = null!;

    [Column("TemplateGUID")]
    public Guid TemplateGuid { get; set; }

    public DateTime TemplateLastModified { get; set; }

    [StringLength(450)]
    public string? TemplateSubject { get; set; }

    [Column("TemplateThumbnailGUID")]
    public Guid? TemplateThumbnailGuid { get; set; }

    public string? TemplateDescription { get; set; }

    [StringLength(200)]
    public string? TemplateIconClass { get; set; }

    public string? TemplateCode { get; set; }

    [Column("TemplateInlineCSS")]
    public bool TemplateInlineCss { get; set; }

    [InverseProperty("Template")]
    public virtual ICollection<NewsletterEmailWidgetTemplate> NewsletterEmailWidgetTemplates { get; set; } = new List<NewsletterEmailWidgetTemplate>();

    [InverseProperty("IssueTemplate")]
    public virtual ICollection<NewsletterNewsletterIssue> NewsletterNewsletterIssues { get; set; } = new List<NewsletterNewsletterIssue>();

    [InverseProperty("NewsletterOptInTemplate")]
    public virtual ICollection<NewsletterNewsletter> NewsletterNewsletterNewsletterOptInTemplates { get; set; } = new List<NewsletterNewsletter>();

    [InverseProperty("NewsletterUnsubscriptionTemplate")]
    public virtual ICollection<NewsletterNewsletter> NewsletterNewsletterNewsletterUnsubscriptionTemplates { get; set; } = new List<NewsletterNewsletter>();

    [ForeignKey("TemplateSiteId")]
    [InverseProperty("NewsletterEmailTemplates")]
    public virtual CmsSite TemplateSite { get; set; } = null!;

    [ForeignKey("TemplateId")]
    [InverseProperty("Templates")]
    public virtual ICollection<NewsletterNewsletter> Newsletters { get; set; } = new List<NewsletterNewsletter>();
}
