using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("Analytics_WeekHits")]
[Index("HitsStatisticsId", Name = "IX_Analytics_WeekHits_HitsStatisticsID")]
public class AnalyticsWeekHit
{
    [Key]
    [Column("HitsID")]
    public int HitsId { get; set; }

    [Column("HitsStatisticsID")]
    public int HitsStatisticsId { get; set; }

    public DateTime HitsStartTime { get; set; }

    public DateTime HitsEndTime { get; set; }

    public int HitsCount { get; set; }

    public double? HitsValue { get; set; }

    [ForeignKey("HitsStatisticsId")]
    [InverseProperty("AnalyticsWeekHits")]
    public virtual AnalyticsStatistic HitsStatistics { get; set; } = null!;
}
