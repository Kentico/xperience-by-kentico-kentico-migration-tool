using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models;

[Table("CMS_HeadlessChannel")]
[Index("HeadlessChannelChannelId", Name = "IX_CMS_HeadlessChannel_HeadlessChannelChannelID")]
[Index("HeadlessChannelPrimaryContentLanguageId", Name = "IX_CMS_HeadlessChannel_HeadlessChannelPrimaryContentLanguageID")]
public partial class CmsHeadlessChannel
{
    [Key]
    [Column("HeadlessChannelID")]
    public int HeadlessChannelId { get; set; }

    [Column("HeadlessChannelGUID")]
    public Guid HeadlessChannelGuid { get; set; }

    [Column("HeadlessChannelChannelID")]
    public int HeadlessChannelChannelId { get; set; }

    [Column("HeadlessChannelPrimaryContentLanguageID")]
    public int HeadlessChannelPrimaryContentLanguageId { get; set; }

    [Column("HeadlessChannelPreviewURL")]
    [StringLength(200)]
    public string? HeadlessChannelPreviewUrl { get; set; }

    [InverseProperty("HeadlessItemHeadlessChannel")]
    public virtual ICollection<CmsHeadlessItem> CmsHeadlessItems { get; set; } = new List<CmsHeadlessItem>();

    [InverseProperty("HeadlessTokenHeadlessChannel")]
    public virtual ICollection<CmsHeadlessToken> CmsHeadlessTokens { get; set; } = new List<CmsHeadlessToken>();

    [ForeignKey("HeadlessChannelChannelId")]
    [InverseProperty("CmsHeadlessChannels")]
    public virtual CmsChannel HeadlessChannelChannel { get; set; } = null!;

    [ForeignKey("HeadlessChannelPrimaryContentLanguageId")]
    [InverseProperty("CmsHeadlessChannels")]
    public virtual CmsContentLanguage HeadlessChannelPrimaryContentLanguage { get; set; } = null!;
}