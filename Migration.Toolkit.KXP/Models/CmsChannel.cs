using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_Channel")]
    public partial class CmsChannel
    {
        public CmsChannel()
        {
            CmsContentItems = new HashSet<CmsContentItem>();
            CmsContentTypeChannels = new HashSet<CmsContentTypeChannel>();
            CmsWebsiteChannels = new HashSet<CmsWebsiteChannel>();
            EmailLibraryEmailChannels = new HashSet<EmailLibraryEmailChannel>();
            OmActivities = new HashSet<OmActivity>();
        }

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

        [InverseProperty("ContentItemChannel")]
        public virtual ICollection<CmsContentItem> CmsContentItems { get; set; }
        [InverseProperty("ContentTypeChannelChannel")]
        public virtual ICollection<CmsContentTypeChannel> CmsContentTypeChannels { get; set; }
        [InverseProperty("WebsiteChannelChannel")]
        public virtual ICollection<CmsWebsiteChannel> CmsWebsiteChannels { get; set; }
        [InverseProperty("EmailChannelChannel")]
        public virtual ICollection<EmailLibraryEmailChannel> EmailLibraryEmailChannels { get; set; }
        [InverseProperty("ActivityChannel")]
        public virtual ICollection<OmActivity> OmActivities { get; set; }
    }
}
