using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("SM_InsightHit_Month")]
[Index("InsightHitInsightId", "InsightHitPeriodFrom", "InsightHitPeriodTo", Name = "UQ_SM_InsightHit_Month_InsightHitInsightID_InsightHitPeriodFrom_InsightHitPeriodTo", IsUnique = true)]
public class SmInsightHitMonth
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
    [InverseProperty("SmInsightHitMonths")]
    public virtual SmInsight InsightHitInsight { get; set; } = null!;
}
