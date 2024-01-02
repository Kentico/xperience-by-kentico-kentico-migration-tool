using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("EmailLibrary_EmailTemplate")]
    public partial class EmailLibraryEmailTemplate
    {
        public EmailLibraryEmailTemplate()
        {
            EmailLibraryEmailTemplateContentTypes = new HashSet<EmailLibraryEmailTemplateContentType>();
        }

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
        public virtual ICollection<EmailLibraryEmailTemplateContentType> EmailLibraryEmailTemplateContentTypes { get; set; }
    }
}
