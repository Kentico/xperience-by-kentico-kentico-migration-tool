using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Reporting_SavedGraph")]
[Index("SavedGraphGuid", Name = "IX_Reporting_SavedGraph_SavedGraphGUID")]
[Index("SavedGraphSavedReportId", Name = "IX_Reporting_SavedGraph_SavedGraphSavedReportID")]
public partial class ReportingSavedGraph
{
    [Key]
    [Column("SavedGraphID")]
    public int SavedGraphId { get; set; }

    [Column("SavedGraphSavedReportID")]
    public int SavedGraphSavedReportId { get; set; }

    [Column("SavedGraphGUID")]
    public Guid SavedGraphGuid { get; set; }

    public byte[] SavedGraphBinary { get; set; } = null!;

    [StringLength(100)]
    public string SavedGraphMimeType { get; set; } = null!;

    public DateTime SavedGraphLastModified { get; set; }

    [ForeignKey("SavedGraphSavedReportId")]
    [InverseProperty("ReportingSavedGraphs")]
    public virtual ReportingSavedReport SavedGraphSavedReport { get; set; } = null!;
}
