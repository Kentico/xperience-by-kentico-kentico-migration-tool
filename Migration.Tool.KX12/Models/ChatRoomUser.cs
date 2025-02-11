using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[Table("Chat_RoomUser")]
[Index("ChatRoomUserChatUserId", Name = "IX_Chat_RoomUser_ChatRoomUserChatUserID")]
[Index("ChatRoomUserRoomId", Name = "IX_Chat_RoomUser_ChatRoomUserRoomID")]
[Index("ChatRoomUserRoomId", "ChatRoomUserChatUserId", Name = "UQ_Chat_RoomUser_RoomID-ChatUserID", IsUnique = true)]
public class ChatRoomUser
{
    [Key]
    [Column("ChatRoomUserID")]
    public int ChatRoomUserId { get; set; }

    [Column("ChatRoomUserRoomID")]
    public int ChatRoomUserRoomId { get; set; }

    [Column("ChatRoomUserChatUserID")]
    public int ChatRoomUserChatUserId { get; set; }

    public DateTime? ChatRoomUserLastChecking { get; set; }

    public DateTime? ChatRoomUserKickExpiration { get; set; }

    public DateTime? ChatRoomUserJoinTime { get; set; }

    public DateTime? ChatRoomUserLeaveTime { get; set; }

    public int ChatRoomUserAdminLevel { get; set; }

    public DateTime ChatRoomUserLastModification { get; set; }

    [ForeignKey("ChatRoomUserChatUserId")]
    [InverseProperty("ChatRoomUsers")]
    public virtual ChatUser ChatRoomUserChatUser { get; set; } = null!;

    [ForeignKey("ChatRoomUserRoomId")]
    [InverseProperty("ChatRoomUsers")]
    public virtual ChatRoom ChatRoomUserRoom { get; set; } = null!;
}
