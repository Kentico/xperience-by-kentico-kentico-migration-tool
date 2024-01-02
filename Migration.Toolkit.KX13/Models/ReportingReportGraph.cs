using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Reporting_ReportGraph")]
[Index("GraphGuid", Name = "IX_Reporting_ReportGraph_GraphGUID", IsUnique = true)]
[Index("GraphReportId", "GraphName", Name = "IX_Reporting_ReportGraph_GraphReportID_GraphName", IsUnique = true)]
public partial class ReportingReportGraph
{
    [Key]
    [Column("GraphID")]
    public int GraphId { get; set; }

    [StringLength(100)]
    public string GraphName { get; set; } = null!;

    [StringLength(450)]
    public string GraphDisplayName { get; set; } = null!;

    public string GraphQuery { get; set; } = null!;

    public bool GraphQueryIsStoredProcedure { get; set; }

    [StringLength(50)]
    public string GraphType { get; set; } = null!;

    [Column("GraphReportID")]
    public int GraphReportId { get; set; }

    [StringLength(200)]
    public string? GraphTitle { get; set; }

    [Column("GraphXAxisTitle")]
    [StringLength(200)]
    public string? GraphXaxisTitle { get; set; }

    [Column("GraphYAxisTitle")]
    [StringLength(200)]
    public string? GraphYaxisTitle { get; set; }

    public int? GraphWidth { get; set; }

    public int? GraphHeight { get; set; }

    public int? GraphLegendPosition { get; set; }

    public string? GraphSettings { get; set; }

    [Column("GraphGUID")]
    public Guid GraphGuid { get; set; }

    public DateTime GraphLastModified { get; set; }

    public bool? GraphIsHtml { get; set; }

    [StringLength(100)]
    public string? GraphConnectionString { get; set; }

    [ForeignKey("GraphReportId")]
    [InverseProperty("ReportingReportGraphs")]
    public virtual ReportingReport GraphReport { get; set; } = null!;

    [InverseProperty("ReportSubscriptionGraph")]
    public virtual ICollection<ReportingReportSubscription> ReportingReportSubscriptions { get; set; } = new List<ReportingReportSubscription>();
}
