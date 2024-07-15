using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Keyless]
public partial class ViewForumsGroupForumPostJoined
{
    [Column("ForumID")]
    public int? ForumId { get; set; }

    [Column("ForumGroupID")]
    public int? ForumGroupId { get; set; }

    [StringLength(200)]
    public string? ForumName { get; set; }

    [StringLength(200)]
    public string? ForumDisplayName { get; set; }

    public string? ForumDescription { get; set; }

    public int? ForumOrder { get; set; }

    [Column("ForumDocumentID")]
    public int? ForumDocumentId { get; set; }

    public bool? ForumOpen { get; set; }

    public bool? ForumModerated { get; set; }

    public bool? ForumDisplayEmails { get; set; }

    public bool? ForumRequireEmail { get; set; }

    public int? ForumAccess { get; set; }

    public int? ForumThreads { get; set; }

    public int? ForumPosts { get; set; }

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
    public Guid? ForumGuid { get; set; }

    public DateTime? ForumLastModified { get; set; }

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
    public int? ForumSiteId { get; set; }

    [Column("GroupID")]
    public int? GroupId { get; set; }

    [Column("GroupSiteID")]
    public int? GroupSiteId { get; set; }

    [StringLength(200)]
    public string? GroupName { get; set; }

    [StringLength(200)]
    public string? GroupDisplayName { get; set; }

    public int? GroupOrder { get; set; }

    public string? GroupDescription { get; set; }

    [Column("GroupGUID")]
    public Guid? GroupGuid { get; set; }

    public DateTime? GroupLastModified { get; set; }

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

    public int PostId { get; set; }

    [Column("PostForumID")]
    public int PostForumId { get; set; }

    [Column("PostParentID")]
    public int? PostParentId { get; set; }

    [Column("PostIDPath")]
    [StringLength(450)]
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
}