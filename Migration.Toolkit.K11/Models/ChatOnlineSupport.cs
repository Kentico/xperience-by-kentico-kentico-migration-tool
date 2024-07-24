using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Chat_OnlineSupport")]
[Index("ChatOnlineSupportChatUserId", Name = "IX_Chat_OnlineSupport_ChatOnlineSupportChatUserID")]
[Index("ChatOnlineSupportSiteId", Name = "IX_Chat_OnlineSupport_SiteID")]
[Index("ChatOnlineSupportChatUserId", "ChatOnlineSupportSiteId", Name = "UQ_Chat_OnlineSupport_ChatUserID-SiteID", IsUnique = true)]
public class ChatOnlineSupport
{
    [Key]
    [Column("ChatOnlineSupportID")]
    public int ChatOnlineSupportId { get; set; }

    [Column("ChatOnlineSupportChatUserID")]
    public int ChatOnlineSupportChatUserId { get; set; }

    public DateTime ChatOnlineSupportLastChecking { get; set; }

    [Column("ChatOnlineSupportSiteID")]
    public int ChatOnlineSupportSiteId { get; set; }

    [StringLength(50)]
    public string? ChatOnlineSupportToken { get; set; }

    [ForeignKey("ChatOnlineSupportChatUserId")]
    [InverseProperty("ChatOnlineSupports")]
    public virtual ChatUser ChatOnlineSupportChatUser { get; set; } = null!;

    [ForeignKey("ChatOnlineSupportSiteId")]
    [InverseProperty("ChatOnlineSupports")]
    public virtual CmsSite ChatOnlineSupportSite { get; set; } = null!;
}
