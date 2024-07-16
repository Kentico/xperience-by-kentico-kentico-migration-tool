using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("Analytics_Conversion")]
[Index("ConversionSiteId", Name = "IX_Analytics_Conversion_ConversionSiteID")]
public partial class AnalyticsConversion
{
    [Key]
    [Column("ConversionID")]
    public int ConversionId { get; set; }

    [StringLength(200)]
    public string ConversionName { get; set; } = null!;

    [StringLength(200)]
    public string ConversionDisplayName { get; set; } = null!;

    public string? ConversionDescription { get; set; }

    [Column("ConversionGUID")]
    public Guid ConversionGuid { get; set; }

    public DateTime ConversionLastModified { get; set; }

    [Column("ConversionSiteID")]
    public int ConversionSiteId { get; set; }

    [ForeignKey("ConversionSiteId")]
    [InverseProperty("AnalyticsConversions")]
    public virtual CmsSite ConversionSite { get; set; } = null!;
}
