using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Chat_SupportTakenRoom")]
[Index("ChatSupportTakenRoomChatUserId", Name = "IX_Chat_SupportTakenRoom_ChatSupportTakenRoomChatUserID")]
[Index("ChatSupportTakenRoomRoomId", Name = "IX_Chat_SupportTakenRoom_ChatSupportTakenRoomRoomID")]
public partial class ChatSupportTakenRoom
{
    [Key]
    [Column("ChatSupportTakenRoomID")]
    public int ChatSupportTakenRoomId { get; set; }

    [Column("ChatSupportTakenRoomChatUserID")]
    public int? ChatSupportTakenRoomChatUserId { get; set; }

    [Column("ChatSupportTakenRoomRoomID")]
    public int ChatSupportTakenRoomRoomId { get; set; }

    public DateTime? ChatSupportTakenRoomResolvedDateTime { get; set; }

    public DateTime ChatSupportTakenRoomLastModification { get; set; }

    [ForeignKey("ChatSupportTakenRoomChatUserId")]
    [InverseProperty("ChatSupportTakenRooms")]
    public virtual ChatUser? ChatSupportTakenRoomChatUser { get; set; }

    [ForeignKey("ChatSupportTakenRoomRoomId")]
    [InverseProperty("ChatSupportTakenRooms")]
    public virtual ChatRoom ChatSupportTakenRoomRoom { get; set; } = null!;
}