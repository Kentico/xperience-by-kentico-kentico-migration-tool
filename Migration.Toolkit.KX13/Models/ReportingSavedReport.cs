using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Reporting_SavedReport")]
[Index("SavedReportCreatedByUserId", Name = "IX_Reporting_SavedReport_SavedReportCreatedByUserID")]
public partial class ReportingSavedReport
{
    [Key]
    [Column("SavedReportID")]
    public int SavedReportId { get; set; }

    [Column("SavedReportReportID")]
    public int SavedReportReportId { get; set; }

    [Column("SavedReportGUID")]
    public Guid SavedReportGuid { get; set; }

    [StringLength(200)]
    public string? SavedReportTitle { get; set; }

    public DateTime SavedReportDate { get; set; }

    [Column("SavedReportHTML")]
    public string SavedReportHtml { get; set; } = null!;

    public string SavedReportParameters { get; set; } = null!;

    [Column("SavedReportCreatedByUserID")]
    public int? SavedReportCreatedByUserId { get; set; }

    public DateTime SavedReportLastModified { get; set; }

    [InverseProperty("SavedGraphSavedReport")]
    public virtual ICollection<ReportingSavedGraph> ReportingSavedGraphs { get; set; } = new List<ReportingSavedGraph>();

    [ForeignKey("SavedReportCreatedByUserId")]
    [InverseProperty("ReportingSavedReports")]
    public virtual CmsUser? SavedReportCreatedByUser { get; set; }

    [ForeignKey("SavedReportReportId")]
    [InverseProperty("ReportingSavedReports")]
    public virtual ReportingReport SavedReportReport { get; set; } = null!;
}
