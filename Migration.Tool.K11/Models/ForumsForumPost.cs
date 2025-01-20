using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("Forums_ForumPost")]
[Index("PostApproved", Name = "IX_Forums_ForumPost_PostApproved")]
[Index("PostApprovedByUserId", Name = "IX_Forums_ForumPost_PostApprovedByUserID")]
[Index("PostForumId", Name = "IX_Forums_ForumPost_PostForumID")]
[Index("PostLevel", Name = "IX_Forums_ForumPost_PostLevel")]
[Index("PostParentId", Name = "IX_Forums_ForumPost_PostParentID")]
[Index("PostUserId", Name = "IX_Forums_ForumPost_PostUserID")]
public class ForumsForumPost
{
    [Key]
    public int PostId { get; set; }

    [Column("PostForumID")]
    public int PostForumId { get; set; }

    [Column("PostParentID")]
    public int? PostParentId { get; set; }

    [Column("PostIDPath")]
    public string PostIdpath { get; set; } = null!;

    public int PostLevel { get; set; }

    [StringLength(450)]
    public string PostSubject { get; set; } = null!;

    [Column("PostUserID")]
    public int? PostUserId { get; set; }

    [StringLength(200)]
    public string PostUserName { get; set; } = null!;

    [StringLength(254)]
    public string? PostUserMail { get; set; }

    public string? PostText { get; set; }

    public DateTime PostTime { get; set; }

    [Column("PostApprovedByUserID")]
    public int? PostApprovedByUserId { get; set; }

    public int? PostThreadPosts { get; set; }

    [StringLength(200)]
    public string? PostThreadLastPostUserName { get; set; }

    public DateTime? PostThreadLastPostTime { get; set; }

    public string? PostUserSignature { get; set; }

    [Column("PostGUID")]
    public Guid PostGuid { get; set; }

    public DateTime PostLastModified { get; set; }

    public bool? PostApproved { get; set; }

    public bool? PostIsLocked { get; set; }

    public int? PostIsAnswer { get; set; }

    public int PostStickOrder { get; set; }

    public int? PostViews { get; set; }

    public DateTime? PostLastEdit { get; set; }

    public string? PostInfo { get; set; }

    public int? PostAttachmentCount { get; set; }

    public int? PostType { get; set; }

    public int? PostThreadPostsAbsolute { get; set; }

    [StringLength(200)]
    public string? PostThreadLastPostUserNameAbsolute { get; set; }

    public DateTime? PostThreadLastPostTimeAbsolute { get; set; }

    public bool? PostQuestionSolved { get; set; }

    public int? PostIsNotAnswer { get; set; }

    [Column("PostSiteID")]
    public int? PostSiteId { get; set; }

    [InverseProperty("AttachmentPost")]
    public virtual ICollection<ForumsAttachment> ForumsAttachments { get; set; } = [];

    [InverseProperty("SubscriptionPost")]
    public virtual ICollection<ForumsForumSubscription> ForumsForumSubscriptions { get; set; } = [];

    [InverseProperty("Post")]
    public virtual ICollection<ForumsUserFavorite> ForumsUserFavorites { get; set; } = [];

    [InverseProperty("PostParent")]
    public virtual ICollection<ForumsForumPost> InversePostParent { get; set; } = [];

    [ForeignKey("PostApprovedByUserId")]
    [InverseProperty("ForumsForumPostPostApprovedByUsers")]
    public virtual CmsUser? PostApprovedByUser { get; set; }

    [ForeignKey("PostForumId")]
    [InverseProperty("ForumsForumPosts")]
    public virtual ForumsForum PostForum { get; set; } = null!;

    [ForeignKey("PostParentId")]
    [InverseProperty("InversePostParent")]
    public virtual ForumsForumPost? PostParent { get; set; }

    [ForeignKey("PostUserId")]
    [InverseProperty("ForumsForumPostPostUsers")]
    public virtual CmsUser? PostUser { get; set; }
}
