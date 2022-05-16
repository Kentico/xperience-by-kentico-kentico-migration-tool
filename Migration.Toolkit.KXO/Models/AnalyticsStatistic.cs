using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXO.Models
{
    [Table("Analytics_Statistics")]
    [Index("StatisticsSiteId", Name = "IX_Analytics_Statistics_StatisticsSiteID")]
    public partial class AnalyticsStatistic
    {
        public AnalyticsStatistic()
        {
            AnalyticsDayHits = new HashSet<AnalyticsDayHit>();
            AnalyticsHourHits = new HashSet<AnalyticsHourHit>();
            AnalyticsMonthHits = new HashSet<AnalyticsMonthHit>();
            AnalyticsWeekHits = new HashSet<AnalyticsWeekHit>();
            AnalyticsYearHits = new HashSet<AnalyticsYearHit>();
        }

        [Key]
        [Column("StatisticsID")]
        public int StatisticsId { get; set; }
        [Column("StatisticsSiteID")]
        public int? StatisticsSiteId { get; set; }
        [StringLength(400)]
        public string StatisticsCode { get; set; } = null!;
        [StringLength(1000)]
        public string? StatisticsObjectName { get; set; }
        [Column("StatisticsObjectID")]
        public int? StatisticsObjectId { get; set; }
        [StringLength(50)]
        public string? StatisticsObjectCulture { get; set; }

        [ForeignKey("StatisticsSiteId")]
        [InverseProperty("AnalyticsStatistics")]
        public virtual CmsSite? StatisticsSite { get; set; }
        [InverseProperty("HitsStatistics")]
        public virtual ICollection<AnalyticsDayHit> AnalyticsDayHits { get; set; }
        [InverseProperty("HitsStatistics")]
        public virtual ICollection<AnalyticsHourHit> AnalyticsHourHits { get; set; }
        [InverseProperty("HitsStatistics")]
        public virtual ICollection<AnalyticsMonthHit> AnalyticsMonthHits { get; set; }
        [InverseProperty("HitsStatistics")]
        public virtual ICollection<AnalyticsWeekHit> AnalyticsWeekHits { get; set; }
        [InverseProperty("HitsStatistics")]
        public virtual ICollection<AnalyticsYearHit> AnalyticsYearHits { get; set; }
    }
}
