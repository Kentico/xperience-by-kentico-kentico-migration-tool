using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.KXP.Models;

[Table("CMS_ContentWorkflow")]
public class CmsContentWorkflow
{
    [Key]
    [Column("ContentWorkflowID")]
    public int ContentWorkflowId { get; set; }

    [StringLength(200)]
    public string ContentWorkflowName { get; set; } = null!;

    [StringLength(200)]
    public string ContentWorkflowDisplayName { get; set; } = null!;

    [Column("ContentWorkflowGUID")]
    public Guid ContentWorkflowGuid { get; set; }

    public DateTime ContentWorkflowLastModified { get; set; }

    [InverseProperty("ContentWorkflowContentTypeContentWorkflow")]
    public virtual ICollection<CmsContentWorkflowContentType> CmsContentWorkflowContentTypes { get; set; } = new List<CmsContentWorkflowContentType>();

    [InverseProperty("ContentWorkflowStepWorkflow")]
    public virtual ICollection<CmsContentWorkflowStep> CmsContentWorkflowSteps { get; set; } = new List<CmsContentWorkflowStep>();
}
