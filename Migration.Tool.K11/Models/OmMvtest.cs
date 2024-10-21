using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("OM_MVTest")]
[Index("MvtestSiteId", Name = "IX_OM_MVTest_MVTestSiteID")]
public class OmMvtest
{
    [Key]
    [Column("MVTestID")]
    public int MvtestId { get; set; }

    [Column("MVTestName")]
    [StringLength(50)]
    public string MvtestName { get; set; } = null!;

    [Column("MVTestDescription")]
    public string? MvtestDescription { get; set; }

    [Column("MVTestPage")]
    [StringLength(450)]
    public string MvtestPage { get; set; } = null!;

    [Column("MVTestSiteID")]
    public int MvtestSiteId { get; set; }

    [Column("MVTestCulture")]
    [StringLength(50)]
    public string? MvtestCulture { get; set; }

    [Column("MVTestOpenFrom")]
    public DateTime? MvtestOpenFrom { get; set; }

    [Column("MVTestOpenTo")]
    public DateTime? MvtestOpenTo { get; set; }

    [Column("MVTestMaxConversions")]
    public int? MvtestMaxConversions { get; set; }

    [Column("MVTestConversions")]
    public int? MvtestConversions { get; set; }

    [Column("MVTestTargetConversionType")]
    [StringLength(100)]
    public string? MvtestTargetConversionType { get; set; }

    [Column("MVTestGUID")]
    public Guid MvtestGuid { get; set; }

    [Column("MVTestLastModified")]
    public DateTime MvtestLastModified { get; set; }

    [Column("MVTestEnabled")]
    public bool MvtestEnabled { get; set; }

    [Column("MVTestDisplayName")]
    [StringLength(100)]
    public string MvtestDisplayName { get; set; } = null!;

    [ForeignKey("MvtestSiteId")]
    [InverseProperty("OmMvtests")]
    public virtual CmsSite MvtestSite { get; set; } = null!;
}
