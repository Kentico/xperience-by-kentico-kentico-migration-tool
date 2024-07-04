using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models;

[Table("CMS_HeadlessToken")]
[Index("HeadlessTokenCreatedByUserId", Name = "IX_CMS_HeadlessToken_HeadlessTokenCreatedByUserID")]
[Index("HeadlessTokenHash", Name = "IX_CMS_HeadlessToken_HeadlessTokenHash")]
[Index("HeadlessTokenHeadlessChannelId", Name = "IX_CMS_HeadlessToken_HeadlessTokenHeadlessChannelID")]
[Index("HeadlessTokenModifiedByUserId", Name = "IX_CMS_HeadlessToken_HeadlessTokenModifiedByUserID")]
public partial class CmsHeadlessToken
{
    [Key]
    [Column("HeadlessTokenID")]
    public int HeadlessTokenId { get; set; }

    [Column("HeadlessTokenGUID")]
    public Guid HeadlessTokenGuid { get; set; }

    [Column("HeadlessTokenHeadlessChannelID")]
    public int HeadlessTokenHeadlessChannelId { get; set; }

    [StringLength(200)]
    public string HeadlessTokenDisplayName { get; set; } = null!;

    [Required]
    public bool? HeadlessTokenEnabled { get; set; }

    public DateTime HeadlessTokenCreatedWhen { get; set; }

    [Column("HeadlessTokenCreatedByUserID")]
    public int? HeadlessTokenCreatedByUserId { get; set; }

    public DateTime HeadlessTokenModifiedWhen { get; set; }

    [Column("HeadlessTokenModifiedByUserID")]
    public int? HeadlessTokenModifiedByUserId { get; set; }

    [StringLength(64)]
    public string HeadlessTokenHash { get; set; } = null!;

    [StringLength(200)]
    public string HeadlessTokenAccessType { get; set; } = null!;

    [ForeignKey("HeadlessTokenCreatedByUserId")]
    [InverseProperty("CmsHeadlessTokenHeadlessTokenCreatedByUsers")]
    public virtual CmsUser? HeadlessTokenCreatedByUser { get; set; }

    [ForeignKey("HeadlessTokenHeadlessChannelId")]
    [InverseProperty("CmsHeadlessTokens")]
    public virtual CmsHeadlessChannel HeadlessTokenHeadlessChannel { get; set; } = null!;

    [ForeignKey("HeadlessTokenModifiedByUserId")]
    [InverseProperty("CmsHeadlessTokenHeadlessTokenModifiedByUsers")]
    public virtual CmsUser? HeadlessTokenModifiedByUser { get; set; }
}