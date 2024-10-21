using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("Analytics_Statistics")]
[Index("StatisticsSiteId", Name = "IX_Analytics_Statistics_StatisticsSiteID")]
public class AnalyticsStatistic
{
    [Key]
    [Column("StatisticsID")]
    public int StatisticsId { get; set; }

    [Column("StatisticsSiteID")]
    public int? StatisticsSiteId { get; set; }

    [StringLength(400)]
    public string StatisticsCode { get; set; } = null!;

    [StringLength(450)]
    public string? StatisticsObjectName { get; set; }

    [Column("StatisticsObjectID")]
    public int? StatisticsObjectId { get; set; }

    [StringLength(10)]
    public string? StatisticsObjectCulture { get; set; }

    [InverseProperty("HitsStatistics")]
    public virtual ICollection<AnalyticsDayHit> AnalyticsDayHits { get; set; } = new List<AnalyticsDayHit>();

    [InverseProperty("HitsStatistics")]
    public virtual ICollection<AnalyticsHourHit> AnalyticsHourHits { get; set; } = new List<AnalyticsHourHit>();

    [InverseProperty("HitsStatistics")]
    public virtual ICollection<AnalyticsMonthHit> AnalyticsMonthHits { get; set; } = new List<AnalyticsMonthHit>();

    [InverseProperty("HitsStatistics")]
    public virtual ICollection<AnalyticsWeekHit> AnalyticsWeekHits { get; set; } = new List<AnalyticsWeekHit>();

    [InverseProperty("HitsStatistics")]
    public virtual ICollection<AnalyticsYearHit> AnalyticsYearHits { get; set; } = new List<AnalyticsYearHit>();

    [ForeignKey("StatisticsSiteId")]
    [InverseProperty("AnalyticsStatistics")]
    public virtual CmsSite? StatisticsSite { get; set; }
}
