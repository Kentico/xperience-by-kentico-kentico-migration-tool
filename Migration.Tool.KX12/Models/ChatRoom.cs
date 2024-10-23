using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[Table("Chat_Room")]
[Index("ChatRoomCreatedByChatUserId", Name = "IX_Chat_Room_ChatRoomCreatedByChatUserID")]
[Index("ChatRoomEnabled", Name = "IX_Chat_Room_Enabled")]
[Index("ChatRoomIsSupport", Name = "IX_Chat_Room_IsSupport")]
[Index("ChatRoomSiteId", Name = "IX_Chat_Room_SiteID")]
public class ChatRoom
{
    [Key]
    [Column("ChatRoomID")]
    public int ChatRoomId { get; set; }

    [StringLength(100)]
    public string ChatRoomName { get; set; } = null!;

    [StringLength(100)]
    public string ChatRoomDisplayName { get; set; } = null!;

    [Column("ChatRoomSiteID")]
    public int? ChatRoomSiteId { get; set; }

    public bool ChatRoomEnabled { get; set; }

    public bool ChatRoomPrivate { get; set; }

    public bool ChatRoomAllowAnonym { get; set; }

    public DateTime ChatRoomCreatedWhen { get; set; }

    [StringLength(100)]
    public string? ChatRoomPassword { get; set; }

    [Column("ChatRoomCreatedByChatUserID")]
    public int? ChatRoomCreatedByChatUserId { get; set; }

    public bool ChatRoomIsSupport { get; set; }

    public bool ChatRoomIsOneToOne { get; set; }

    [StringLength(500)]
    public string? ChatRoomDescription { get; set; }

    public DateTime ChatRoomLastModification { get; set; }

    public DateTime? ChatRoomScheduledToDelete { get; set; }

    public DateTime ChatRoomPrivateStateLastModification { get; set; }

    [Column("ChatRoomGUID")]
    public Guid ChatRoomGuid { get; set; }

    [InverseProperty("InitiatedChatRequestRoom")]
    public virtual ChatInitiatedChatRequest? ChatInitiatedChatRequest { get; set; }

    [InverseProperty("ChatMessageRoom")]
    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    [InverseProperty("ChatNotificationRoom")]
    public virtual ICollection<ChatNotification> ChatNotifications { get; set; } = new List<ChatNotification>();

    [ForeignKey("ChatRoomCreatedByChatUserId")]
    [InverseProperty("ChatRooms")]
    public virtual ChatUser? ChatRoomCreatedByChatUser { get; set; }

    [ForeignKey("ChatRoomSiteId")]
    [InverseProperty("ChatRooms")]
    public virtual CmsSite? ChatRoomSite { get; set; }

    [InverseProperty("ChatRoomUserRoom")]
    public virtual ICollection<ChatRoomUser> ChatRoomUsers { get; set; } = new List<ChatRoomUser>();

    [InverseProperty("ChatSupportTakenRoomRoom")]
    public virtual ICollection<ChatSupportTakenRoom> ChatSupportTakenRooms { get; set; } = new List<ChatSupportTakenRoom>();
}
