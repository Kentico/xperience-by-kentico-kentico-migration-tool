using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Messaging_Message")]
[Index("MessageSenderUserId", "MessageSent", "MessageSenderDeleted", Name = "IX_Messaging_Message_MessageSenderUserID_MessageSent_MessageSenderDeleted")]
public partial class MessagingMessage
{
    [Key]
    [Column("MessageID")]
    public int MessageId { get; set; }

    [Column("MessageSenderUserID")]
    public int? MessageSenderUserId { get; set; }

    [StringLength(200)]
    public string? MessageSenderNickName { get; set; }

    [Column("MessageRecipientUserID")]
    public int? MessageRecipientUserId { get; set; }

    [StringLength(200)]
    public string? MessageRecipientNickName { get; set; }

    public DateTime MessageSent { get; set; }

    [StringLength(200)]
    public string? MessageSubject { get; set; }

    public string MessageBody { get; set; } = null!;

    public DateTime? MessageRead { get; set; }

    public bool? MessageSenderDeleted { get; set; }

    public bool? MessageRecipientDeleted { get; set; }

    [Column("MessageGUID")]
    public Guid MessageGuid { get; set; }

    public DateTime MessageLastModified { get; set; }

    public bool? MessageIsRead { get; set; }

    [ForeignKey("MessageRecipientUserId")]
    [InverseProperty("MessagingMessageMessageRecipientUsers")]
    public virtual CmsUser? MessageRecipientUser { get; set; }

    [ForeignKey("MessageSenderUserId")]
    [InverseProperty("MessagingMessageMessageSenderUsers")]
    public virtual CmsUser? MessageSenderUser { get; set; }
}