using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_Class")]
    [Index("ClassName", Name = "IX_CMS_Class_ClassName", IsUnique = true)]
    [Index("ClassName", "ClassGuid", Name = "IX_CMS_Class_ClassName_ClassGUID")]
    [Index("ClassResourceId", Name = "IX_CMS_Class_ClassResourceID")]
    public partial class CmsClass
    {
        public CmsClass()
        {
            CmsAlternativeFormFormClasses = new HashSet<CmsAlternativeForm>();
            CmsAlternativeFormFormCoupledClasses = new HashSet<CmsAlternativeForm>();
            CmsContentItems = new HashSet<CmsContentItem>();
            CmsContentTypeChannels = new HashSet<CmsContentTypeChannel>();
            CmsForms = new HashSet<CmsForm>();
            CmsQueries = new HashSet<CmsQuery>();
            EmailLibraryEmailTemplateContentTypes = new HashSet<EmailLibraryEmailTemplateContentType>();
        }

        [Key]
        [Column("ClassID")]
        public int ClassId { get; set; }
        [StringLength(100)]
        public string ClassDisplayName { get; set; } = null!;
        [StringLength(100)]
        public string ClassName { get; set; } = null!;
        public string ClassXmlSchema { get; set; } = null!;
        public string ClassFormDefinition { get; set; } = null!;
        [StringLength(100)]
        public string? ClassTableName { get; set; }
        public bool? ClassShowTemplateSelection { get; set; }
        public DateTime ClassLastModified { get; set; }
        [Column("ClassGUID")]
        public Guid ClassGuid { get; set; }
        public string? ClassContactMapping { get; set; }
        public bool? ClassContactOverwriteEnabled { get; set; }
        [StringLength(100)]
        public string? ClassConnectionString { get; set; }
        [StringLength(100)]
        public string? ClassDefaultObjectType { get; set; }
        [Column("ClassResourceID")]
        public int? ClassResourceId { get; set; }
        public string? ClassCodeGenerationSettings { get; set; }
        [StringLength(200)]
        public string? ClassIconClass { get; set; }
        public bool ClassHasUnmanagedDbSchema { get; set; }
        [StringLength(10)]
        public string ClassType { get; set; } = null!;
        [StringLength(10)]
        public string? ClassContentTypeType { get; set; }
        public bool? ClassWebPageHasUrl { get; set; }

        [ForeignKey("ClassResourceId")]
        [InverseProperty("CmsClasses")]
        public virtual CmsResource? ClassResource { get; set; }
        [InverseProperty("FormClass")]
        public virtual ICollection<CmsAlternativeForm> CmsAlternativeFormFormClasses { get; set; }
        [InverseProperty("FormCoupledClass")]
        public virtual ICollection<CmsAlternativeForm> CmsAlternativeFormFormCoupledClasses { get; set; }
        [InverseProperty("ContentItemContentType")]
        public virtual ICollection<CmsContentItem> CmsContentItems { get; set; }
        [InverseProperty("ContentTypeChannelContentType")]
        public virtual ICollection<CmsContentTypeChannel> CmsContentTypeChannels { get; set; }
        [InverseProperty("FormClass")]
        public virtual ICollection<CmsForm> CmsForms { get; set; }
        [InverseProperty("Class")]
        public virtual ICollection<CmsQuery> CmsQueries { get; set; }
        [InverseProperty("EmailTemplateContentTypeContentType")]
        public virtual ICollection<EmailLibraryEmailTemplateContentType> EmailLibraryEmailTemplateContentTypes { get; set; }
    }
}
