using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("Chat_User")]
[Index("ChatUserUserId", Name = "IX_Chat_User_UserID")]
public partial class ChatUser
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
    public virtual ICollection<ChatInitiatedChatRequest> ChatInitiatedChatRequests { get; set; } = new List<ChatInitiatedChatRequest>();

    [InverseProperty("ChatMessageRecipient")]
    public virtual ICollection<ChatMessage> ChatMessageChatMessageRecipients { get; set; } = new List<ChatMessage>();

    [InverseProperty("ChatMessageUser")]
    public virtual ICollection<ChatMessage> ChatMessageChatMessageUsers { get; set; } = new List<ChatMessage>();

    [InverseProperty("ChatNotificationReceiver")]
    public virtual ICollection<ChatNotification> ChatNotificationChatNotificationReceivers { get; set; } = new List<ChatNotification>();

    [InverseProperty("ChatNotificationSender")]
    public virtual ICollection<ChatNotification> ChatNotificationChatNotificationSenders { get; set; } = new List<ChatNotification>();

    [InverseProperty("ChatOnlineSupportChatUser")]
    public virtual ICollection<ChatOnlineSupport> ChatOnlineSupports { get; set; } = new List<ChatOnlineSupport>();

    [InverseProperty("ChatOnlineUserChatUser")]
    public virtual ICollection<ChatOnlineUser> ChatOnlineUsers { get; set; } = new List<ChatOnlineUser>();

    [InverseProperty("ChatRoomUserChatUser")]
    public virtual ICollection<ChatRoomUser> ChatRoomUsers { get; set; } = new List<ChatRoomUser>();

    [InverseProperty("ChatRoomCreatedByChatUser")]
    public virtual ICollection<ChatRoom> ChatRooms { get; set; } = new List<ChatRoom>();

    [InverseProperty("ChatSupportCannedResponseChatUser")]
    public virtual ICollection<ChatSupportCannedResponse> ChatSupportCannedResponses { get; set; } = new List<ChatSupportCannedResponse>();

    [InverseProperty("ChatSupportTakenRoomChatUser")]
    public virtual ICollection<ChatSupportTakenRoom> ChatSupportTakenRooms { get; set; } = new List<ChatSupportTakenRoom>();

    [ForeignKey("ChatUserUserId")]
    [InverseProperty("ChatUsers")]
    public virtual CmsUser? ChatUserUser { get; set; }
}
