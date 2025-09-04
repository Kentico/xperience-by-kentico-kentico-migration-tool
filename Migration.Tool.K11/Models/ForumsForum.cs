using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("Forums_Forum")]
[Index("ForumCommunityGroupId", Name = "IX_Forums_Forum_ForumCommunityGroupID")]
[Index("ForumDocumentId", Name = "IX_Forums_Forum_ForumDocumentID")]
[Index("ForumSiteId", "ForumName", Name = "IX_Forums_Forum_ForumSiteID_ForumName")]
public class ForumsForum
{
    [Key]
    [Column("ForumID")]
    public int ForumId { get; set; }

    [Column("ForumGroupID")]
    public int ForumGroupId { get; set; }

    [StringLength(200)]
    public string ForumName { get; set; } = null!;

    [StringLength(200)]
    public string ForumDisplayName { get; set; } = null!;

    public string? ForumDescription { get; set; }

    public int? ForumOrder { get; set; }

    [Column("ForumDocumentID")]
    public int? ForumDocumentId { get; set; }

    public bool ForumOpen { get; set; }

    public bool ForumModerated { get; set; }

    public bool? ForumDisplayEmails { get; set; }

    public bool? ForumRequireEmail { get; set; }

    public int ForumAccess { get; set; }

    public int ForumThreads { get; set; }

    public int ForumPosts { get; set; }

    public DateTime? ForumLastPostTime { get; set; }

    [StringLength(200)]
    public string? ForumLastPostUserName { get; set; }

    [StringLength(200)]
    public string? ForumBaseUrl { get; set; }

    public bool? ForumAllowChangeName { get; set; }

    [Column("ForumHTMLEditor")]
    public bool? ForumHtmleditor { get; set; }

    [Column("ForumUseCAPTCHA")]
    public bool? ForumUseCaptcha { get; set; }

    [Column("ForumGUID")]
    public Guid ForumGuid { get; set; }

    public DateTime ForumLastModified { get; set; }

    [StringLength(200)]
    public string? ForumUnsubscriptionUrl { get; set; }

    public bool? ForumIsLocked { get; set; }

    public string? ForumSettings { get; set; }

    public bool? ForumAuthorEdit { get; set; }

    public bool? ForumAuthorDelete { get; set; }

    public int? ForumType { get; set; }

    public int? ForumIsAnswerLimit { get; set; }

    public int? ForumImageMaxSideSize { get; set; }

    public DateTime? ForumLastPostTimeAbsolute { get; set; }

    [StringLength(200)]
    public string? ForumLastPostUserNameAbsolute { get; set; }

    public int? ForumPostsAbsolute { get; set; }

    public int? ForumThreadsAbsolute { get; set; }

    public int? ForumAttachmentMaxFileSize { get; set; }

    public int? ForumDiscussionActions { get; set; }

    [Column("ForumSiteID")]
    public int ForumSiteId { get; set; }

    public bool? ForumLogActivity { get; set; }

    [Column("ForumCommunityGroupID")]
    public int? ForumCommunityGroupId { get; set; }

    public bool? ForumEnableOptIn { get; set; }

    public bool? ForumSendOptInConfirmation { get; set; }

    [Column("ForumOptInApprovalURL")]
    [StringLength(450)]
    public string? ForumOptInApprovalUrl { get; set; }

    [ForeignKey("ForumCommunityGroupId")]
    [InverseProperty("ForumsForums")]
    public virtual CommunityGroup? ForumCommunityGroup { get; set; }

    [ForeignKey("ForumDocumentId")]
    [InverseProperty("ForumsForums")]
    public virtual CmsDocument? ForumDocument { get; set; }

    [ForeignKey("ForumGroupId")]
    [InverseProperty("ForumsForums")]
    public virtual ForumsForumGroup ForumGroup { get; set; } = null!;

    [ForeignKey("ForumSiteId")]
    [InverseProperty("ForumsForums")]
    public virtual CmsSite ForumSite { get; set; } = null!;

    [InverseProperty("PostForum")]
    public virtual ICollection<ForumsForumPost> ForumsForumPosts { get; set; } = new List<ForumsForumPost>();

    [InverseProperty("Forum")]
    public virtual ICollection<ForumsForumRole> ForumsForumRoles { get; set; } = new List<ForumsForumRole>();

    [InverseProperty("SubscriptionForum")]
    public virtual ICollection<ForumsForumSubscription> ForumsForumSubscriptions { get; set; } = new List<ForumsForumSubscription>();

    [InverseProperty("Forum")]
    public virtual ICollection<ForumsUserFavorite> ForumsUserFavorites { get; set; } = new List<ForumsUserFavorite>();

    [ForeignKey("ForumId")]
    [InverseProperty("Forums")]
    public virtual ICollection<CmsUser> Users { get; set; } = new List<CmsUser>();
}
