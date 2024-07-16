using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Forums_ForumGroup")]
[Index("GroupGroupId", Name = "IX_Forums_ForumGroup_GroupGroupID")]
[Index("GroupSiteId", "GroupName", Name = "IX_Forums_ForumGroup_GroupSiteID_GroupName")]
public partial class ForumsForumGroup
{
    [Key]
    [Column("GroupID")]
    public int GroupId { get; set; }

    [Column("GroupSiteID")]
    public int GroupSiteId { get; set; }

    [StringLength(200)]
    public string GroupName { get; set; } = null!;

    [StringLength(200)]
    public string GroupDisplayName { get; set; } = null!;

    public int? GroupOrder { get; set; }

    public string? GroupDescription { get; set; }

    [Column("GroupGUID")]
    public Guid GroupGuid { get; set; }

    public DateTime GroupLastModified { get; set; }

    [StringLength(200)]
    public string? GroupBaseUrl { get; set; }

    [StringLength(200)]
    public string? GroupUnsubscriptionUrl { get; set; }

    [Column("GroupGroupID")]
    public int? GroupGroupId { get; set; }

    public bool? GroupAuthorEdit { get; set; }

    public bool? GroupAuthorDelete { get; set; }

    public int? GroupType { get; set; }

    public int? GroupIsAnswerLimit { get; set; }

    public int? GroupImageMaxSideSize { get; set; }

    public bool? GroupDisplayEmails { get; set; }

    public bool? GroupRequireEmail { get; set; }

    [Column("GroupHTMLEditor")]
    public bool? GroupHtmleditor { get; set; }

    [Column("GroupUseCAPTCHA")]
    public bool? GroupUseCaptcha { get; set; }

    public int? GroupAttachmentMaxFileSize { get; set; }

    public int? GroupDiscussionActions { get; set; }

    public bool? GroupLogActivity { get; set; }

    public bool? GroupEnableOptIn { get; set; }

    public bool? GroupSendOptInConfirmation { get; set; }

    [Column("GroupOptInApprovalURL")]
    [StringLength(450)]
    public string? GroupOptInApprovalUrl { get; set; }

    [InverseProperty("ForumGroup")]
    public virtual ICollection<ForumsForum> ForumsForums { get; set; } = new List<ForumsForum>();

    [ForeignKey("GroupGroupId")]
    [InverseProperty("ForumsForumGroups")]
    public virtual CommunityGroup? GroupGroup { get; set; }

    [ForeignKey("GroupSiteId")]
    [InverseProperty("ForumsForumGroups")]
    public virtual CmsSite GroupSite { get; set; } = null!;
}
