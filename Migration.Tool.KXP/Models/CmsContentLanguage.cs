using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.KXP.Models;

[Table("CMS_ContentLanguage")]
public class CmsContentLanguage
{
    [Key]
    [Column("ContentLanguageID")]
    public int ContentLanguageId { get; set; }

    [StringLength(200)]
    public string ContentLanguageDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string ContentLanguageName { get; set; } = null!;

    public bool ContentLanguageIsDefault { get; set; }

    [Column("ContentLanguageFallbackContentLanguageID")]
    public int? ContentLanguageFallbackContentLanguageId { get; set; }

    [StringLength(200)]
    public string ContentLanguageCultureFormat { get; set; } = null!;

    [Column("ContentLanguageGUID")]
    public Guid ContentLanguageGuid { get; set; }

    [InverseProperty("ContentItemCommonDataContentLanguage")]
    public virtual ICollection<CmsContentItemCommonDatum> CmsContentItemCommonData { get; set; } = new List<CmsContentItemCommonDatum>();

    [InverseProperty("ContentItemLanguageMetadataContentLanguage")]
    public virtual ICollection<CmsContentItemLanguageMetadatum> CmsContentItemLanguageMetadata { get; set; } = new List<CmsContentItemLanguageMetadatum>();

    [InverseProperty("HeadlessChannelPrimaryContentLanguage")]
    public virtual ICollection<CmsHeadlessChannel> CmsHeadlessChannels { get; set; } = new List<CmsHeadlessChannel>();

    [InverseProperty("WebPageFormerUrlPathContentLanguage")]
    public virtual ICollection<CmsWebPageFormerUrlPath> CmsWebPageFormerUrlPaths { get; set; } = new List<CmsWebPageFormerUrlPath>();

    [InverseProperty("WebPageUrlPathContentLanguage")]
    public virtual ICollection<CmsWebPageUrlPath> CmsWebPageUrlPaths { get; set; } = new List<CmsWebPageUrlPath>();

    [InverseProperty("WebsiteChannelPrimaryContentLanguage")]
    public virtual ICollection<CmsWebsiteChannel> CmsWebsiteChannels { get; set; } = new List<CmsWebsiteChannel>();

    [InverseProperty("ActivityLanguage")]
    public virtual ICollection<OmActivity> OmActivities { get; set; } = new List<OmActivity>();
}
