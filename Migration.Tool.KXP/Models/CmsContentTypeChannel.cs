using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KXP.Models;

[Table("CMS_ContentTypeChannel")]
[Index("ContentTypeChannelChannelId", "ContentTypeChannelContentTypeId", Name = "IX_CMS_ContentTypeChannel_ContentTypeChannelChannelID_ContentTypeChannelContentTypeID", IsUnique = true)]
[Index("ContentTypeChannelContentTypeId", Name = "IX_CMS_ContentTypeChannel_ContentTypeChannelContentTypeID")]
public class CmsContentTypeChannel
{
    [Key]
    [Column("ContentTypeChannelID")]
    public int ContentTypeChannelId { get; set; }

    [Column("ContentTypeChannelChannelID")]
    public int ContentTypeChannelChannelId { get; set; }

    [Column("ContentTypeChannelContentTypeID")]
    public int ContentTypeChannelContentTypeId { get; set; }

    [ForeignKey("ContentTypeChannelChannelId")]
    [InverseProperty("CmsContentTypeChannels")]
    public virtual CmsChannel ContentTypeChannelChannel { get; set; } = null!;

    [ForeignKey("ContentTypeChannelContentTypeId")]
    [InverseProperty("CmsContentTypeChannels")]
    public virtual CmsClass ContentTypeChannelContentType { get; set; } = null!;
}
