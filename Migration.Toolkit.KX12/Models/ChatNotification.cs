using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("Chat_Notification")]
[Index("ChatNotificationReceiverId", Name = "IX_Chat_Notification_ChatNotificationReceiverID")]
[Index("ChatNotificationRoomId", Name = "IX_Chat_Notification_ChatNotificationRoomID")]
[Index("ChatNotificationSenderId", Name = "IX_Chat_Notification_ChatNotificationSenderID")]
[Index("ChatNotificationSiteId", Name = "IX_Chat_Notification_ChatNotificationSiteID")]
public class ChatNotification
{
    [Key]
    [Column("ChatNotificationID")]
    public int ChatNotificationId { get; set; }

    [Column("ChatNotificationSenderID")]
    public int ChatNotificationSenderId { get; set; }

    [Column("ChatNotificationReceiverID")]
    public int ChatNotificationReceiverId { get; set; }

    public bool ChatNotificationIsRead { get; set; }

    public int ChatNotificationType { get; set; }

    [Column("ChatNotificationRoomID")]
    public int? ChatNotificationRoomId { get; set; }

    public DateTime ChatNotificationSendDateTime { get; set; }

    public DateTime? ChatNotificationReadDateTime { get; set; }

    [Column("ChatNotificationSiteID")]
    public int? ChatNotificationSiteId { get; set; }

    [ForeignKey("ChatNotificationReceiverId")]
    [InverseProperty("ChatNotificationChatNotificationReceivers")]
    public virtual ChatUser ChatNotificationReceiver { get; set; } = null!;

    [ForeignKey("ChatNotificationRoomId")]
    [InverseProperty("ChatNotifications")]
    public virtual ChatRoom? ChatNotificationRoom { get; set; }

    [ForeignKey("ChatNotificationSenderId")]
    [InverseProperty("ChatNotificationChatNotificationSenders")]
    public virtual ChatUser ChatNotificationSender { get; set; } = null!;

    [ForeignKey("ChatNotificationSiteId")]
    [InverseProperty("ChatNotifications")]
    public virtual CmsSite? ChatNotificationSite { get; set; }
}
