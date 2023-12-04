using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_Tree")]
[Index("NodeAclid", Name = "IX_CMS_Tree_NodeACLID")]
[Index("NodeAliasPath", Name = "IX_CMS_Tree_NodeAliasPath")]
[Index("NodeClassId", Name = "IX_CMS_Tree_NodeClassID")]
[Index("NodeLevel", Name = "IX_CMS_Tree_NodeLevel")]
[Index("NodeLinkedNodeId", Name = "IX_CMS_Tree_NodeLinkedNodeID")]
[Index("NodeLinkedNodeSiteId", Name = "IX_CMS_Tree_NodeLinkedNodeSiteID")]
[Index("NodeOriginalNodeId", Name = "IX_CMS_Tree_NodeOriginalNodeID")]
[Index("NodeOwner", Name = "IX_CMS_Tree_NodeOwner")]
[Index("NodeParentId", "NodeAlias", "NodeName", Name = "IX_CMS_Tree_NodeParentID_NodeAlias_NodeName")]
[Index("NodeSkuid", Name = "IX_CMS_Tree_NodeSKUID")]
[Index("NodeSiteId", "NodeGuid", Name = "IX_CMS_Tree_NodeSiteID_NodeGUID", IsUnique = true)]
public partial class CmsTree
{
    [Key]
    [Column("NodeID")]
    public int NodeId { get; set; }

    public string NodeAliasPath { get; set; } = null!;

    [StringLength(100)]
    public string NodeName { get; set; } = null!;

    [StringLength(50)]
    public string NodeAlias { get; set; } = null!;

    [Column("NodeClassID")]
    public int NodeClassId { get; set; }

    [Column("NodeParentID")]
    public int? NodeParentId { get; set; }

    public int NodeLevel { get; set; }

    [Column("NodeACLID")]
    public int? NodeAclid { get; set; }

    [Column("NodeSiteID")]
    public int NodeSiteId { get; set; }

    [Column("NodeGUID")]
    public Guid NodeGuid { get; set; }

    public int? NodeOrder { get; set; }

    public bool? IsSecuredNode { get; set; }

    [Column("NodeSKUID")]
    public int? NodeSkuid { get; set; }

    [Column("NodeLinkedNodeID")]
    public int? NodeLinkedNodeId { get; set; }

    public int? NodeOwner { get; set; }

    public string? NodeCustomData { get; set; }

    [Column("NodeLinkedNodeSiteID")]
    public int? NodeLinkedNodeSiteId { get; set; }

    public bool? NodeHasChildren { get; set; }

    public bool? NodeHasLinks { get; set; }

    [Column("NodeOriginalNodeID")]
    public int? NodeOriginalNodeId { get; set; }

    [Column("NodeIsACLOwner")]
    public bool NodeIsAclowner { get; set; }

    [InverseProperty("DocumentNode")]
    public virtual ICollection<CmsDocument> CmsDocuments { get; set; } = new List<CmsDocument>();

    [InverseProperty("PageFormerUrlPathNode")]
    public virtual ICollection<CmsPageFormerUrlPath> CmsPageFormerUrlPaths { get; set; } = new List<CmsPageFormerUrlPath>();

    [InverseProperty("PageUrlPathNode")]
    public virtual ICollection<CmsPageUrlPath> CmsPageUrlPaths { get; set; } = new List<CmsPageUrlPath>();

    [InverseProperty("LeftNode")]
    public virtual ICollection<CmsRelationship> CmsRelationshipLeftNodes { get; set; } = new List<CmsRelationship>();

    [InverseProperty("RightNode")]
    public virtual ICollection<CmsRelationship> CmsRelationshipRightNodes { get; set; } = new List<CmsRelationship>();

    [InverseProperty("Node")]
    public virtual ICollection<ComMultiBuyDiscountTree> ComMultiBuyDiscountTrees { get; set; } = new List<ComMultiBuyDiscountTree>();

    [InverseProperty("NodeLinkedNode")]
    public virtual ICollection<CmsTree> InverseNodeLinkedNode { get; set; } = new List<CmsTree>();

    [InverseProperty("NodeOriginalNode")]
    public virtual ICollection<CmsTree> InverseNodeOriginalNode { get; set; } = new List<CmsTree>();

    [InverseProperty("NodeParent")]
    public virtual ICollection<CmsTree> InverseNodeParent { get; set; } = new List<CmsTree>();

    [ForeignKey("NodeAclid")]
    [InverseProperty("CmsTrees")]
    public virtual CmsAcl? NodeAcl { get; set; }

    [ForeignKey("NodeClassId")]
    [InverseProperty("CmsTrees")]
    public virtual CmsClass NodeClass { get; set; } = null!;

    [ForeignKey("NodeLinkedNodeId")]
    [InverseProperty("InverseNodeLinkedNode")]
    public virtual CmsTree? NodeLinkedNode { get; set; }

    [ForeignKey("NodeLinkedNodeSiteId")]
    [InverseProperty("CmsTreeNodeLinkedNodeSites")]
    public virtual CmsSite? NodeLinkedNodeSite { get; set; }

    [ForeignKey("NodeOriginalNodeId")]
    [InverseProperty("InverseNodeOriginalNode")]
    public virtual CmsTree? NodeOriginalNode { get; set; }

    [ForeignKey("NodeOwner")]
    [InverseProperty("CmsTrees")]
    public virtual CmsUser? NodeOwnerNavigation { get; set; }

    [ForeignKey("NodeParentId")]
    [InverseProperty("InverseNodeParent")]
    public virtual CmsTree? NodeParent { get; set; }

    [ForeignKey("NodeSiteId")]
    [InverseProperty("CmsTreeNodeSites")]
    public virtual CmsSite NodeSite { get; set; } = null!;

    [ForeignKey("NodeSkuid")]
    [InverseProperty("CmsTrees")]
    public virtual ComSku? NodeSku { get; set; }
}
