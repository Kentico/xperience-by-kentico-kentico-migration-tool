using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("OM_Score")]
public partial class OmScore
{
    [Key]
    [Column("ScoreID")]
    public int ScoreId { get; set; }

    [StringLength(200)]
    public string ScoreName { get; set; } = null!;

    [StringLength(200)]
    public string ScoreDisplayName { get; set; } = null!;

    public string? ScoreDescription { get; set; }

    public bool ScoreEnabled { get; set; }

    public int? ScoreEmailAtScore { get; set; }

    [StringLength(998)]
    public string? ScoreNotificationEmail { get; set; }

    public DateTime ScoreLastModified { get; set; }

    [Column("ScoreGUID")]
    public Guid ScoreGuid { get; set; }

    public int? ScoreStatus { get; set; }

    [Column("ScoreScheduledTaskID")]
    public int? ScoreScheduledTaskId { get; set; }

    [Column("ScorePersonaID")]
    public int? ScorePersonaId { get; set; }

    [InverseProperty("RuleScore")]
    public virtual ICollection<OmRule> OmRules { get; set; } = new List<OmRule>();

    [InverseProperty("Score")]
    public virtual ICollection<OmScoreContactRule> OmScoreContactRules { get; set; } = new List<OmScoreContactRule>();

    [ForeignKey("ScorePersonaId")]
    [InverseProperty("OmScore")]
    public virtual PersonasPersona? ScorePersona { get; set; }
}
