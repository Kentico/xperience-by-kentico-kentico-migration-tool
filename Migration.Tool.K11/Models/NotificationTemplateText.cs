using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("Notification_TemplateText")]
[Index("GatewayId", Name = "IX_Notification_TemplateText_GatewayID")]
[Index("TemplateId", Name = "IX_Notification_TemplateText_TemplateID")]
public class NotificationTemplateText
{
    [Key]
    [Column("TemplateTextID")]
    public int TemplateTextId { get; set; }

    [Column("TemplateID")]
    public int TemplateId { get; set; }

    [Column("GatewayID")]
    public int GatewayId { get; set; }

    [StringLength(250)]
    public string TemplateSubject { get; set; } = null!;

    [Column("TemplateHTMLText")]
    public string TemplateHtmltext { get; set; } = null!;

    public string TemplatePlainText { get; set; } = null!;

    [Column("TemplateTextGUID")]
    public Guid TemplateTextGuid { get; set; }

    public DateTime TemplateTextLastModified { get; set; }

    [ForeignKey("GatewayId")]
    [InverseProperty("NotificationTemplateTexts")]
    public virtual NotificationGateway Gateway { get; set; } = null!;

    [ForeignKey("TemplateId")]
    [InverseProperty("NotificationTemplateTexts")]
    public virtual NotificationTemplate Template { get; set; } = null!;
}
