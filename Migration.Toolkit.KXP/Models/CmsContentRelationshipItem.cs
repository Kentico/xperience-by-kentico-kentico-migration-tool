using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_ContentRelationshipItem")]
    [Index("ContentRelationshipItemObjectType", "ContentRelationshipItemObjectId", Name = "IX_CMS_ContentRelationshipItem_ObjectType_ObjectID", IsUnique = true)]
    public partial class CmsContentRelationshipItem
    {
        [Column("graph_id_0F42876FB2D4400FB0F5DC129B956195")]
        public long GraphId0f42876fb2d4400fb0f5dc129b956195 { get; set; }
        [Column("$node_id_BB9B40FFA6D34F9F9733E1D3D9259FC6")]
        [StringLength(1000)]
        public string NodeIdBb9b40ffa6d34f9f9733e1d3d9259fc6 { get; set; } = null!;
        [Key]
        [Column("ContentRelationshipItemID")]
        public int ContentRelationshipItemId { get; set; }
        [Column("ContentRelationshipItemGUID")]
        public Guid ContentRelationshipItemGuid { get; set; }
        [StringLength(200)]
        public string ContentRelationshipItemObjectType { get; set; } = null!;
        [Column("ContentRelationshipItemObjectID")]
        public int ContentRelationshipItemObjectId { get; set; }
        [Column("ContentRelationshipItemSiteID")]
        public int? ContentRelationshipItemSiteId { get; set; }

        [ForeignKey("ContentRelationshipItemSiteId")]
        [InverseProperty("CmsContentRelationshipItems")]
        public virtual CmsSite? ContentRelationshipItemSite { get; set; }
    }
}
