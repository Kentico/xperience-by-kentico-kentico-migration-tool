using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[Table("Chat_User")]
[Index("ChatUserUserId", Name = "IX_Chat_User_UserID")]
public class ChatUser
{
    [Key]
    [Column("ChatUserID")]
    public int ChatUserId { get; set; }

    [Column("ChatUserUserID")]
    public int? ChatUserUserId { get; set; }

    [StringLength(50)]
    public string ChatUserNickname { get; set; } = null!;

    public DateTime ChatUserLastModification { get; set; }

    [InverseProperty("InitiatedChatRequestInitiatorChatUser")]
    public virtual ICollection<ChatInitiatedChatRequest> ChatInitiatedChatRequests { get; set; } = [];

    [InverseProperty("ChatMessageRecipient")]
    public virtual ICollection<ChatMessage> ChatMessageChatMessageRecipients { get; set; } = [];

    [InverseProperty("ChatMessageUser")]
    public virtual ICollection<ChatMessage> ChatMessageChatMessageUsers { get; set; } = [];

    [InverseProperty("ChatNotificationReceiver")]
    public virtual ICollection<ChatNotification> ChatNotificationChatNotificationReceivers { get; set; } = [];

    [InverseProperty("ChatNotificationSender")]
    public virtual ICollection<ChatNotification> ChatNotificationChatNotificationSenders { get; set; } = [];

    [InverseProperty("ChatOnlineSupportChatUser")]
    public virtual ICollection<ChatOnlineSupport> ChatOnlineSupports { get; set; } = [];

    [InverseProperty("ChatOnlineUserChatUser")]
    public virtual ICollection<ChatOnlineUser> ChatOnlineUsers { get; set; } = [];

    [InverseProperty("ChatRoomUserChatUser")]
    public virtual ICollection<ChatRoomUser> ChatRoomUsers { get; set; } = [];

    [InverseProperty("ChatRoomCreatedByChatUser")]
    public virtual ICollection<ChatRoom> ChatRooms { get; set; } = [];

    [InverseProperty("ChatSupportCannedResponseChatUser")]
    public virtual ICollection<ChatSupportCannedResponse> ChatSupportCannedResponses { get; set; } = [];

    [InverseProperty("ChatSupportTakenRoomChatUser")]
    public virtual ICollection<ChatSupportTakenRoom> ChatSupportTakenRooms { get; set; } = [];

    [ForeignKey("ChatUserUserId")]
    [InverseProperty("ChatUsers")]
    public virtual CmsUser? ChatUserUser { get; set; }
}
