using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_TranslationService")]
public partial class CmsTranslationService
{
    [Key]
    [Column("TranslationServiceID")]
    public int TranslationServiceId { get; set; }

    [StringLength(200)]
    public string TranslationServiceAssemblyName { get; set; } = null!;

    [StringLength(200)]
    public string TranslationServiceClassName { get; set; } = null!;

    [StringLength(200)]
    public string TranslationServiceName { get; set; } = null!;

    [StringLength(200)]
    public string TranslationServiceDisplayName { get; set; } = null!;

    public bool TranslationServiceIsMachine { get; set; }

    public DateTime TranslationServiceLastModified { get; set; }

    [Column("TranslationServiceGUID")]
    public Guid TranslationServiceGuid { get; set; }

    public bool TranslationServiceEnabled { get; set; }

    public bool? TranslationServiceSupportsInstructions { get; set; }

    public bool? TranslationServiceSupportsPriority { get; set; }

    public bool? TranslationServiceSupportsDeadline { get; set; }

    [StringLength(1000)]
    public string? TranslationServiceParameter { get; set; }

    public bool? TranslationServiceSupportsStatusUpdate { get; set; }

    public bool? TranslationServiceSupportsCancel { get; set; }

    [InverseProperty("SubmissionService")]
    public virtual ICollection<CmsTranslationSubmission> CmsTranslationSubmissions { get; set; } = new List<CmsTranslationSubmission>();
}
