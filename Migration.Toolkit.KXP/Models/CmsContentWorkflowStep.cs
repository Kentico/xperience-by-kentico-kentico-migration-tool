using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models;

[Table("CMS_ContentWorkflowStep")]
[Index("ContentWorkflowStepName", Name = "IX_CMS_ContentWorkflowStep_ContentWorkflowStepName", IsUnique = true)]
[Index("ContentWorkflowStepWorkflowId", Name = "IX_CMS_ContentWorkflowStep_ContentWorkflowStepWorkflowID")]
public partial class CmsContentWorkflowStep
{
    [Key]
    [Column("ContentWorkflowStepID")]
    public int ContentWorkflowStepId { get; set; }

    [Column("ContentWorkflowStepGUID")]
    public Guid ContentWorkflowStepGuid { get; set; }

    [Column("ContentWorkflowStepWorkflowID")]
    public int ContentWorkflowStepWorkflowId { get; set; }

    public DateTime ContentWorkflowStepLastModified { get; set; }

    [StringLength(200)]
    public string ContentWorkflowStepName { get; set; } = null!;

    [StringLength(200)]
    public string ContentWorkflowStepDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string ContentWorkflowStepIconClass { get; set; } = null!;

    public int ContentWorkflowStepOrder { get; set; }

    public int ContentWorkflowStepType { get; set; }

    [InverseProperty("ContentItemLanguageMetadataContentWorkflowStep")]
    public virtual ICollection<CmsContentItemLanguageMetadatum> CmsContentItemLanguageMetadata { get; set; } = new List<CmsContentItemLanguageMetadatum>();

    [InverseProperty("ContentWorkflowStepRoleContentWorkflowStep")]
    public virtual ICollection<CmsContentWorkflowStepRole> CmsContentWorkflowStepRoles { get; set; } = new List<CmsContentWorkflowStepRole>();

    [ForeignKey("ContentWorkflowStepWorkflowId")]
    [InverseProperty("CmsContentWorkflowSteps")]
    public virtual CmsContentWorkflow ContentWorkflowStepWorkflow { get; set; } = null!;
}