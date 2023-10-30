using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_ContentItemCommonData")]
    [Index("ContentItemCommonDataContentItemId", "ContentItemCommonDataContentLanguageId", "ContentItemCommonDataIsLatest", Name = "IX_CMS_ContentItemCommonData_ContentItemID_ContentLanguageID_IsLatest", IsUnique = true)]
    [Index("ContentItemCommonDataContentItemId", "ContentItemCommonDataContentLanguageId", "ContentItemCommonDataVersionStatus", Name = "IX_CMS_ContentItemCommonData_ContentItemID_ContentLanguageID_VersionStatus", IsUnique = true)]
    public partial class CmsContentItemCommonDatum
    {
        public CmsContentItemCommonDatum()
        {
            CmsContentItemReferences = new HashSet<CmsContentItemReference>();
        }

        [Key]
        [Column("ContentItemCommonDataID")]
        public int ContentItemCommonDataId { get; set; }
        [Column("ContentItemCommonDataGUID")]
        public Guid ContentItemCommonDataGuid { get; set; }
        [Column("ContentItemCommonDataContentItemID")]
        public int ContentItemCommonDataContentItemId { get; set; }
        [Column("ContentItemCommonDataContentLanguageID")]
        public int ContentItemCommonDataContentLanguageId { get; set; }
        public int ContentItemCommonDataVersionStatus { get; set; }
        public bool ContentItemCommonDataIsLatest { get; set; }
        public string? ContentItemCommonDataPageBuilderWidgets { get; set; }
        public string? ContentItemCommonDataPageTemplateConfiguration { get; set; }

        [ForeignKey("ContentItemCommonDataContentItemId")]
        [InverseProperty("CmsContentItemCommonData")]
        public virtual CmsContentItem ContentItemCommonDataContentItem { get; set; } = null!;
        [ForeignKey("ContentItemCommonDataContentLanguageId")]
        [InverseProperty("CmsContentItemCommonData")]
        public virtual CmsContentLanguage ContentItemCommonDataContentLanguage { get; set; } = null!;
        [InverseProperty("ContentItemReferenceSourceCommonData")]
        public virtual ICollection<CmsContentItemReference> CmsContentItemReferences { get; set; }
    }
}
