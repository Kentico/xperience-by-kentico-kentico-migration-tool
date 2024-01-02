using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("OM_ScoreContactRule")]
[Index("ContactId", Name = "IX_OM_ScoreContactRule_ContactID")]
[Index("RuleId", Name = "IX_OM_ScoreContactRule_RuleID")]
[Index("ScoreId", Name = "IX_OM_ScoreContactRule_ScoreID_ContactID_Value_Expiration")]
[Index("ScoreId", "ContactId", "RuleId", Name = "UQ_OM_ScoreContactRule", IsUnique = true)]
public partial class OmScoreContactRule
{
    [Column("ScoreID")]
    public int ScoreId { get; set; }

    [Column("ContactID")]
    public int ContactId { get; set; }

    [Column("RuleID")]
    public int RuleId { get; set; }

    public int Value { get; set; }

    public DateTime? Expiration { get; set; }

    [Key]
    [Column("ScoreContactRuleID")]
    public int ScoreContactRuleId { get; set; }

    [ForeignKey("ContactId")]
    [InverseProperty("OmScoreContactRules")]
    public virtual OmContact Contact { get; set; } = null!;

    [ForeignKey("RuleId")]
    [InverseProperty("OmScoreContactRules")]
    public virtual OmRule Rule { get; set; } = null!;

    [ForeignKey("ScoreId")]
    [InverseProperty("OmScoreContactRules")]
    public virtual OmScore Score { get; set; } = null!;
}
