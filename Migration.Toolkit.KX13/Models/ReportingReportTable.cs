using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Reporting_ReportTable")]
[Index("TableReportId", Name = "IX_Reporting_ReportTable_TableReportID")]
[Index("TableName", "TableReportId", Name = "IX_Reporting_ReportTable_TableReportID_TableName", IsUnique = true)]
public partial class ReportingReportTable
{
    [Key]
    [Column("TableID")]
    public int TableId { get; set; }

    [StringLength(100)]
    public string TableName { get; set; } = null!;

    [StringLength(450)]
    public string TableDisplayName { get; set; } = null!;

    public string TableQuery { get; set; } = null!;

    public bool TableQueryIsStoredProcedure { get; set; }

    [Column("TableReportID")]
    public int TableReportId { get; set; }

    public string? TableSettings { get; set; }

    [Column("TableGUID")]
    public Guid TableGuid { get; set; }

    public DateTime TableLastModified { get; set; }

    [StringLength(100)]
    public string? TableConnectionString { get; set; }

    [InverseProperty("ReportSubscriptionTable")]
    public virtual ICollection<ReportingReportSubscription> ReportingReportSubscriptions { get; set; } = new List<ReportingReportSubscription>();

    [ForeignKey("TableReportId")]
    [InverseProperty("ReportingReportTables")]
    public virtual ReportingReport TableReport { get; set; } = null!;
}
