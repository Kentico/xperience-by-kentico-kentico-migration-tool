using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Keyless]
public class ViewPollAnswerCount
{
    [Column("PollID")]
    public int PollId { get; set; }

    [StringLength(200)]
    public string PollCodeName { get; set; } = null!;

    [StringLength(200)]
    public string PollDisplayName { get; set; } = null!;

    [StringLength(100)]
    public string? PollTitle { get; set; }

    public DateTime? PollOpenFrom { get; set; }

    public DateTime? PollOpenTo { get; set; }

    public bool PollAllowMultipleAnswers { get; set; }

    [StringLength(450)]
    public string PollQuestion { get; set; } = null!;

    public int PollAccess { get; set; }

    [StringLength(450)]
    public string? PollResponseMessage { get; set; }

    [Column("PollGUID")]
    public Guid PollGuid { get; set; }

    public DateTime PollLastModified { get; set; }

    [Column("PollGroupID")]
    public int? PollGroupId { get; set; }

    [Column("PollSiteID")]
    public int? PollSiteId { get; set; }

    public bool? PollLogActivity { get; set; }

    public int? AnswerCount { get; set; }
}
