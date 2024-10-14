using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX13.Models;

[Table("Analytics_HourHits")]
[Index("HitsStatisticsId", Name = "IX_Analytics_HourHits_HitsStatisticsID")]
public class AnalyticsHourHit
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
    [InverseProperty("AnalyticsHourHits")]
    public virtual AnalyticsStatistic HitsStatistics { get; set; } = null!;
}
