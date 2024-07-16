using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KXP.Models;

[Table("EmailLibrary_EmailTemplate")]
public partial class EmailLibraryEmailTemplate
{
    [Key]
    [Column("EmailTemplateID")]
    public int EmailTemplateId { get; set; }

    [StringLength(250)]
    public string EmailTemplateName { get; set; } = null!;

    [StringLength(250)]
    public string EmailTemplateDisplayName { get; set; } = null!;

    public string? EmailTemplateDescription { get; set; }

    public string EmailTemplateCode { get; set; } = null!;

    public Guid EmailTemplateGuid { get; set; }

    public DateTime EmailTemplateLastModified { get; set; }

    [InverseProperty("EmailTemplateContentTypeEmailTemplate")]
    public virtual ICollection<EmailLibraryEmailTemplateContentType> EmailLibraryEmailTemplateContentTypes { get; set; } = new List<EmailLibraryEmailTemplateContentType>();
}
