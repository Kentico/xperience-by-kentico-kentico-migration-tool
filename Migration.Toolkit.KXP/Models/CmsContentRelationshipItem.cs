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
        [Column("graph_id_ADFBD307F6894A6B9C2C0A3D9A6058DC")]
        public long GraphIdAdfbd307f6894a6b9c2c0a3d9a6058dc { get; set; }
        [Column("$node_id_B12FA7C97AAB49A3BC4B145536D270E3")]
        [StringLength(1000)]
        public string NodeIdB12fa7c97aab49a3bc4b145536d270e3 { get; set; } = null!;
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
