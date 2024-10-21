using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[Table("OM_Rule")]
[Index("RuleScoreId", Name = "IX_OM_Rule_RuleScoreID")]
public class OmRule
{
    [Key]
    [Column("RuleID")]
    public int RuleId { get; set; }

    [Column("RuleScoreID")]
    public int RuleScoreId { get; set; }

    [StringLength(200)]
    public string RuleDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string RuleName { get; set; } = null!;

    public int RuleValue { get; set; }

    public bool? RuleIsRecurring { get; set; }

    public int? RuleMaxPoints { get; set; }

    public DateTime? RuleValidUntil { get; set; }

    [StringLength(50)]
    public string? RuleValidity { get; set; }

    public int? RuleValidFor { get; set; }

    public int RuleType { get; set; }

    [StringLength(250)]
    public string? RuleParameter { get; set; }

    public string RuleCondition { get; set; } = null!;

    public DateTime RuleLastModified { get; set; }

    [Column("RuleGUID")]
    public Guid RuleGuid { get; set; }

    public bool RuleBelongsToPersona { get; set; }

    [InverseProperty("Rule")]
    public virtual ICollection<OmScoreContactRule> OmScoreContactRules { get; set; } = new List<OmScoreContactRule>();

    [ForeignKey("RuleScoreId")]
    [InverseProperty("OmRules")]
    public virtual OmScore RuleScore { get; set; } = null!;
}
