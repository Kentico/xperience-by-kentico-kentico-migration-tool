﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.KXP.Models;

[Table("CMS_Channel")]
public class CmsChannel
{
    [Key]
    [Column("ChannelID")]
    public int ChannelId { get; set; }

    [StringLength(200)]
    public string ChannelDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string ChannelName { get; set; } = null!;

    [StringLength(200)]
    public string ChannelType { get; set; } = null!;

    [Column("ChannelGUID")]
    public Guid ChannelGuid { get; set; }

    [StringLength(20)]
    public string ChannelSize { get; set; } = null!;

    [InverseProperty("ContentItemChannel")]
    public virtual ICollection<CmsContentItem> CmsContentItems { get; set; } = new List<CmsContentItem>();

    [InverseProperty("ContentTypeChannelChannel")]
    public virtual ICollection<CmsContentTypeChannel> CmsContentTypeChannels { get; set; } = new List<CmsContentTypeChannel>();

    [InverseProperty("HeadlessChannelChannel")]
    public virtual ICollection<CmsHeadlessChannel> CmsHeadlessChannels { get; set; } = new List<CmsHeadlessChannel>();

    [InverseProperty("WebsiteChannelChannel")]
    public virtual ICollection<CmsWebsiteChannel> CmsWebsiteChannels { get; set; } = new List<CmsWebsiteChannel>();

    [InverseProperty("EmailChannelChannel")]
    public virtual ICollection<EmailLibraryEmailChannel> EmailLibraryEmailChannels { get; set; } = new List<EmailLibraryEmailChannel>();

    [InverseProperty("ActivityChannel")]
    public virtual ICollection<OmActivity> OmActivities { get; set; } = new List<OmActivity>();
}
