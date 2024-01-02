using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Reporting_ReportSubscription")]
[Index("ReportSubscriptionGraphId", Name = "IX_Reporting_ReportSubscription_ReportSubscriptionGraphID")]
[Index("ReportSubscriptionReportId", Name = "IX_Reporting_ReportSubscription_ReportSubscriptionReportID")]
[Index("ReportSubscriptionSiteId", Name = "IX_Reporting_ReportSubscription_ReportSubscriptionSiteID")]
[Index("ReportSubscriptionTableId", Name = "IX_Reporting_ReportSubscription_ReportSubscriptionTableID")]
[Index("ReportSubscriptionUserId", Name = "IX_Reporting_ReportSubscription_ReportSubscriptionUserID")]
[Index("ReportSubscriptionValueId", Name = "IX_Reporting_ReportSubscription_ReportSubscriptionValueID")]
public partial class ReportingReportSubscription
{
    [Key]
    [Column("ReportSubscriptionID")]
    public int ReportSubscriptionId { get; set; }

    [Column("ReportSubscriptionReportID")]
    public int ReportSubscriptionReportId { get; set; }

    [StringLength(1000)]
    public string ReportSubscriptionInterval { get; set; } = null!;

    public string? ReportSubscriptionCondition { get; set; }

    [Required]
    public bool? ReportSubscriptionEnabled { get; set; }

    public string? ReportSubscriptionParameters { get; set; }

    [Column("ReportSubscriptionGUID")]
    public Guid ReportSubscriptionGuid { get; set; }

    public DateTime ReportSubscriptionLastModified { get; set; }

    [StringLength(200)]
    public string? ReportSubscriptionSubject { get; set; }

    [Column("ReportSubscriptionGraphID")]
    public int? ReportSubscriptionGraphId { get; set; }

    [Column("ReportSubscriptionTableID")]
    public int? ReportSubscriptionTableId { get; set; }

    [Column("ReportSubscriptionValueID")]
    public int? ReportSubscriptionValueId { get; set; }

    [Column("ReportSubscriptionUserID")]
    public int ReportSubscriptionUserId { get; set; }

    [StringLength(400)]
    public string ReportSubscriptionEmail { get; set; } = null!;

    [Required]
    public bool? ReportSubscriptionOnlyNonEmpty { get; set; }

    public DateTime? ReportSubscriptionLastPostDate { get; set; }

    public DateTime? ReportSubscriptionNextPostDate { get; set; }

    [Column("ReportSubscriptionSiteID")]
    public int ReportSubscriptionSiteId { get; set; }

    public string? ReportSubscriptionSettings { get; set; }

    [ForeignKey("ReportSubscriptionGraphId")]
    [InverseProperty("ReportingReportSubscriptions")]
    public virtual ReportingReportGraph? ReportSubscriptionGraph { get; set; }

    [ForeignKey("ReportSubscriptionReportId")]
    [InverseProperty("ReportingReportSubscriptions")]
    public virtual ReportingReport ReportSubscriptionReport { get; set; } = null!;

    [ForeignKey("ReportSubscriptionSiteId")]
    [InverseProperty("ReportingReportSubscriptions")]
    public virtual CmsSite ReportSubscriptionSite { get; set; } = null!;

    [ForeignKey("ReportSubscriptionTableId")]
    [InverseProperty("ReportingReportSubscriptions")]
    public virtual ReportingReportTable? ReportSubscriptionTable { get; set; }

    [ForeignKey("ReportSubscriptionUserId")]
    [InverseProperty("ReportingReportSubscriptions")]
    public virtual CmsUser ReportSubscriptionUser { get; set; } = null!;

    [ForeignKey("ReportSubscriptionValueId")]
    [InverseProperty("ReportingReportSubscriptions")]
    public virtual ReportingReportValue? ReportSubscriptionValue { get; set; }
}
