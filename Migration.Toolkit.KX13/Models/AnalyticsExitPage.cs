using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Analytics_ExitPages")]
[Index("ExitPageLastModified", Name = "IX_Analytics_ExitPages_ExitPageLastModified")]
[Index("ExitPageSessionIdentifier", Name = "UQ_Analytics_ExitPages_ExitPageSessionIdentifier", IsUnique = true)]
public partial class AnalyticsExitPage
{
    public DateTime ExitPageLastModified { get; set; }

    [Column("ExitPageSiteID")]
    public int ExitPageSiteId { get; set; }

    [StringLength(50)]
    public string? ExitPageCulture { get; set; }

    [Key]
    public int ExitPageId { get; set; }

    [StringLength(1000)]
    public string ExitPageUrl { get; set; } = null!;

    public Guid ExitPageSessionIdentifier { get; set; }

    [ForeignKey("ExitPageSiteId")]
    [InverseProperty("AnalyticsExitPages")]
    public virtual CmsSite ExitPageSite { get; set; } = null!;
}
