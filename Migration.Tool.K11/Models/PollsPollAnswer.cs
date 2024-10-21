using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("Polls_PollAnswer")]
[Index("AnswerPollId", Name = "IX_Polls_PollAnswer_AnswerPollID")]
public class PollsPollAnswer
{
    [Key]
    [Column("AnswerID")]
    public int AnswerId { get; set; }

    [StringLength(200)]
    public string AnswerText { get; set; } = null!;

    public int? AnswerOrder { get; set; }

    public int? AnswerCount { get; set; }

    public bool? AnswerEnabled { get; set; }

    [Column("AnswerPollID")]
    public int AnswerPollId { get; set; }

    [Column("AnswerGUID")]
    public Guid AnswerGuid { get; set; }

    public DateTime AnswerLastModified { get; set; }

    [StringLength(100)]
    public string? AnswerForm { get; set; }

    [StringLength(100)]
    public string? AnswerAlternativeForm { get; set; }

    public bool? AnswerHideForm { get; set; }

    [ForeignKey("AnswerPollId")]
    [InverseProperty("PollsPollAnswers")]
    public virtual PollsPoll AnswerPoll { get; set; } = null!;
}
