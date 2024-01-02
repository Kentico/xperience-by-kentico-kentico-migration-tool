using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("EmailLibrary_EmailTemplateContentType")]
    [Index("EmailTemplateContentTypeContentTypeId", Name = "IX_EmailLibrary_EmailTemplateContentType_EmailTemplateContentTypeContentTypeID")]
    [Index("EmailTemplateContentTypeEmailTemplateId", Name = "IX_EmailLibrary_EmailTemplateContentType_EmailTemplateContentTypeEmailTemplateID")]
    public partial class EmailLibraryEmailTemplateContentType
    {
        [Key]
        [Column("EmailTemplateContentTypeID")]
        public int EmailTemplateContentTypeId { get; set; }
        [Column("EmailTemplateContentTypeContentTypeID")]
        public int EmailTemplateContentTypeContentTypeId { get; set; }
        [Column("EmailTemplateContentTypeEmailTemplateID")]
        public int EmailTemplateContentTypeEmailTemplateId { get; set; }

        [ForeignKey("EmailTemplateContentTypeContentTypeId")]
        [InverseProperty("EmailLibraryEmailTemplateContentTypes")]
        public virtual CmsClass EmailTemplateContentTypeContentType { get; set; } = null!;
        [ForeignKey("EmailTemplateContentTypeEmailTemplateId")]
        [InverseProperty("EmailLibraryEmailTemplateContentTypes")]
        public virtual EmailLibraryEmailTemplate EmailTemplateContentTypeEmailTemplate { get; set; } = null!;
    }
}
