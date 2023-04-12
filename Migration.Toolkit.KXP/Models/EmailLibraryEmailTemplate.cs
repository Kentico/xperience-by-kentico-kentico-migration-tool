using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("EmailLibrary_EmailTemplate")]
    [Index("EmailTemplateSiteId", Name = "IX_EmailLibrary_EmailTemplate_EmailTemplateSiteID")]
    public partial class EmailLibraryEmailTemplate
    {
        public EmailLibraryEmailTemplate()
        {
            EmailLibraryEmailConfigurations = new HashSet<EmailLibraryEmailConfiguration>();
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
        [Column("EmailTemplateSiteID")]
        public int EmailTemplateSiteId { get; set; }
        public Guid EmailTemplateGuid { get; set; }
        public DateTime EmailTemplateLastModified { get; set; }
        [StringLength(50)]
        public string EmailTemplateType { get; set; } = null!;

        [ForeignKey("EmailTemplateSiteId")]
        [InverseProperty("EmailLibraryEmailTemplates")]
        public virtual CmsSite EmailTemplateSite { get; set; } = null!;
        [InverseProperty("EmailConfigurationEmailTemplate")]
        public virtual ICollection<EmailLibraryEmailConfiguration> EmailLibraryEmailConfigurations { get; set; }
    }
}
