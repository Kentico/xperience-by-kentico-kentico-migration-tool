using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_WorkflowStep")]
[Index("StepActionId", Name = "IX_CMS_WorkflowStep_StepActionID")]
[Index("StepId", "StepName", Name = "IX_CMS_WorkflowStep_StepID_StepName")]
[Index("StepWorkflowId", "StepName", Name = "IX_CMS_WorkflowStep_StepWorkflowID_StepName", IsUnique = true)]
[Index("StepWorkflowId", "StepOrder", Name = "IX_CMS_WorkflowStep_StepWorkflowID_StepOrder")]
public partial class CmsWorkflowStep
{
    [Key]
    [Column("StepID")]
    public int StepId { get; set; }

    [StringLength(450)]
    public string StepDisplayName { get; set; } = null!;

    [StringLength(440)]
    public string? StepName { get; set; }

    public int? StepOrder { get; set; }

    [Column("StepWorkflowID")]
    public int StepWorkflowId { get; set; }

    [Column("StepGUID")]
    public Guid StepGuid { get; set; }

    public DateTime StepLastModified { get; set; }

    public int? StepType { get; set; }

    public bool? StepAllowReject { get; set; }

    public string? StepDefinition { get; set; }

    public int? StepRolesSecurity { get; set; }

    public int? StepUsersSecurity { get; set; }

    [StringLength(200)]
    public string? StepApprovedTemplateName { get; set; }

    [StringLength(200)]
    public string? StepRejectedTemplateName { get; set; }

    [StringLength(200)]
    public string? StepReadyforApprovalTemplateName { get; set; }

    public bool? StepSendApproveEmails { get; set; }

    public bool? StepSendRejectEmails { get; set; }

    public bool? StepSendReadyForApprovalEmails { get; set; }

    public bool? StepSendEmails { get; set; }

    public bool? StepAllowPublish { get; set; }

    [Column("StepActionID")]
    public int? StepActionId { get; set; }

    public string? StepActionParameters { get; set; }

    public int? StepWorkflowType { get; set; }

    [InverseProperty("HistoryStep")]
    public virtual ICollection<CmsAutomationHistory> CmsAutomationHistoryHistorySteps { get; set; } = new List<CmsAutomationHistory>();

    [InverseProperty("HistoryTargetStep")]
    public virtual ICollection<CmsAutomationHistory> CmsAutomationHistoryHistoryTargetSteps { get; set; } = new List<CmsAutomationHistory>();

    [InverseProperty("StateStep")]
    public virtual ICollection<CmsAutomationState> CmsAutomationStates { get; set; } = new List<CmsAutomationState>();

    [InverseProperty("DocumentWorkflowStep")]
    public virtual ICollection<CmsDocument> CmsDocuments { get; set; } = new List<CmsDocument>();

    [InverseProperty("ObjectWorkflowStep")]
    public virtual ICollection<CmsObjectSetting> CmsObjectSettings { get; set; } = new List<CmsObjectSetting>();

    [InverseProperty("VersionWorkflowStep")]
    public virtual ICollection<CmsVersionHistory> CmsVersionHistories { get; set; } = new List<CmsVersionHistory>();

    [InverseProperty("Step")]
    public virtual ICollection<CmsWorkflowHistory> CmsWorkflowHistorySteps { get; set; } = new List<CmsWorkflowHistory>();

    [InverseProperty("TargetStep")]
    public virtual ICollection<CmsWorkflowHistory> CmsWorkflowHistoryTargetSteps { get; set; } = new List<CmsWorkflowHistory>();

    [InverseProperty("Step")]
    public virtual ICollection<CmsWorkflowStepRole> CmsWorkflowStepRoles { get; set; } = new List<CmsWorkflowStepRole>();

    [InverseProperty("Step")]
    public virtual ICollection<CmsWorkflowStepUser> CmsWorkflowStepUsers { get; set; } = new List<CmsWorkflowStepUser>();

    [InverseProperty("TransitionEndStep")]
    public virtual ICollection<CmsWorkflowTransition> CmsWorkflowTransitionTransitionEndSteps { get; set; } = new List<CmsWorkflowTransition>();

    [InverseProperty("TransitionStartStep")]
    public virtual ICollection<CmsWorkflowTransition> CmsWorkflowTransitionTransitionStartSteps { get; set; } = new List<CmsWorkflowTransition>();

    [ForeignKey("StepActionId")]
    [InverseProperty("CmsWorkflowSteps")]
    public virtual CmsWorkflowAction? StepAction { get; set; }

    [ForeignKey("StepWorkflowId")]
    [InverseProperty("CmsWorkflowSteps")]
    public virtual CmsWorkflow StepWorkflow { get; set; } = null!;
}
