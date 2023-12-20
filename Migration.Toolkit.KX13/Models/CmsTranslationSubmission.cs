using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_TranslationSubmission")]
[Index("SubmissionServiceId", Name = "IX_CMS_TranslationSubmission_SubmissionServiceID")]
[Index("SubmissionSubmittedByUserId", Name = "IX_CMS_TranslationSubmission_SubmissionSubmittedByUserID")]
public partial class CmsTranslationSubmission
{
    [Key]
    [Column("SubmissionID")]
    public int SubmissionId { get; set; }

    [StringLength(200)]
    public string SubmissionName { get; set; } = null!;

    [StringLength(200)]
    public string? SubmissionTicket { get; set; }

    public int SubmissionStatus { get; set; }

    [Column("SubmissionServiceID")]
    public int SubmissionServiceId { get; set; }

    [StringLength(50)]
    public string SubmissionSourceCulture { get; set; } = null!;

    public string SubmissionTargetCulture { get; set; } = null!;

    public int SubmissionPriority { get; set; }

    public DateTime? SubmissionDeadline { get; set; }

    [StringLength(500)]
    public string? SubmissionInstructions { get; set; }

    public DateTime SubmissionLastModified { get; set; }

    [Column("SubmissionGUID")]
    public Guid SubmissionGuid { get; set; }

    [Column("SubmissionSiteID")]
    public int? SubmissionSiteId { get; set; }

    public double? SubmissionPrice { get; set; }

    public string? SubmissionStatusMessage { get; set; }

    public bool? SubmissionTranslateAttachments { get; set; }

    public int SubmissionItemCount { get; set; }

    public DateTime SubmissionDate { get; set; }

    public int? SubmissionWordCount { get; set; }

    public int? SubmissionCharCount { get; set; }

    [Column("SubmissionSubmittedByUserID")]
    public int? SubmissionSubmittedByUserId { get; set; }

    [InverseProperty("SubmissionItemSubmission")]
    public virtual ICollection<CmsTranslationSubmissionItem> CmsTranslationSubmissionItems { get; set; } = new List<CmsTranslationSubmissionItem>();

    [ForeignKey("SubmissionServiceId")]
    [InverseProperty("CmsTranslationSubmissions")]
    public virtual CmsTranslationService SubmissionService { get; set; } = null!;

    [ForeignKey("SubmissionSubmittedByUserId")]
    [InverseProperty("CmsTranslationSubmissions")]
    public virtual CmsUser? SubmissionSubmittedByUser { get; set; }
}
