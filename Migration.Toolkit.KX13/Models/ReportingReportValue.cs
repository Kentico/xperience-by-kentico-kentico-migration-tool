using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Reporting_ReportValue")]
[Index("ValueName", "ValueReportId", Name = "IX_Reporting_ReportValue_ValueName_ValueReportID")]
[Index("ValueReportId", Name = "IX_Reporting_ReportValue_ValueReportID")]
public partial class ReportingReportValue
{
    [Key]
    [Column("ValueID")]
    public int ValueId { get; set; }

    [StringLength(100)]
    public string ValueName { get; set; } = null!;

    [StringLength(450)]
    public string ValueDisplayName { get; set; } = null!;

    public string ValueQuery { get; set; } = null!;

    public bool ValueQueryIsStoredProcedure { get; set; }

    [StringLength(200)]
    public string? ValueFormatString { get; set; }

    [Column("ValueReportID")]
    public int ValueReportId { get; set; }

    [Column("ValueGUID")]
    public Guid ValueGuid { get; set; }

    public DateTime ValueLastModified { get; set; }

    public string? ValueSettings { get; set; }

    [StringLength(100)]
    public string? ValueConnectionString { get; set; }

    [InverseProperty("ReportSubscriptionValue")]
    public virtual ICollection<ReportingReportSubscription> ReportingReportSubscriptions { get; set; } = new List<ReportingReportSubscription>();

    [ForeignKey("ValueReportId")]
    [InverseProperty("ReportingReportValues")]
    public virtual ReportingReport ValueReport { get; set; } = null!;
}
