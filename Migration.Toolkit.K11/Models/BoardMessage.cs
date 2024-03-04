using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Board_Message")]
[Index("MessageApprovedByUserId", Name = "IX_Board_Message_MessageApprovedByUserID")]
[Index("MessageApproved", "MessageIsSpam", Name = "IX_Board_Message_MessageApproved_MessageIsSpam")]
[Index("MessageBoardId", "MessageGuid", Name = "IX_Board_Message_MessageBoardID_MessageGUID", IsUnique = true)]
[Index("MessageUserId", Name = "IX_Board_Message_MessageUserID")]
public partial class BoardMessage
{
    [Key]
    [Column("MessageID")]
    public int MessageId { get; set; }

    [StringLength(250)]
    public string MessageUserName { get; set; } = null!;

    public string MessageText { get; set; } = null!;

    [StringLength(254)]
    public string MessageEmail { get; set; } = null!;

    [Column("MessageURL")]
    [StringLength(450)]
    public string MessageUrl { get; set; } = null!;

    public bool MessageIsSpam { get; set; }

    [Column("MessageBoardID")]
    public int MessageBoardId { get; set; }

    public bool MessageApproved { get; set; }

    [Column("MessageApprovedByUserID")]
    public int? MessageApprovedByUserId { get; set; }

    [Column("MessageUserID")]
    public int? MessageUserId { get; set; }

    public string MessageUserInfo { get; set; } = null!;

    [Column("MessageAvatarGUID")]
    public Guid? MessageAvatarGuid { get; set; }

    public DateTime MessageInserted { get; set; }

    public DateTime MessageLastModified { get; set; }

    [Column("MessageGUID")]
    public Guid MessageGuid { get; set; }

    public double? MessageRatingValue { get; set; }

    [ForeignKey("MessageApprovedByUserId")]
    [InverseProperty("BoardMessageMessageApprovedByUsers")]
    public virtual CmsUser? MessageApprovedByUser { get; set; }

    [ForeignKey("MessageBoardId")]
    [InverseProperty("BoardMessagesNavigation")]
    public virtual BoardBoard MessageBoard { get; set; } = null!;

    [ForeignKey("MessageUserId")]
    [InverseProperty("BoardMessageMessageUsers")]
    public virtual CmsUser? MessageUser { get; set; }
}
