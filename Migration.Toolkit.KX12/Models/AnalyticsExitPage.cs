using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("Analytics_ExitPages")]
[Index("ExitPageLastModified", Name = "IX_Analytics_ExitPages_ExitPageLastModified")]
public partial class AnalyticsExitPage
{
    [Key]
    [StringLength(200)]
    public string SessionIdentificator { get; set; } = null!;

    [Column("ExitPageNodeID")]
    public int ExitPageNodeId { get; set; }

    public DateTime ExitPageLastModified { get; set; }

    [Column("ExitPageSiteID")]
    public int ExitPageSiteId { get; set; }

    [StringLength(10)]
    public string? ExitPageCulture { get; set; }
}