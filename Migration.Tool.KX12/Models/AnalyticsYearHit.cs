using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[Table("Analytics_YearHits")]
[Index("HitsStatisticsId", Name = "IX_Analytics_WeekYearHits_HitsStatisticsID")]
public class AnalyticsYearHit
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
    [InverseProperty("AnalyticsYearHits")]
    public virtual AnalyticsStatistic HitsStatistics { get; set; } = null!;
}
