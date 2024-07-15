using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("Community_Invitation")]
[Index("InvitationGroupId", Name = "IX_Community_Invitation_InvitationGroupID")]
[Index("InvitedByUserId", Name = "IX_Community_Invitation_InvitedByUserID")]
[Index("InvitedUserId", Name = "IX_Community_Invitation_InvitedUserID")]
public partial class CommunityInvitation
{
    [Key]
    [Column("InvitationID")]
    public int InvitationId { get; set; }

    [Column("InvitedUserID")]
    public int? InvitedUserId { get; set; }

    [Column("InvitedByUserID")]
    public int InvitedByUserId { get; set; }

    [Column("InvitationGroupID")]
    public int? InvitationGroupId { get; set; }

    public DateTime? InvitationCreated { get; set; }

    public DateTime? InvitationValidTo { get; set; }

    public string? InvitationComment { get; set; }

    [Column("InvitationGUID")]
    public Guid InvitationGuid { get; set; }

    public DateTime InvitationLastModified { get; set; }

    [StringLength(254)]
    public string? InvitationUserEmail { get; set; }

    [ForeignKey("InvitationGroupId")]
    [InverseProperty("CommunityInvitations")]
    public virtual CommunityGroup? InvitationGroup { get; set; }

    [ForeignKey("InvitedByUserId")]
    [InverseProperty("CommunityInvitationInvitedByUsers")]
    public virtual CmsUser InvitedByUser { get; set; } = null!;

    [ForeignKey("InvitedUserId")]
    [InverseProperty("CommunityInvitationInvitedUsers")]
    public virtual CmsUser? InvitedUser { get; set; }
}