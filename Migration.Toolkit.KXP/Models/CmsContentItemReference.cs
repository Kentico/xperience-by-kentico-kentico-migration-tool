using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_ContentItemReference")]
    [Index("ContentItemReferenceSourceCommonDataId", Name = "IX_CMS_ContentItemReference_ContentItemReferenceSourceCommonDataID")]
    [Index("ContentItemReferenceSourceCommonDataId", "ContentItemReferenceTargetItemId", "ContentItemReferenceGroupGuid", Name = "IX_CMS_ContentItemReference_ContentItemReferenceSourceCommonDataID_TargetItemID_GroupGUID", IsUnique = true)]
    [Index("ContentItemReferenceTargetItemId", Name = "IX_CMS_ContentItemReference_ContentItemReferenceTargetItemID")]
    public partial class CmsContentItemReference
    {
        [Key]
        [Column("ContentItemReferenceID")]
        public int ContentItemReferenceId { get; set; }
        [Column("ContentItemReferenceGUID")]
        public Guid ContentItemReferenceGuid { get; set; }
        [Column("ContentItemReferenceSourceCommonDataID")]
        public int ContentItemReferenceSourceCommonDataId { get; set; }
        [Column("ContentItemReferenceTargetItemID")]
        public int ContentItemReferenceTargetItemId { get; set; }
        [Column("ContentItemReferenceGroupGUID")]
        public Guid ContentItemReferenceGroupGuid { get; set; }

        [ForeignKey("ContentItemReferenceSourceCommonDataId")]
        [InverseProperty("CmsContentItemReferences")]
        public virtual CmsContentItemCommonDatum ContentItemReferenceSourceCommonData { get; set; } = null!;
        [ForeignKey("ContentItemReferenceTargetItemId")]
        [InverseProperty("CmsContentItemReferences")]
        public virtual CmsContentItem ContentItemReferenceTargetItem { get; set; } = null!;
    }
}
