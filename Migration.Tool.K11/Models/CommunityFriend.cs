using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("Community_Friend")]
[Index("FriendApprovedBy", Name = "IX_Community_Friend_FriendApprovedBy")]
[Index("FriendRejectedBy", Name = "IX_Community_Friend_FriendRejectedBy")]
[Index("FriendRequestedUserId", "FriendStatus", Name = "IX_Community_Friend_FriendRequestedUserID_FriendStatus")]
[Index("FriendRequestedUserId", "FriendUserId", Name = "IX_Community_Friend_FriendRequestedUserID_FriendUserID", IsUnique = true)]
[Index("FriendUserId", "FriendStatus", Name = "IX_Community_Friend_FriendUserID_FriendStatus")]
public class CommunityFriend
{
    [Key]
    [Column("FriendID")]
    public int FriendId { get; set; }

    [Column("FriendRequestedUserID")]
    public int FriendRequestedUserId { get; set; }

    [Column("FriendUserID")]
    public int FriendUserId { get; set; }

    public DateTime FriendRequestedWhen { get; set; }

    public string? FriendComment { get; set; }

    public int? FriendApprovedBy { get; set; }

    public DateTime? FriendApprovedWhen { get; set; }

    public int? FriendRejectedBy { get; set; }

    public DateTime? FriendRejectedWhen { get; set; }

    [Column("FriendGUID")]
    public Guid FriendGuid { get; set; }

    public int FriendStatus { get; set; }

    [ForeignKey("FriendApprovedBy")]
    [InverseProperty("CommunityFriendFriendApprovedByNavigations")]
    public virtual CmsUser? FriendApprovedByNavigation { get; set; }

    [ForeignKey("FriendRejectedBy")]
    [InverseProperty("CommunityFriendFriendRejectedByNavigations")]
    public virtual CmsUser? FriendRejectedByNavigation { get; set; }

    [ForeignKey("FriendRequestedUserId")]
    [InverseProperty("CommunityFriendFriendRequestedUsers")]
    public virtual CmsUser FriendRequestedUser { get; set; } = null!;

    [ForeignKey("FriendUserId")]
    [InverseProperty("CommunityFriendFriendUsers")]
    public virtual CmsUser FriendUser { get; set; } = null!;
}
