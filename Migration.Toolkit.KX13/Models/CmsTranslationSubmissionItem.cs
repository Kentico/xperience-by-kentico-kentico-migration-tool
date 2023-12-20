using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_TranslationSubmissionItem")]
[Index("SubmissionItemSubmissionId", Name = "IX_CMS_TranslationSubmissionItem_SubmissionItemSubmissionID")]
public partial class CmsTranslationSubmissionItem
{
    [Key]
    [Column("SubmissionItemID")]
    public int SubmissionItemId { get; set; }

    [Column("SubmissionItemSubmissionID")]
    public int SubmissionItemSubmissionId { get; set; }

    [Column("SubmissionItemSourceXLIFF")]
    public string? SubmissionItemSourceXliff { get; set; }

    [Column("SubmissionItemTargetXLIFF")]
    public string? SubmissionItemTargetXliff { get; set; }

    [StringLength(100)]
    public string SubmissionItemObjectType { get; set; } = null!;

    [Column("SubmissionItemObjectID")]
    public int SubmissionItemObjectId { get; set; }

    [Column("SubmissionItemGUID")]
    public Guid SubmissionItemGuid { get; set; }

    public DateTime SubmissionItemLastModified { get; set; }

    [StringLength(200)]
    public string SubmissionItemName { get; set; } = null!;

    public int? SubmissionItemWordCount { get; set; }

    public int? SubmissionItemCharCount { get; set; }

    public string? SubmissionItemCustomData { get; set; }

    [Column("SubmissionItemTargetObjectID")]
    public int SubmissionItemTargetObjectId { get; set; }

    [StringLength(50)]
    public string? SubmissionItemType { get; set; }

    [StringLength(50)]
    public string? SubmissionItemTargetCulture { get; set; }

    [ForeignKey("SubmissionItemSubmissionId")]
    [InverseProperty("CmsTranslationSubmissionItems")]
    public virtual CmsTranslationSubmission SubmissionItemSubmission { get; set; } = null!;
}
