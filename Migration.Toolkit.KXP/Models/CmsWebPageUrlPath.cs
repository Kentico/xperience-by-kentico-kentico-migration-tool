using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_WebPageUrlPath")]
    [Index("WebPageUrlPathContentLanguageId", Name = "IX_CMS_WebPageUrlPath_WebPageUrlPathContentLanguageID")]
    [Index("WebPageUrlPathHash", "WebPageUrlPathWebsiteChannelId", "WebPageUrlPathContentLanguageId", "WebPageUrlPathIsDraft", Name = "IX_CMS_WebPageUrlPath_WebPageUrlPathHash_WebPageUrlPathWebsiteChannelID_WebPageUrlPathContentLanguageID_WebPageUrlPathIsDraft", IsUnique = true)]
    [Index("WebPageUrlPathWebPageItemId", Name = "IX_CMS_WebPageUrlPath_WebPageUrlPathWebPageItemID")]
    [Index("WebPageUrlPathWebsiteChannelId", Name = "IX_CMS_WebPageUrlPath_WebPageUrlPathWebsiteChannelID")]
    public partial class CmsWebPageUrlPath
    {
        [Key]
        [Column("WebPageUrlPathID")]
        public int WebPageUrlPathId { get; set; }
        [StringLength(2000)]
        public string WebPageUrlPath { get; set; } = null!;
        [StringLength(64)]
        public string WebPageUrlPathHash { get; set; } = null!;
        [Column("WebPageUrlPathWebPageItemID")]
        public int WebPageUrlPathWebPageItemId { get; set; }
        [Required]
        public bool? WebPageUrlPathIsLatest { get; set; }
        [Column("WebPageUrlPathGUID")]
        public Guid WebPageUrlPathGuid { get; set; }
        [Column("WebPageUrlPathWebsiteChannelID")]
        public int WebPageUrlPathWebsiteChannelId { get; set; }
        [Column("WebPageUrlPathContentLanguageID")]
        public int WebPageUrlPathContentLanguageId { get; set; }
        public bool WebPageUrlPathIsDraft { get; set; }

        [ForeignKey("WebPageUrlPathContentLanguageId")]
        [InverseProperty("CmsWebPageUrlPaths")]
        public virtual CmsContentLanguage WebPageUrlPathContentLanguage { get; set; } = null!;
        [ForeignKey("WebPageUrlPathWebPageItemId")]
        [InverseProperty("CmsWebPageUrlPaths")]
        public virtual CmsWebPageItem WebPageUrlPathWebPageItem { get; set; } = null!;
        [ForeignKey("WebPageUrlPathWebsiteChannelId")]
        [InverseProperty("CmsWebPageUrlPaths")]
        public virtual CmsWebsiteChannel WebPageUrlPathWebsiteChannel { get; set; } = null!;
    }
}
