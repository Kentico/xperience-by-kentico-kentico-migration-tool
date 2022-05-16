﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models
{
    [Table("SM_Insight")]
    [Index("InsightCodeName", "InsightPeriodType", Name = "IX_SM_Insight_InsightCodeName_InsightPeriodType")]
    public partial class SmInsight
    {
        public SmInsight()
        {
            SmInsightHitDays = new HashSet<SmInsightHitDay>();
            SmInsightHitMonths = new HashSet<SmInsightHitMonth>();
            SmInsightHitWeeks = new HashSet<SmInsightHitWeek>();
            SmInsightHitYears = new HashSet<SmInsightHitYear>();
        }

        [Key]
        [Column("InsightID")]
        public int InsightId { get; set; }
        [StringLength(200)]
        public string InsightCodeName { get; set; } = null!;
        [Column("InsightExternalID")]
        public string InsightExternalId { get; set; } = null!;
        [StringLength(20)]
        public string InsightPeriodType { get; set; } = null!;
        public string? InsightValueName { get; set; }

        [InverseProperty("InsightHitInsight")]
        public virtual ICollection<SmInsightHitDay> SmInsightHitDays { get; set; }
        [InverseProperty("InsightHitInsight")]
        public virtual ICollection<SmInsightHitMonth> SmInsightHitMonths { get; set; }
        [InverseProperty("InsightHitInsight")]
        public virtual ICollection<SmInsightHitWeek> SmInsightHitWeeks { get; set; }
        [InverseProperty("InsightHitInsight")]
        public virtual ICollection<SmInsightHitYear> SmInsightHitYears { get; set; }
    }
}
