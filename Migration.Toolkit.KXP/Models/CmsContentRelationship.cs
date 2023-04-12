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
        [Column("graph_id_7F94B75F70E84E8FB9B227B6C692806A")]
        public long GraphId7f94b75f70e84e8fb9b227b6c692806a { get; set; }
        [Column("$edge_id_6B1607C77385450BB8F50DCEA5189C6F")]
        [StringLength(1000)]
        public string EdgeId6b1607c77385450bb8f50dcea5189c6f { get; set; } = null!;
        [Column("from_obj_id_476D08013231480588EE995977E21DF5")]
        public int FromObjId476d08013231480588ee995977e21df5 { get; set; }
        [Column("from_id_4CB26B80E523479DB829CF207AF83D88")]
        public long FromId4cb26b80e523479db829cf207af83d88 { get; set; }
        [Column("$from_id_5A00FF7305FC4CA3A10D84BC73A532BA")]
        [StringLength(1000)]
        public string? FromId5a00ff7305fc4ca3a10d84bc73a532ba { get; set; }
        [Column("to_obj_id_2C87E30CDE554517903E799E869F71D6")]
        public int ToObjId2c87e30cde554517903e799e869f71d6 { get; set; }
        [Column("to_id_3E6168226DCE4BEFA2E6F70D09CE793F")]
        public long ToId3e6168226dce4befa2e6f70d09ce793f { get; set; }
        [Column("$to_id_961490D158BF4023B5DEA1BE5BE417C6")]
        [StringLength(1000)]
        public string? ToId961490d158bf4023b5dea1be5be417c6 { get; set; }
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
