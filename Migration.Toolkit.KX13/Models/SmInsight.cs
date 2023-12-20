using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("SM_Insight")]
[Index("InsightCodeName", "InsightPeriodType", Name = "IX_SM_Insight_InsightCodeName_InsightPeriodType")]
public partial class SmInsight
{
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
    public virtual ICollection<SmInsightHitDay> SmInsightHitDays { get; set; } = new List<SmInsightHitDay>();

    [InverseProperty("InsightHitInsight")]
    public virtual ICollection<SmInsightHitMonth> SmInsightHitMonths { get; set; } = new List<SmInsightHitMonth>();

    [InverseProperty("InsightHitInsight")]
    public virtual ICollection<SmInsightHitWeek> SmInsightHitWeeks { get; set; } = new List<SmInsightHitWeek>();

    [InverseProperty("InsightHitInsight")]
    public virtual ICollection<SmInsightHitYear> SmInsightHitYears { get; set; } = new List<SmInsightHitYear>();
}
