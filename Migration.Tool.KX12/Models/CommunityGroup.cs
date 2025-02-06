using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[Table("Community_Group")]
[Index("GroupApproved", Name = "IX_Community_Group_GroupApproved")]
[Index("GroupApprovedByUserId", Name = "IX_Community_Group_GroupApprovedByUserID")]
[Index("GroupAvatarId", Name = "IX_Community_Group_GroupAvatarID")]
[Index("GroupCreatedByUserId", Name = "IX_Community_Group_GroupCreatedByUserID")]
[Index("GroupSiteId", "GroupName", Name = "IX_Community_Group_GroupSiteID_GroupName")]
public class CommunityGroup
{
    [Key]
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

    [InverseProperty("BoardGroup")]
    public virtual ICollection<BoardBoard> BoardBoards { get; set; } = [];

    [InverseProperty("RoleGroup")]
    public virtual ICollection<CmsRole> CmsRoles { get; set; } = [];

    [InverseProperty("NodeGroup")]
    public virtual ICollection<CmsTree> CmsTrees { get; set; } = [];

    [InverseProperty("MemberGroup")]
    public virtual ICollection<CommunityGroupMember> CommunityGroupMembers { get; set; } = [];

    [InverseProperty("Group")]
    public virtual ICollection<CommunityGroupRolePermission> CommunityGroupRolePermissions { get; set; } = [];

    [InverseProperty("InvitationGroup")]
    public virtual ICollection<CommunityInvitation> CommunityInvitations { get; set; } = [];

    [InverseProperty("GroupGroup")]
    public virtual ICollection<ForumsForumGroup> ForumsForumGroups { get; set; } = [];

    [InverseProperty("ForumCommunityGroup")]
    public virtual ICollection<ForumsForum> ForumsForums { get; set; } = [];

    [ForeignKey("GroupApprovedByUserId")]
    [InverseProperty("CommunityGroupGroupApprovedByUsers")]
    public virtual CmsUser? GroupApprovedByUser { get; set; }

    [ForeignKey("GroupAvatarId")]
    [InverseProperty("CommunityGroups")]
    public virtual CmsAvatar? GroupAvatar { get; set; }

    [ForeignKey("GroupCreatedByUserId")]
    [InverseProperty("CommunityGroupGroupCreatedByUsers")]
    public virtual CmsUser? GroupCreatedByUser { get; set; }

    [ForeignKey("GroupSiteId")]
    [InverseProperty("CommunityGroups")]
    public virtual CmsSite GroupSite { get; set; } = null!;

    [InverseProperty("LibraryGroup")]
    public virtual ICollection<MediaLibrary> MediaLibraries { get; set; } = [];

    [InverseProperty("PollGroup")]
    public virtual ICollection<PollsPoll> PollsPolls { get; set; } = [];
}
