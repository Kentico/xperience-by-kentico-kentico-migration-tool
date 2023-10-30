using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_WebPageItem")]
    [Index("WebPageItemContentItemId", Name = "IX_CMS_WebPageItem_WebPageItemContentItemID")]
    [Index("WebPageItemGuid", Name = "IX_CMS_WebPageItem_WebPageItemGUID_Unique", IsUnique = true)]
    [Index("WebPageItemName", Name = "IX_CMS_WebPageItem_WebPageItemName_Unique", IsUnique = true)]
    [Index("WebPageItemParentId", Name = "IX_CMS_WebPageItem_WebPageItemParentID")]
    [Index("WebPageItemTreePath", "WebPageItemWebsiteChannelId", Name = "IX_CMS_WebPageItem_WebPageItemTreePath_WebPageItemWebsiteChannelID_Unique", IsUnique = true)]
    [Index("WebPageItemWebsiteChannelId", Name = "IX_CMS_WebPageItem_WebPageItemWebsiteChannelID")]
    public partial class CmsWebPageItem
    {
        public CmsWebPageItem()
        {
            CmsWebPageFormerUrlPaths = new HashSet<CmsWebPageFormerUrlPath>();
            CmsWebPageUrlPaths = new HashSet<CmsWebPageUrlPath>();
            InverseWebPageItemParent = new HashSet<CmsWebPageItem>();
        }

        [Key]
        [Column("WebPageItemID")]
        public int WebPageItemId { get; set; }
        [Column("WebPageItemParentID")]
        public int? WebPageItemParentId { get; set; }
        [Column("WebPageItemGUID")]
        public Guid WebPageItemGuid { get; set; }
        [StringLength(100)]
        public string WebPageItemName { get; set; } = null!;
        [StringLength(850)]
        public string WebPageItemTreePath { get; set; } = null!;
        [Column("WebPageItemWebsiteChannelID")]
        public int WebPageItemWebsiteChannelId { get; set; }
        [Column("WebPageItemContentItemID")]
        public int? WebPageItemContentItemId { get; set; }
        public int? WebPageItemOrder { get; set; }

        [ForeignKey("WebPageItemContentItemId")]
        [InverseProperty("CmsWebPageItems")]
        public virtual CmsContentItem? WebPageItemContentItem { get; set; }
        [ForeignKey("WebPageItemParentId")]
        [InverseProperty("InverseWebPageItemParent")]
        public virtual CmsWebPageItem? WebPageItemParent { get; set; }
        [ForeignKey("WebPageItemWebsiteChannelId")]
        [InverseProperty("CmsWebPageItems")]
        public virtual CmsWebsiteChannel WebPageItemWebsiteChannel { get; set; } = null!;
        [InverseProperty("WebPageFormerUrlPathWebPageItem")]
        public virtual ICollection<CmsWebPageFormerUrlPath> CmsWebPageFormerUrlPaths { get; set; }
        [InverseProperty("WebPageUrlPathWebPageItem")]
        public virtual ICollection<CmsWebPageUrlPath> CmsWebPageUrlPaths { get; set; }
        [InverseProperty("WebPageItemParent")]
        public virtual ICollection<CmsWebPageItem> InverseWebPageItemParent { get; set; }
    }
}
