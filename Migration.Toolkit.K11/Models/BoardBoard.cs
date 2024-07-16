using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Board_Board")]
[Index("BoardDocumentId", "BoardName", Name = "IX_Board_Board_BoardDocumentID_BoardName", IsUnique = true)]
[Index("BoardGroupId", "BoardName", Name = "IX_Board_Board_BoardGroupID_BoardName")]
[Index("BoardSiteId", Name = "IX_Board_Board_BoardSiteID")]
[Index("BoardUserId", "BoardName", Name = "IX_Board_Board_BoardUserID_BoardName")]
public class BoardBoard
{
    [Key]
    [Column("BoardID")]
    public int BoardId { get; set; }

    [StringLength(250)]
    public string BoardName { get; set; } = null!;

    [StringLength(250)]
    public string BoardDisplayName { get; set; } = null!;

    public string BoardDescription { get; set; } = null!;

    public bool BoardOpened { get; set; }

    public DateTime? BoardOpenedFrom { get; set; }

    public DateTime? BoardOpenedTo { get; set; }

    public bool BoardEnabled { get; set; }

    public int BoardAccess { get; set; }

    public bool BoardModerated { get; set; }

    public bool BoardUseCaptcha { get; set; }

    public int BoardMessages { get; set; }

    public DateTime BoardLastModified { get; set; }

    [Column("BoardGUID")]
    public Guid BoardGuid { get; set; }

    [Column("BoardDocumentID")]
    public int BoardDocumentId { get; set; }

    [Column("BoardUserID")]
    public int? BoardUserId { get; set; }

    [Column("BoardGroupID")]
    public int? BoardGroupId { get; set; }

    public DateTime? BoardLastMessageTime { get; set; }

    [StringLength(250)]
    public string? BoardLastMessageUserName { get; set; }

    [Column("BoardUnsubscriptionURL")]
    [StringLength(450)]
    public string? BoardUnsubscriptionUrl { get; set; }

    public bool? BoardRequireEmails { get; set; }

    [Column("BoardSiteID")]
    public int BoardSiteId { get; set; }

    public bool BoardEnableSubscriptions { get; set; }

    [Column("BoardBaseURL")]
    [StringLength(450)]
    public string? BoardBaseUrl { get; set; }

    public bool? BoardLogActivity { get; set; }

    public bool? BoardEnableOptIn { get; set; }

    public bool? BoardSendOptInConfirmation { get; set; }

    [Column("BoardOptInApprovalURL")]
    [StringLength(450)]
    public string? BoardOptInApprovalUrl { get; set; }

    [ForeignKey("BoardDocumentId")]
    [InverseProperty("BoardBoards")]
    public virtual CmsDocument BoardDocument { get; set; } = null!;

    [ForeignKey("BoardGroupId")]
    [InverseProperty("BoardBoards")]
    public virtual CommunityGroup? BoardGroup { get; set; }

    [InverseProperty("MessageBoard")]
    public virtual ICollection<BoardMessage> BoardMessagesNavigation { get; set; } = new List<BoardMessage>();

    [ForeignKey("BoardSiteId")]
    [InverseProperty("BoardBoards")]
    public virtual CmsSite BoardSite { get; set; } = null!;

    [InverseProperty("SubscriptionBoard")]
    public virtual ICollection<BoardSubscription> BoardSubscriptions { get; set; } = new List<BoardSubscription>();

    [ForeignKey("BoardUserId")]
    [InverseProperty("BoardBoards")]
    public virtual CmsUser? BoardUser { get; set; }

    [ForeignKey("BoardId")]
    [InverseProperty("Boards")]
    public virtual ICollection<CmsRole> Roles { get; set; } = new List<CmsRole>();

    [ForeignKey("BoardId")]
    [InverseProperty("Boards")]
    public virtual ICollection<CmsUser> Users { get; set; } = new List<CmsUser>();
}
