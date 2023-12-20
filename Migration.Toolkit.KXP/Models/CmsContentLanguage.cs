using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_ContentLanguage")]
    public partial class CmsContentLanguage
    {
        public CmsContentLanguage()
        {
            CmsContentItemCommonData = new HashSet<CmsContentItemCommonDatum>();
            CmsContentItemLanguageMetadata = new HashSet<CmsContentItemLanguageMetadatum>();
            CmsWebPageFormerUrlPaths = new HashSet<CmsWebPageFormerUrlPath>();
            CmsWebPageUrlPaths = new HashSet<CmsWebPageUrlPath>();
            CmsWebsiteChannels = new HashSet<CmsWebsiteChannel>();
            OmActivities = new HashSet<OmActivity>();
        }

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
        public virtual ICollection<CmsContentItemCommonDatum> CmsContentItemCommonData { get; set; }
        [InverseProperty("ContentItemLanguageMetadataContentLanguage")]
        public virtual ICollection<CmsContentItemLanguageMetadatum> CmsContentItemLanguageMetadata { get; set; }
        [InverseProperty("WebPageFormerUrlPathContentLanguage")]
        public virtual ICollection<CmsWebPageFormerUrlPath> CmsWebPageFormerUrlPaths { get; set; }
        [InverseProperty("WebPageUrlPathContentLanguage")]
        public virtual ICollection<CmsWebPageUrlPath> CmsWebPageUrlPaths { get; set; }
        [InverseProperty("WebsiteChannelPrimaryContentLanguage")]
        public virtual ICollection<CmsWebsiteChannel> CmsWebsiteChannels { get; set; }
        [InverseProperty("ActivityLanguage")]
        public virtual ICollection<OmActivity> OmActivities { get; set; }
    }
}
