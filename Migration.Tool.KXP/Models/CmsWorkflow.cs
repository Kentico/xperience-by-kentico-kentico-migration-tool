using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.KXP.Models;

[Table("CMS_Workflow")]
public class CmsWorkflow
{
    [Key]
    [Column("WorkflowID")]
    public int WorkflowId { get; set; }

    public string WorkflowDisplayName { get; set; } = null!;

    [StringLength(450)]
    public string WorkflowName { get; set; } = null!;

    [Column("WorkflowGUID")]
    public Guid WorkflowGuid { get; set; }

    public DateTime WorkflowLastModified { get; set; }

    public bool? WorkflowAutoPublishChanges { get; set; }

    public bool? WorkflowUseCheckinCheckout { get; set; }

    public int? WorkflowType { get; set; }

    public bool? WorkflowSendEmails { get; set; }

    public bool? WorkflowSendApproveEmails { get; set; }

    public bool? WorkflowSendRejectEmails { get; set; }

    public bool? WorkflowSendPublishEmails { get; set; }

    public bool? WorkflowSendArchiveEmails { get; set; }

    [StringLength(200)]
    public string? WorkflowApprovedTemplateName { get; set; }

    [StringLength(200)]
    public string? WorkflowRejectedTemplateName { get; set; }

    [StringLength(200)]
    public string? WorkflowPublishedTemplateName { get; set; }

    [StringLength(200)]
    public string? WorkflowArchivedTemplateName { get; set; }

    public bool? WorkflowSendReadyForApprovalEmails { get; set; }

    [StringLength(200)]
    public string? WorkflowReadyForApprovalTemplateName { get; set; }

    [StringLength(200)]
    public string? WorkflowNotificationTemplateName { get; set; }

    public string? WorkflowAllowedObjects { get; set; }

    public int? WorkflowRecurrenceType { get; set; }

    public bool WorkflowEnabled { get; set; }

    [InverseProperty("HistoryWorkflow")]
    public virtual ICollection<CmsAutomationHistory> CmsAutomationHistories { get; set; } = new List<CmsAutomationHistory>();

    [InverseProperty("StateWorkflow")]
    public virtual ICollection<CmsAutomationState> CmsAutomationStates { get; set; } = new List<CmsAutomationState>();

    [InverseProperty("TriggerWorkflow")]
    public virtual ICollection<CmsObjectWorkflowTrigger> CmsObjectWorkflowTriggers { get; set; } = new List<CmsObjectWorkflowTrigger>();

    [InverseProperty("StepWorkflow")]
    public virtual ICollection<CmsWorkflowStep> CmsWorkflowSteps { get; set; } = new List<CmsWorkflowStep>();

    [InverseProperty("TransitionWorkflow")]
    public virtual ICollection<CmsWorkflowTransition> CmsWorkflowTransitions { get; set; } = new List<CmsWorkflowTransition>();
}
