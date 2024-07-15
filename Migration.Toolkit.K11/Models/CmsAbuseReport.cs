using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CMS_AbuseReport")]
[Index("ReportSiteId", Name = "IX_CMS_AbuseReport_ReportSiteID")]
[Index("ReportStatus", Name = "IX_CMS_AbuseReport_ReportStatus")]
[Index("ReportUserId", Name = "IX_CMS_AbuseReport_ReportUserID")]
public partial class CmsAbuseReport
{
    [Key]
    [Column("ReportID")]
    public int ReportId { get; set; }

    [Column("ReportGUID")]
    public Guid ReportGuid { get; set; }

    [StringLength(100)]
    public string? ReportTitle { get; set; }

    [Column("ReportURL")]
    [StringLength(1000)]
    public string ReportUrl { get; set; } = null!;

    [StringLength(50)]
    public string ReportCulture { get; set; } = null!;

    [Column("ReportObjectID")]
    public int? ReportObjectId { get; set; }

    [StringLength(100)]
    public string? ReportObjectType { get; set; }

    public string ReportComment { get; set; } = null!;

    [Column("ReportUserID")]
    public int? ReportUserId { get; set; }

    public DateTime ReportWhen { get; set; }

    public int ReportStatus { get; set; }

    [Column("ReportSiteID")]
    public int ReportSiteId { get; set; }

    [ForeignKey("ReportSiteId")]
    [InverseProperty("CmsAbuseReports")]
    public virtual CmsSite ReportSite { get; set; } = null!;

    [ForeignKey("ReportUserId")]
    [InverseProperty("CmsAbuseReports")]
    public virtual CmsUser? ReportUser { get; set; }
}