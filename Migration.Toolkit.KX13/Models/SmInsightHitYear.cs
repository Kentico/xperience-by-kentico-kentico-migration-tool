using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("SM_InsightHit_Year")]
[Index("InsightHitInsightId", "InsightHitPeriodFrom", "InsightHitPeriodTo", Name = "UQ_SM_InsightHit_Year_InsightHitInsightID_InsightHitPeriodFrom_InsightHitPeriodTo", IsUnique = true)]
public partial class SmInsightHitYear
{
    [Key]
    [Column("InsightHitID")]
    public int InsightHitId { get; set; }

    public DateTime InsightHitPeriodFrom { get; set; }

    public DateTime InsightHitPeriodTo { get; set; }

    public long InsightHitValue { get; set; }

    [Column("InsightHitInsightID")]
    public int InsightHitInsightId { get; set; }

    [ForeignKey("InsightHitInsightId")]
    [InverseProperty("SmInsightHitYears")]
    public virtual SmInsight InsightHitInsight { get; set; } = null!;
}
