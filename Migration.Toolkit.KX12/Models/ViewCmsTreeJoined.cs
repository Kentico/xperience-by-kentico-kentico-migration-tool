using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Keyless]
public partial class ViewCmsTreeJoined
{
    [StringLength(100)]
    public string ClassName { get; set; } = null!;

    [StringLength(100)]
    public string ClassDisplayName { get; set; } = null!;

    [Column("NodeID")]
    public int NodeId { get; set; }

    [StringLength(450)]
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

    public int? NodeCacheMinutes { get; set; }

    [Column("NodeSKUID")]
    public int? NodeSkuid { get; set; }

    public string? NodeDocType { get; set; }

    public string? NodeHeadTags { get; set; }

    public string? NodeBodyElementAttributes { get; set; }

    [StringLength(200)]
    public string? NodeInheritPageLevels { get; set; }

    [Column("RequiresSSL")]
    public int? RequiresSsl { get; set; }

    [Column("NodeLinkedNodeID")]
    public int? NodeLinkedNodeId { get; set; }

    public int? NodeOwner { get; set; }

    public string? NodeCustomData { get; set; }

    [Column("NodeGroupID")]
    public int? NodeGroupId { get; set; }

    [Column("NodeLinkedNodeSiteID")]
    public int? NodeLinkedNodeSiteId { get; set; }

    [Column("NodeTemplateID")]
    public int? NodeTemplateId { get; set; }

    public bool? NodeTemplateForAllCultures { get; set; }

    public bool? NodeInheritPageTemplate { get; set; }

    public bool? NodeAllowCacheInFileSystem { get; set; }

    public bool? NodeHasChildren { get; set; }

    public bool? NodeHasLinks { get; set; }

    [Column("NodeOriginalNodeID")]
    public int? NodeOriginalNodeId { get; set; }

    public bool NodeIsContentOnly { get; set; }

    [Column("NodeIsACLOwner")]
    public bool NodeIsAclowner { get; set; }

    public string? NodeBodyScripts { get; set; }

    [Column("DocumentID")]
    public int DocumentId { get; set; }

    [StringLength(100)]
    public string DocumentName { get; set; } = null!;

    [StringLength(1500)]
    public string? DocumentNamePath { get; set; }

    public DateTime? DocumentModifiedWhen { get; set; }

    [Column("DocumentModifiedByUserID")]
    public int? DocumentModifiedByUserId { get; set; }

    public int? DocumentForeignKeyValue { get; set; }

    [Column("DocumentCreatedByUserID")]
    public int? DocumentCreatedByUserId { get; set; }

    public DateTime? DocumentCreatedWhen { get; set; }

    [Column("DocumentCheckedOutByUserID")]
    public int? DocumentCheckedOutByUserId { get; set; }

    public DateTime? DocumentCheckedOutWhen { get; set; }

    [Column("DocumentCheckedOutVersionHistoryID")]
    public int? DocumentCheckedOutVersionHistoryId { get; set; }

    [Column("DocumentPublishedVersionHistoryID")]
    public int? DocumentPublishedVersionHistoryId { get; set; }

    [Column("DocumentWorkflowStepID")]
    public int? DocumentWorkflowStepId { get; set; }

    public DateTime? DocumentPublishFrom { get; set; }

    public DateTime? DocumentPublishTo { get; set; }

    [StringLength(450)]
    public string? DocumentUrlPath { get; set; }

    [StringLength(10)]
    public string DocumentCulture { get; set; } = null!;

    [Column("DocumentNodeID")]
    public int DocumentNodeId { get; set; }

    public string? DocumentPageTitle { get; set; }

    public string? DocumentPageKeyWords { get; set; }

    public string? DocumentPageDescription { get; set; }

    public bool DocumentShowInSiteMap { get; set; }

    public bool DocumentMenuItemHideInNavigation { get; set; }

    [StringLength(200)]
    public string? DocumentMenuCaption { get; set; }

    [StringLength(100)]
    public string? DocumentMenuStyle { get; set; }

    [StringLength(200)]
    public string? DocumentMenuItemImage { get; set; }

    [StringLength(200)]
    public string? DocumentMenuItemLeftImage { get; set; }

    [StringLength(200)]
    public string? DocumentMenuItemRightImage { get; set; }

    [Column("DocumentPageTemplateID")]
    public int? DocumentPageTemplateId { get; set; }

    [StringLength(450)]
    public string? DocumentMenuJavascript { get; set; }

    [StringLength(450)]
    public string? DocumentMenuRedirectUrl { get; set; }

    public bool? DocumentUseNamePathForUrlPath { get; set; }

    [Column("DocumentStylesheetID")]
    public int? DocumentStylesheetId { get; set; }

    public string? DocumentContent { get; set; }

    [StringLength(100)]
    public string? DocumentMenuClass { get; set; }

    [StringLength(200)]
    public string? DocumentMenuStyleHighlighted { get; set; }

    [StringLength(100)]
    public string? DocumentMenuClassHighlighted { get; set; }

    [StringLength(200)]
    public string? DocumentMenuItemImageHighlighted { get; set; }

    [StringLength(200)]
    public string? DocumentMenuItemLeftImageHighlighted { get; set; }

    [StringLength(200)]
    public string? DocumentMenuItemRightImageHighlighted { get; set; }

    public bool? DocumentMenuItemInactive { get; set; }

    public string? DocumentCustomData { get; set; }

    [StringLength(100)]
    public string? DocumentExtensions { get; set; }

    public string? DocumentTags { get; set; }

    [Column("DocumentTagGroupID")]
    public int? DocumentTagGroupId { get; set; }

    [StringLength(440)]
    public string? DocumentWildcardRule { get; set; }

    public string? DocumentWebParts { get; set; }

    public double? DocumentRatingValue { get; set; }

    public int? DocumentRatings { get; set; }

    public int? DocumentPriority { get; set; }

    [StringLength(50)]
    public string? DocumentType { get; set; }

    public DateTime? DocumentLastPublished { get; set; }

    public bool? DocumentUseCustomExtensions { get; set; }

    public string? DocumentGroupWebParts { get; set; }

    public bool? DocumentCheckedOutAutomatically { get; set; }

    [StringLength(200)]
    public string? DocumentTrackConversionName { get; set; }

    [StringLength(100)]
    public string? DocumentConversionValue { get; set; }

    public bool? DocumentSearchExcluded { get; set; }

    [StringLength(50)]
    public string? DocumentLastVersionNumber { get; set; }

    public bool? DocumentIsArchived { get; set; }

    [StringLength(32)]
    public string? DocumentHash { get; set; }

    public bool? DocumentLogVisitActivity { get; set; }

    [Column("DocumentGUID")]
    public Guid? DocumentGuid { get; set; }

    [Column("DocumentWorkflowCycleGUID")]
    public Guid? DocumentWorkflowCycleGuid { get; set; }

    [StringLength(100)]
    public string? DocumentSitemapSettings { get; set; }

    public bool? DocumentIsWaitingForTranslation { get; set; }

    [Column("DocumentSKUName")]
    [StringLength(440)]
    public string? DocumentSkuname { get; set; }

    [Column("DocumentSKUDescription")]
    public string? DocumentSkudescription { get; set; }

    [Column("DocumentSKUShortDescription")]
    public string? DocumentSkushortDescription { get; set; }

    [StringLength(450)]
    public string? DocumentWorkflowActionStatus { get; set; }

    public bool? DocumentMenuRedirectToFirstChild { get; set; }

    public bool DocumentCanBePublished { get; set; }

    public bool DocumentInheritsStylesheet { get; set; }

    public string? DocumentPageBuilderWidgets { get; set; }

    public string? DocumentPageTemplateConfiguration { get; set; }

    [Column("DocumentABTestConfiguration")]
    public string? DocumentAbtestConfiguration { get; set; }
}