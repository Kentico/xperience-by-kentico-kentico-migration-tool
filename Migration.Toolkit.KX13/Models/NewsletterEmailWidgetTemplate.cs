using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Newsletter_EmailWidgetTemplate")]
[Index("EmailWidgetId", Name = "IX_Newsletter_EmailWidgetTemplate_EmailWidgetID")]
[Index("TemplateId", Name = "IX_Newsletter_EmailWidgetTemplate_TemplateID")]
public partial class NewsletterEmailWidgetTemplate
{
    [Key]
    [Column("EmailWidgetTemplateID")]
    public int EmailWidgetTemplateId { get; set; }

    [Column("EmailWidgetID")]
    public int EmailWidgetId { get; set; }

    [Column("TemplateID")]
    public int TemplateId { get; set; }

    [ForeignKey("EmailWidgetId")]
    [InverseProperty("NewsletterEmailWidgetTemplates")]
    public virtual NewsletterEmailWidget EmailWidget { get; set; } = null!;

    [ForeignKey("TemplateId")]
    [InverseProperty("NewsletterEmailWidgetTemplates")]
    public virtual NewsletterEmailTemplate Template { get; set; } = null!;
}
