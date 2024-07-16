using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Polls_Poll")]
[Index("PollGroupId", Name = "IX_Polls_Poll_PollGroupID")]
[Index("PollSiteId", "PollCodeName", Name = "IX_Polls_Poll_PollSiteID_PollCodeName")]
public partial class PollsPoll
{
    [Key]
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

    [ForeignKey("PollGroupId")]
    [InverseProperty("PollsPolls")]
    public virtual CommunityGroup? PollGroup { get; set; }

    [ForeignKey("PollSiteId")]
    [InverseProperty("PollsPolls")]
    public virtual CmsSite? PollSite { get; set; }

    [InverseProperty("AnswerPoll")]
    public virtual ICollection<PollsPollAnswer> PollsPollAnswers { get; set; } = new List<PollsPollAnswer>();

    [ForeignKey("PollId")]
    [InverseProperty("Polls")]
    public virtual ICollection<CmsRole> Roles { get; set; } = new List<CmsRole>();

    [ForeignKey("PollId")]
    [InverseProperty("Polls")]
    public virtual ICollection<CmsSite> Sites { get; set; } = new List<CmsSite>();
}
