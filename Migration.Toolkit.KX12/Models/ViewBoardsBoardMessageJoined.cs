using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Keyless]
public class ViewBoardsBoardMessageJoined
{
    [Column("BoardID")]
    public int BoardId { get; set; }

    [StringLength(250)]
    public string BoardName { get; set; } = null!;

    [StringLength(250)]
    public string BoardDisplayName { get; set; } = null!;

    public string BoardDescription { get; set; } = null!;

    public DateTime? BoardOpenedFrom { get; set; }

    public bool BoardOpened { get; set; }

    public DateTime? BoardOpenedTo { get; set; }

    public bool BoardEnabled { get; set; }

    public bool BoardModerated { get; set; }

    public int BoardAccess { get; set; }

    public bool BoardUseCaptcha { get; set; }

    public DateTime BoardLastModified { get; set; }

    public int BoardMessages { get; set; }

    [Column("BoardDocumentID")]
    public int BoardDocumentId { get; set; }

    [Column("BoardGUID")]
    public Guid BoardGuid { get; set; }

    [Column("BoardUserID")]
    public int? BoardUserId { get; set; }

    [Column("BoardGroupID")]
    public int? BoardGroupId { get; set; }

    public DateTime? BoardLastMessageTime { get; set; }

    [StringLength(250)]
    public string? BoardLastMessageUserName { get; set; }

    [Column("BoardUnsubscriptionURL")]
    [StringLength(450)]
    public string? BoardUnsubscriptionUrl { get; set; }

    public bool? BoardRequireEmails { get; set; }

    [Column("BoardSiteID")]
    public int BoardSiteId { get; set; }

    public bool BoardEnableSubscriptions { get; set; }

    [Column("BoardBaseURL")]
    [StringLength(450)]
    public string? BoardBaseUrl { get; set; }

    [Column("MessageID")]
    public int MessageId { get; set; }

    [StringLength(250)]
    public string MessageUserName { get; set; } = null!;

    public string MessageText { get; set; } = null!;

    [StringLength(254)]
    public string MessageEmail { get; set; } = null!;

    [Column("MessageURL")]
    [StringLength(450)]
    public string MessageUrl { get; set; } = null!;

    public bool MessageIsSpam { get; set; }

    [Column("MessageBoardID")]
    public int MessageBoardId { get; set; }

    public bool MessageApproved { get; set; }

    [Column("MessageUserID")]
    public int? MessageUserId { get; set; }

    [Column("MessageApprovedByUserID")]
    public int? MessageApprovedByUserId { get; set; }

    public string MessageUserInfo { get; set; } = null!;

    [Column("MessageAvatarGUID")]
    public Guid? MessageAvatarGuid { get; set; }

    public DateTime MessageInserted { get; set; }

    public DateTime MessageLastModified { get; set; }

    [Column("MessageGUID")]
    public Guid MessageGuid { get; set; }

    public double? MessageRatingValue { get; set; }

    [Column("GroupID")]
    public int? GroupId { get; set; }

    [Column("GroupGUID")]
    public Guid? GroupGuid { get; set; }

    public DateTime? GroupLastModified { get; set; }

    [Column("GroupSiteID")]
    public int? GroupSiteId { get; set; }

    [StringLength(200)]
    public string? GroupDisplayName { get; set; }

    [StringLength(100)]
    public string? GroupName { get; set; }

    public string? GroupDescription { get; set; }

    [Column("GroupNodeGUID")]
    public Guid? GroupNodeGuid { get; set; }

    public int? GroupApproveMembers { get; set; }

    public int? GroupAccess { get; set; }

    [Column("GroupCreatedByUserID")]
    public int? GroupCreatedByUserId { get; set; }

    [Column("GroupApprovedByUserID")]
    public int? GroupApprovedByUserId { get; set; }

    [Column("GroupAvatarID")]
    public int? GroupAvatarId { get; set; }

    public bool? GroupApproved { get; set; }

    public DateTime? GroupCreatedWhen { get; set; }

    public bool? GroupSendJoinLeaveNotification { get; set; }

    public bool? GroupSendWaitingForApprovalNotification { get; set; }

    public int? GroupSecurity { get; set; }
}
