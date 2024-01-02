using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Reporting_Report")]
[Index("ReportCategoryId", Name = "IX_Reporting_Report_ReportCategoryID")]
[Index("ReportGuid", "ReportName", Name = "IX_Reporting_Report_ReportGUID_ReportName")]
[Index("ReportName", Name = "IX_Reporting_Report_ReportName", IsUnique = true)]
public partial class ReportingReport
{
    [Key]
    [Column("ReportID")]
    public int ReportId { get; set; }

    [StringLength(200)]
    public string ReportName { get; set; } = null!;

    [StringLength(440)]
    public string ReportDisplayName { get; set; } = null!;

    public string? ReportLayout { get; set; }

    public string? ReportParameters { get; set; }

    [Column("ReportCategoryID")]
    public int ReportCategoryId { get; set; }

    public int ReportAccess { get; set; }

    [Column("ReportGUID")]
    public Guid ReportGuid { get; set; }

    public DateTime ReportLastModified { get; set; }

    public bool? ReportEnableSubscription { get; set; }

    [StringLength(100)]
    public string? ReportConnectionString { get; set; }

    [ForeignKey("ReportCategoryId")]
    [InverseProperty("ReportingReports")]
    public virtual ReportingReportCategory ReportCategory { get; set; } = null!;

    [InverseProperty("GraphReport")]
    public virtual ICollection<ReportingReportGraph> ReportingReportGraphs { get; set; } = new List<ReportingReportGraph>();

    [InverseProperty("ReportSubscriptionReport")]
    public virtual ICollection<ReportingReportSubscription> ReportingReportSubscriptions { get; set; } = new List<ReportingReportSubscription>();

    [InverseProperty("TableReport")]
    public virtual ICollection<ReportingReportTable> ReportingReportTables { get; set; } = new List<ReportingReportTable>();

    [InverseProperty("ValueReport")]
    public virtual ICollection<ReportingReportValue> ReportingReportValues { get; set; } = new List<ReportingReportValue>();

    [InverseProperty("SavedReportReport")]
    public virtual ICollection<ReportingSavedReport> ReportingSavedReports { get; set; } = new List<ReportingSavedReport>();
}
