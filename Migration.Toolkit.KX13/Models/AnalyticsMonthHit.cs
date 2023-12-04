using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Analytics_MonthHits")]
[Index("HitsStatisticsId", Name = "IX_Analytics_MonthHits_HitsStatisticsID")]
public partial class AnalyticsMonthHit
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
    [InverseProperty("AnalyticsMonthHits")]
    public virtual AnalyticsStatistic HitsStatistics { get; set; } = null!;
}
