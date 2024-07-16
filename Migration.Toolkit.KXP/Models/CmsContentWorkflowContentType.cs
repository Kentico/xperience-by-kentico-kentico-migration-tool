using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models;

[Table("CMS_ContentWorkflowContentType")]
[Index("ContentWorkflowContentTypeContentTypeId", Name = "IX_CMS_ContentWorkflowContentType_ContentWorkflowContentTypeContentTypeID")]
[Index("ContentWorkflowContentTypeContentWorkflowId", Name = "IX_CMS_ContentWorkflowContentType_ContentWorkflowContentTypeContentWorkflowID")]
public class CmsContentWorkflowContentType
{
    [Key]
    [Column("ContentWorkflowContentTypeID")]
    public int ContentWorkflowContentTypeId { get; set; }

    [Column("ContentWorkflowContentTypeContentWorkflowID")]
    public int ContentWorkflowContentTypeContentWorkflowId { get; set; }

    [Column("ContentWorkflowContentTypeContentTypeID")]
    public int ContentWorkflowContentTypeContentTypeId { get; set; }

    [ForeignKey("ContentWorkflowContentTypeContentTypeId")]
    [InverseProperty("CmsContentWorkflowContentTypes")]
    public virtual CmsClass ContentWorkflowContentTypeContentType { get; set; } = null!;

    [ForeignKey("ContentWorkflowContentTypeContentWorkflowId")]
    [InverseProperty("CmsContentWorkflowContentTypes")]
    public virtual CmsContentWorkflow ContentWorkflowContentTypeContentWorkflow { get; set; } = null!;
}
