using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_Relationship")]
[Index("LeftNodeId", Name = "IX_CMS_Relationship_LeftNodeID")]
[Index("RelationshipNameId", Name = "IX_CMS_Relationship_RelationshipNameID")]
[Index("RightNodeId", Name = "IX_CMS_Relationship_RightNodeID")]
public partial class CmsRelationship
{
    [Key]
    [Column("RelationshipID")]
    public int RelationshipId { get; set; }

    [Column("LeftNodeID")]
    public int LeftNodeId { get; set; }

    [Column("RightNodeID")]
    public int RightNodeId { get; set; }

    [Column("RelationshipNameID")]
    public int RelationshipNameId { get; set; }

    public string? RelationshipCustomData { get; set; }

    public int? RelationshipOrder { get; set; }

    public bool? RelationshipIsAdHoc { get; set; }

    [ForeignKey("LeftNodeId")]
    [InverseProperty("CmsRelationshipLeftNodes")]
    public virtual CmsTree LeftNode { get; set; } = null!;

    [ForeignKey("RelationshipNameId")]
    [InverseProperty("CmsRelationships")]
    public virtual CmsRelationshipName RelationshipName { get; set; } = null!;

    [ForeignKey("RightNodeId")]
    [InverseProperty("CmsRelationshipRightNodes")]
    public virtual CmsTree RightNode { get; set; } = null!;
}
