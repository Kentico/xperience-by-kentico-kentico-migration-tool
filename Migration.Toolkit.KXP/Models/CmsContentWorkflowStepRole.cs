using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models;

[Table("CMS_ContentWorkflowStepRole")]
[Index("ContentWorkflowStepRoleContentWorkflowStepId", Name = "IX_CMS_ContentWorkflowStepRole_ContentWorkflowStepRoleContentWorkflowStepID")]
[Index("ContentWorkflowStepRoleRoleId", Name = "IX_CMS_ContentWorkflowStepRole_ContentWorkflowStepRoleRoleID")]
public partial class CmsContentWorkflowStepRole
{
    [Key]
    [Column("ContentWorkflowStepRoleID")]
    public int ContentWorkflowStepRoleId { get; set; }

    [Column("ContentWorkflowStepRoleContentWorkflowStepID")]
    public int ContentWorkflowStepRoleContentWorkflowStepId { get; set; }

    [Column("ContentWorkflowStepRoleRoleID")]
    public int ContentWorkflowStepRoleRoleId { get; set; }

    [ForeignKey("ContentWorkflowStepRoleContentWorkflowStepId")]
    [InverseProperty("CmsContentWorkflowStepRoles")]
    public virtual CmsContentWorkflowStep ContentWorkflowStepRoleContentWorkflowStep { get; set; } = null!;

    [ForeignKey("ContentWorkflowStepRoleRoleId")]
    [InverseProperty("CmsContentWorkflowStepRoles")]
    public virtual CmsRole ContentWorkflowStepRoleRole { get; set; } = null!;
}