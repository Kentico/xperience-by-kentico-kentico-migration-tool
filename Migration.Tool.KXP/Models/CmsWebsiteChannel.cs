using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KXP.Models;

[Table("CMS_WebsiteChannel")]
[Index("WebsiteChannelChannelId", Name = "IX_CMS_WebsiteChannel_WebsiteChannelChannelID")]
[Index("WebsiteChannelPrimaryContentLanguageId", Name = "IX_CMS_WebsiteChannel_WebsiteChannelPrimaryContentLanguageID")]
public class CmsWebsiteChannel
{
    [Key]
    [Column("WebsiteChannelID")]
    public int WebsiteChannelId { get; set; }

    [Column("WebsiteChannelGUID")]
    public Guid WebsiteChannelGuid { get; set; }

    [StringLength(400)]
    public string WebsiteChannelDomain { get; set; } = null!;

    [Column("WebsiteChannelChannelID")]
    public int WebsiteChannelChannelId { get; set; }

    [StringLength(200)]
    public string? WebsiteChannelHomePage { get; set; }

    [Column("WebsiteChannelPrimaryContentLanguageID")]
    public int WebsiteChannelPrimaryContentLanguageId { get; set; }

    public int WebsiteChannelDefaultCookieLevel { get; set; }

    public bool WebsiteChannelStoreFormerUrls { get; set; }

    [InverseProperty("WebPageFormerUrlPathWebsiteChannel")]
    public virtual ICollection<CmsWebPageFormerUrlPath> CmsWebPageFormerUrlPaths { get; set; } = new List<CmsWebPageFormerUrlPath>();

    [InverseProperty("WebPageItemWebsiteChannel")]
    public virtual ICollection<CmsWebPageItem> CmsWebPageItems { get; set; } = new List<CmsWebPageItem>();

    [InverseProperty("WebPageUrlPathWebsiteChannel")]
    public virtual ICollection<CmsWebPageUrlPath> CmsWebPageUrlPaths { get; set; } = new List<CmsWebPageUrlPath>();

    [ForeignKey("WebsiteChannelChannelId")]
    [InverseProperty("CmsWebsiteChannels")]
    public virtual CmsChannel WebsiteChannelChannel { get; set; } = null!;

    [ForeignKey("WebsiteChannelPrimaryContentLanguageId")]
    [InverseProperty("CmsWebsiteChannels")]
    public virtual CmsContentLanguage WebsiteChannelPrimaryContentLanguage { get; set; } = null!;
}
