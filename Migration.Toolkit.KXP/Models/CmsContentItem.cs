using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_ContentItem")]
    [Index("ContentItemContentTypeId", Name = "IX_CMS_ContentItem_ContentItemContentTypeID")]
    [Index("ContentItemName", Name = "IX_CMS_ContentItem_ContentItemName", IsUnique = true)]
    public partial class CmsContentItem
    {
        public CmsContentItem()
        {
            CmsContentItemCommonData = new HashSet<CmsContentItemCommonDatum>();
            CmsContentItemLanguageMetadata = new HashSet<CmsContentItemLanguageMetadatum>();
            CmsContentItemReferences = new HashSet<CmsContentItemReference>();
            CmsWebPageItems = new HashSet<CmsWebPageItem>();
            EmailLibraryEmailConfigurations = new HashSet<EmailLibraryEmailConfiguration>();
        }

        [Key]
        [Column("ContentItemID")]
        public int ContentItemId { get; set; }
        [Column("ContentItemGUID")]
        public Guid ContentItemGuid { get; set; }
        [StringLength(100)]
        public string ContentItemName { get; set; } = null!;
        public bool ContentItemIsReusable { get; set; }
        public bool ContentItemIsSecured { get; set; }
        [Column("ContentItemContentTypeID")]
        public int? ContentItemContentTypeId { get; set; }
        [Column("ContentItemChannelID")]
        public int? ContentItemChannelId { get; set; }

        [ForeignKey("ContentItemChannelId")]
        [InverseProperty("CmsContentItems")]
        public virtual CmsChannel? ContentItemChannel { get; set; }
        [ForeignKey("ContentItemContentTypeId")]
        [InverseProperty("CmsContentItems")]
        public virtual CmsClass? ContentItemContentType { get; set; }
        [InverseProperty("ContentItemCommonDataContentItem")]
        public virtual ICollection<CmsContentItemCommonDatum> CmsContentItemCommonData { get; set; }
        [InverseProperty("ContentItemLanguageMetadataContentItem")]
        public virtual ICollection<CmsContentItemLanguageMetadatum> CmsContentItemLanguageMetadata { get; set; }
        [InverseProperty("ContentItemReferenceTargetItem")]
        public virtual ICollection<CmsContentItemReference> CmsContentItemReferences { get; set; }
        [InverseProperty("WebPageItemContentItem")]
        public virtual ICollection<CmsWebPageItem> CmsWebPageItems { get; set; }
        [InverseProperty("EmailConfigurationContentItem")]
        public virtual ICollection<EmailLibraryEmailConfiguration> EmailLibraryEmailConfigurations { get; set; }
    }
}
