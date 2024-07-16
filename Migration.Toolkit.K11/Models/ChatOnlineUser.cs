using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Chat_OnlineUser")]
[Index("ChatOnlineUserChatUserId", Name = "IX_Chat_OnlineUser_ChatOnlineUserChatUserID")]
[Index("ChatOnlineUserSiteId", Name = "IX_Chat_OnlineUser_SiteID")]
[Index("ChatOnlineUserChatUserId", "ChatOnlineUserSiteId", Name = "UQ_Chat_OnlineUser_SiteID-ChatUserID", IsUnique = true)]
public class ChatOnlineUser
{
    [Key]
    [Column("ChatOnlineUserID")]
    public int ChatOnlineUserId { get; set; }

    [Column("ChatOnlineUserSiteID")]
    public int ChatOnlineUserSiteId { get; set; }

    public DateTime? ChatOnlineUserLastChecking { get; set; }

    [Column("ChatOnlineUserChatUserID")]
    public int ChatOnlineUserChatUserId { get; set; }

    public DateTime? ChatOnlineUserJoinTime { get; set; }

    public DateTime? ChatOnlineUserLeaveTime { get; set; }

    [StringLength(50)]
    public string? ChatOnlineUserToken { get; set; }

    public bool ChatOnlineUserIsHidden { get; set; }

    [ForeignKey("ChatOnlineUserChatUserId")]
    [InverseProperty("ChatOnlineUsers")]
    public virtual ChatUser ChatOnlineUserChatUser { get; set; } = null!;

    [ForeignKey("ChatOnlineUserSiteId")]
    [InverseProperty("ChatOnlineUsers")]
    public virtual CmsSite ChatOnlineUserSite { get; set; } = null!;
}
