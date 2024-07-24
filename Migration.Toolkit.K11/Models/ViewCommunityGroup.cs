using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Keyless]
public class ViewCommunityGroup
{
    [Column("GroupID")]
    public int GroupId { get; set; }

    [Column("GroupGUID")]
    public Guid GroupGuid { get; set; }

    public DateTime GroupLastModified { get; set; }

    [Column("GroupSiteID")]
    public int GroupSiteId { get; set; }

    [StringLength(200)]
    public string GroupDisplayName { get; set; } = null!;

    [StringLength(100)]
    public string GroupName { get; set; } = null!;

    public string GroupDescription { get; set; } = null!;

    [Column("GroupNodeGUID")]
    public Guid? GroupNodeGuid { get; set; }

    public int GroupApproveMembers { get; set; }

    public int GroupAccess { get; set; }

    [Column("GroupCreatedByUserID")]
    public int? GroupCreatedByUserId { get; set; }

    [Column("GroupApprovedByUserID")]
    public int? GroupApprovedByUserId { get; set; }

    [Column("GroupAvatarID")]
    public int? GroupAvatarId { get; set; }

    public bool? GroupApproved { get; set; }

    public DateTime GroupCreatedWhen { get; set; }

    public bool? GroupSendJoinLeaveNotification { get; set; }

    public bool? GroupSendWaitingForApprovalNotification { get; set; }

    public int? GroupSecurity { get; set; }

    public bool? GroupLogActivity { get; set; }

    [Column("AvatarID")]
    public int? AvatarId { get; set; }

    [StringLength(200)]
    public string? AvatarFileName { get; set; }

    [Column("AvatarGUID")]
    public Guid? AvatarGuid { get; set; }
}
