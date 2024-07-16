using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Chat_Message")]
[Index("ChatMessageLastModified", Name = "IX_Chat_Message_ChatMessageLastModified")]
[Index("ChatMessageRecipientId", Name = "IX_Chat_Message_ChatMessageRecipientID")]
[Index("ChatMessageRoomId", Name = "IX_Chat_Message_ChatMessageRoomID")]
[Index("ChatMessageSystemMessageType", Name = "IX_Chat_Message_ChatMessageSystemMessageType")]
[Index("ChatMessageUserId", Name = "IX_Chat_Message_ChatMessageUserID")]
public partial class ChatMessage
{
    [Key]
    [Column("ChatMessageID")]
    public int ChatMessageId { get; set; }

    public DateTime ChatMessageCreatedWhen { get; set; }

    [Column("ChatMessageIPAddress")]
    public string ChatMessageIpaddress { get; set; } = null!;

    [Column("ChatMessageUserID")]
    public int? ChatMessageUserId { get; set; }

    [Column("ChatMessageRoomID")]
    public int ChatMessageRoomId { get; set; }

    public bool ChatMessageRejected { get; set; }

    public DateTime ChatMessageLastModified { get; set; }

    public string ChatMessageText { get; set; } = null!;

    public int ChatMessageSystemMessageType { get; set; }

    [Column("ChatMessageRecipientID")]
    public int? ChatMessageRecipientId { get; set; }

    [ForeignKey("ChatMessageRecipientId")]
    [InverseProperty("ChatMessageChatMessageRecipients")]
    public virtual ChatUser? ChatMessageRecipient { get; set; }

    [ForeignKey("ChatMessageRoomId")]
    [InverseProperty("ChatMessages")]
    public virtual ChatRoom ChatMessageRoom { get; set; } = null!;

    [ForeignKey("ChatMessageUserId")]
    [InverseProperty("ChatMessageChatMessageUsers")]
    public virtual ChatUser? ChatMessageUser { get; set; }
}
