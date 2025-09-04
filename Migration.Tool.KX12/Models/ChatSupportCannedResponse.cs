using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[Table("Chat_SupportCannedResponse")]
[Index("ChatSupportCannedResponseChatUserId", Name = "IX_Chat_SupportCannedResponse_ChatSupportCannedResponseChatUserID")]
[Index("ChatSupportCannedResponseSiteId", Name = "IX_Chat_SupportCannedResponse_ChatSupportCannedResponseSiteID")]
public class ChatSupportCannedResponse
{
    [Key]
    [Column("ChatSupportCannedResponseID")]
    public int ChatSupportCannedResponseId { get; set; }

    [Column("ChatSupportCannedResponseChatUserID")]
    public int? ChatSupportCannedResponseChatUserId { get; set; }

    [StringLength(500)]
    public string ChatSupportCannedResponseText { get; set; } = null!;

    [StringLength(50)]
    public string ChatSupportCannedResponseTagName { get; set; } = null!;

    [Column("ChatSupportCannedResponseSiteID")]
    public int? ChatSupportCannedResponseSiteId { get; set; }

    [StringLength(100)]
    public string ChatSupportCannedResponseName { get; set; } = null!;

    [ForeignKey("ChatSupportCannedResponseChatUserId")]
    [InverseProperty("ChatSupportCannedResponses")]
    public virtual ChatUser? ChatSupportCannedResponseChatUser { get; set; }

    [ForeignKey("ChatSupportCannedResponseSiteId")]
    [InverseProperty("ChatSupportCannedResponses")]
    public virtual CmsSite? ChatSupportCannedResponseSite { get; set; }
}
