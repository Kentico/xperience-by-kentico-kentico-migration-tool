using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models;

[Table("CMS_Class")]
[Index("ClassName", Name = "IX_CMS_Class_ClassName", IsUnique = true)]
[Index("ClassName", "ClassGuid", Name = "IX_CMS_Class_ClassName_ClassGUID")]
[Index("ClassResourceId", Name = "IX_CMS_Class_ClassResourceID")]
public partial class CmsClass
{
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

    [StringLength(100)]
    public string? ClassShortName { get; set; }

    [ForeignKey("ClassResourceId")]
    [InverseProperty("CmsClasses")]
    public virtual CmsResource? ClassResource { get; set; }

    [InverseProperty("FormClass")]
    public virtual ICollection<CmsAlternativeForm> CmsAlternativeFormFormClasses { get; set; } = new List<CmsAlternativeForm>();

    [InverseProperty("FormCoupledClass")]
    public virtual ICollection<CmsAlternativeForm> CmsAlternativeFormFormCoupledClasses { get; set; } = new List<CmsAlternativeForm>();

    [InverseProperty("ContentItemContentType")]
    public virtual ICollection<CmsContentItem> CmsContentItems { get; set; } = new List<CmsContentItem>();

    [InverseProperty("ContentTypeChannelContentType")]
    public virtual ICollection<CmsContentTypeChannel> CmsContentTypeChannels { get; set; } = new List<CmsContentTypeChannel>();

    [InverseProperty("ContentWorkflowContentTypeContentType")]
    public virtual ICollection<CmsContentWorkflowContentType> CmsContentWorkflowContentTypes { get; set; } = new List<CmsContentWorkflowContentType>();

    [InverseProperty("FormClass")]
    public virtual ICollection<CmsForm> CmsForms { get; set; } = new List<CmsForm>();

    [InverseProperty("Class")]
    public virtual ICollection<CmsQuery> CmsQueries { get; set; } = new List<CmsQuery>();

    [InverseProperty("EmailTemplateContentTypeContentType")]
    public virtual ICollection<EmailLibraryEmailTemplateContentType> EmailLibraryEmailTemplateContentTypes { get; set; } = new List<EmailLibraryEmailTemplateContentType>();
}
