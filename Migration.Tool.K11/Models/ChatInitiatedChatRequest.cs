using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("Chat_InitiatedChatRequest")]
[Index("InitiatedChatRequestInitiatorChatUserId", Name = "IX_Chat_InitiatedChatRequest_InitiatedChatRequestInitiatorChatUserID")]
[Index("InitiatedChatRequestUserId", Name = "IX_Chat_InitiatedChatRequest_InitiatedChatRequestUserID")]
[Index("InitiatedChatRequestRoomId", Name = "UQ_Chat_InitiatedChatRequest_RoomID", IsUnique = true)]
[Index("InitiatedChatRequestUserId", "InitiatedChatRequestContactId", Name = "UQ_Chat_InitiatedChatRequest_UserIDContactID", IsUnique = true)]
public class ChatInitiatedChatRequest
{
    [Key]
    [Column("InitiatedChatRequestID")]
    public int InitiatedChatRequestId { get; set; }

    [Column("InitiatedChatRequestUserID")]
    public int? InitiatedChatRequestUserId { get; set; }

    [Column("InitiatedChatRequestContactID")]
    public int? InitiatedChatRequestContactId { get; set; }

    [Column("InitiatedChatRequestRoomID")]
    public int InitiatedChatRequestRoomId { get; set; }

    public int InitiatedChatRequestState { get; set; }

    [StringLength(100)]
    public string InitiatedChatRequestInitiatorName { get; set; } = null!;

    [Column("InitiatedChatRequestInitiatorChatUserID")]
    public int InitiatedChatRequestInitiatorChatUserId { get; set; }

    public DateTime InitiatedChatRequestLastModification { get; set; }

    [ForeignKey("InitiatedChatRequestInitiatorChatUserId")]
    [InverseProperty("ChatInitiatedChatRequests")]
    public virtual ChatUser InitiatedChatRequestInitiatorChatUser { get; set; } = null!;

    [ForeignKey("InitiatedChatRequestRoomId")]
    [InverseProperty("ChatInitiatedChatRequest")]
    public virtual ChatRoom InitiatedChatRequestRoom { get; set; } = null!;

    [ForeignKey("InitiatedChatRequestUserId")]
    [InverseProperty("ChatInitiatedChatRequests")]
    public virtual CmsUser? InitiatedChatRequestUser { get; set; }
}
