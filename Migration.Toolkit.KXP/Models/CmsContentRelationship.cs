using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_ContentRelationship")]
    public partial class CmsContentRelationship
    {
        [Column("graph_id_741F379D95C34D2CB17F74FAD4E11C91")]
        public long GraphId741f379d95c34d2cb17f74fad4e11c91 { get; set; }
        [Column("$edge_id_845C2B755CDD4FEC9A746480DCADD809")]
        [StringLength(1000)]
        public string EdgeId845c2b755cdd4fec9a746480dcadd809 { get; set; } = null!;
        [Column("from_obj_id_3EA6B379F9EC4B5199DD1285908AC97F")]
        public int FromObjId3ea6b379f9ec4b5199dd1285908ac97f { get; set; }
        [Column("from_id_C68E204CDD8A4F0989D187E933226765")]
        public long FromIdC68e204cdd8a4f0989d187e933226765 { get; set; }
        [Column("$from_id_A7B8B75CECF84F869552F10D5EF8843B")]
        [StringLength(1000)]
        public string? FromIdA7b8b75cecf84f869552f10d5ef8843b { get; set; }
        [Column("to_obj_id_BF32BEBD1107428DA8A9CB51500FB66F")]
        public int ToObjIdBf32bebd1107428da8a9cb51500fb66f { get; set; }
        [Column("to_id_A0E3165266D0441680EFF403F1E7F54E")]
        public long ToIdA0e3165266d0441680eff403f1e7f54e { get; set; }
        [Column("$to_id_30C5D1E23418447DB3E03D10DE678C5E")]
        [StringLength(1000)]
        public string? ToId30c5d1e23418447db3e03d10de678c5e { get; set; }
        [Key]
        [Column("ContentRelationshipID")]
        public int ContentRelationshipId { get; set; }
        [Column("ContentRelationshipGUID")]
        public Guid ContentRelationshipGuid { get; set; }
        [Column("ContentRelationshipGroupGUID")]
        public Guid ContentRelationshipGroupGuid { get; set; }
        [StringLength(200)]
        public string ContentRelationshipSourceObjectType { get; set; } = null!;
        [Column("ContentRelationshipSourceObjectID")]
        public int ContentRelationshipSourceObjectId { get; set; }
        [StringLength(200)]
        public string ContentRelationshipTargetObjectType { get; set; } = null!;
        [Column("ContentRelationshipTargetObjectID")]
        public int ContentRelationshipTargetObjectId { get; set; }
        [Column("ContentRelationshipSiteID")]
        public int? ContentRelationshipSiteId { get; set; }
        [StringLength(200)]
        public string? ContentRelationshipGroupType { get; set; }

        [ForeignKey("ContentRelationshipSiteId")]
        [InverseProperty("CmsContentRelationships")]
        public virtual CmsSite? ContentRelationshipSite { get; set; }
    }
}
