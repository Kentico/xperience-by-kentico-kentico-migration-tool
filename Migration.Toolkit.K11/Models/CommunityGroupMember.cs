using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Community_GroupMember")]
[Index("MemberApprovedByUserId", Name = "IX_Community_GroupMember_MemberApprovedByUserID")]
[Index("MemberGroupId", Name = "IX_Community_GroupMember_MemberGroupID")]
[Index("MemberInvitedByUserId", Name = "IX_Community_GroupMember_MemberInvitedByUserID")]
[Index("MemberStatus", Name = "IX_Community_GroupMember_MemberStatus")]
[Index("MemberUserId", Name = "IX_Community_GroupMember_MemberUserID")]
public partial class CommunityGroupMember
{
    [Key]
    [Column("MemberID")]
    public int MemberId { get; set; }

    [Column("MemberGUID")]
    public Guid MemberGuid { get; set; }

    [Column("MemberUserID")]
    public int MemberUserId { get; set; }

    [Column("MemberGroupID")]
    public int MemberGroupId { get; set; }

    public DateTime MemberJoined { get; set; }

    public DateTime? MemberApprovedWhen { get; set; }

    public DateTime? MemberRejectedWhen { get; set; }

    [Column("MemberApprovedByUserID")]
    public int? MemberApprovedByUserId { get; set; }

    public string? MemberComment { get; set; }

    [Column("MemberInvitedByUserID")]
    public int? MemberInvitedByUserId { get; set; }

    public int? MemberStatus { get; set; }

    [ForeignKey("MemberApprovedByUserId")]
    [InverseProperty("CommunityGroupMemberMemberApprovedByUsers")]
    public virtual CmsUser? MemberApprovedByUser { get; set; }

    [ForeignKey("MemberGroupId")]
    [InverseProperty("CommunityGroupMembers")]
    public virtual CommunityGroup MemberGroup { get; set; } = null!;

    [ForeignKey("MemberInvitedByUserId")]
    [InverseProperty("CommunityGroupMemberMemberInvitedByUsers")]
    public virtual CmsUser? MemberInvitedByUser { get; set; }

    [ForeignKey("MemberUserId")]
    [InverseProperty("CommunityGroupMemberMemberUsers")]
    public virtual CmsUser MemberUser { get; set; } = null!;
}