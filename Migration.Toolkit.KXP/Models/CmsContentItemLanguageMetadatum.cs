using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_ContentItemLanguageMetadata")]
    [Index("ContentItemLanguageMetadataContentItemId", Name = "IX_CMS_ContentItemLanguageMetadata_ContentItemLanguageMetadataContentItemID")]
    [Index("ContentItemLanguageMetadataContentLanguageId", Name = "IX_CMS_ContentItemLanguageMetadata_ContentItemLanguageMetadataContentLanguageID")]
    [Index("ContentItemLanguageMetadataCreatedByUserId", Name = "IX_CMS_ContentItemLanguageMetadata_ContentItemLanguageMetadataCreatedByUserID")]
    [Index("ContentItemLanguageMetadataModifiedByUserId", Name = "IX_CMS_ContentItemLanguageMetadata_ContentItemLanguageMetadataModifiedByUserID")]
    public partial class CmsContentItemLanguageMetadatum
    {
        [Key]
        [Column("ContentItemLanguageMetadataID")]
        public int ContentItemLanguageMetadataId { get; set; }
        [Column("ContentItemLanguageMetadataContentItemID")]
        public int ContentItemLanguageMetadataContentItemId { get; set; }
        [StringLength(100)]
        public string ContentItemLanguageMetadataDisplayName { get; set; } = null!;
        public int ContentItemLanguageMetadataLatestVersionStatus { get; set; }
        [Column("ContentItemLanguageMetadataGUID")]
        public Guid ContentItemLanguageMetadataGuid { get; set; }
        public DateTime ContentItemLanguageMetadataCreatedWhen { get; set; }
        [Column("ContentItemLanguageMetadataCreatedByUserID")]
        public int? ContentItemLanguageMetadataCreatedByUserId { get; set; }
        public DateTime ContentItemLanguageMetadataModifiedWhen { get; set; }
        [Column("ContentItemLanguageMetadataModifiedByUserID")]
        public int? ContentItemLanguageMetadataModifiedByUserId { get; set; }
        public bool ContentItemLanguageMetadataHasImageAsset { get; set; }
        [Column("ContentItemLanguageMetadataContentLanguageID")]
        public int ContentItemLanguageMetadataContentLanguageId { get; set; }

        [ForeignKey("ContentItemLanguageMetadataContentItemId")]
        [InverseProperty("CmsContentItemLanguageMetadata")]
        public virtual CmsContentItem ContentItemLanguageMetadataContentItem { get; set; } = null!;
        [ForeignKey("ContentItemLanguageMetadataContentLanguageId")]
        [InverseProperty("CmsContentItemLanguageMetadata")]
        public virtual CmsContentLanguage ContentItemLanguageMetadataContentLanguage { get; set; } = null!;
        [ForeignKey("ContentItemLanguageMetadataCreatedByUserId")]
        [InverseProperty("CmsContentItemLanguageMetadatumContentItemLanguageMetadataCreatedByUsers")]
        public virtual CmsUser? ContentItemLanguageMetadataCreatedByUser { get; set; }
        [ForeignKey("ContentItemLanguageMetadataModifiedByUserId")]
        [InverseProperty("CmsContentItemLanguageMetadatumContentItemLanguageMetadataModifiedByUsers")]
        public virtual CmsUser? ContentItemLanguageMetadataModifiedByUser { get; set; }
    }
}
