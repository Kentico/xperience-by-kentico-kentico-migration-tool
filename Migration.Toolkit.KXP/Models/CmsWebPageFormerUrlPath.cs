using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_WebPageFormerUrlPath")]
    [Index("WebPageFormerUrlPathContentLanguageId", Name = "IX_CMS_WebPageFormerUrlPath_WebPageFormerUrlPathContentLanguageID")]
    [Index("WebPageFormerUrlPathWebPageItemId", Name = "IX_CMS_WebPageFormerUrlPath_WebPageFormerUrlPathWebPageItemID")]
    [Index("WebPageFormerUrlPathWebsiteChannelId", Name = "IX_CMS_WebPageFormerUrlPath_WebPageFormerUrlPathWebsiteChannelID")]
    public partial class CmsWebPageFormerUrlPath
    {
        [Key]
        [Column("WebPageFormerUrlPathID")]
        public int WebPageFormerUrlPathId { get; set; }
        [StringLength(2000)]
        public string WebPageFormerUrlPath { get; set; } = null!;
        [StringLength(64)]
        public string WebPageFormerUrlPathHash { get; set; } = null!;
        [Column("WebPageFormerUrlPathContentLanguageID")]
        public int WebPageFormerUrlPathContentLanguageId { get; set; }
        [Column("WebPageFormerUrlPathWebPageItemID")]
        public int WebPageFormerUrlPathWebPageItemId { get; set; }
        [Column("WebPageFormerUrlPathWebsiteChannelID")]
        public int WebPageFormerUrlPathWebsiteChannelId { get; set; }
        public DateTime WebPageFormerUrlPathLastModified { get; set; }

        [ForeignKey("WebPageFormerUrlPathContentLanguageId")]
        [InverseProperty("CmsWebPageFormerUrlPaths")]
        public virtual CmsContentLanguage WebPageFormerUrlPathContentLanguage { get; set; } = null!;
        [ForeignKey("WebPageFormerUrlPathWebPageItemId")]
        [InverseProperty("CmsWebPageFormerUrlPaths")]
        public virtual CmsWebPageItem WebPageFormerUrlPathWebPageItem { get; set; } = null!;
        [ForeignKey("WebPageFormerUrlPathWebsiteChannelId")]
        [InverseProperty("CmsWebPageFormerUrlPaths")]
        public virtual CmsWebsiteChannel WebPageFormerUrlPathWebsiteChannel { get; set; } = null!;
    }
}
