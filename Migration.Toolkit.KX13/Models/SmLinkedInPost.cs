using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("SM_LinkedInPost")]
[Index("LinkedInPostCampaignId", Name = "IX_SM_LinkedInPost_LinkedInPostCampaignID")]
[Index("LinkedInPostLinkedInAccountId", Name = "IX_SM_LinkedInPost_LinkedInPostLinkedInAccountID")]
[Index("LinkedInPostSiteId", Name = "IX_SM_LinkedInPost_LinkedInPostSiteID")]
public partial class SmLinkedInPost
{
    [Key]
    [Column("LinkedInPostID")]
    public int LinkedInPostId { get; set; }

    [Column("LinkedInPostLinkedInAccountID")]
    public int LinkedInPostLinkedInAccountId { get; set; }

    [StringLength(700)]
    public string LinkedInPostComment { get; set; } = null!;

    [Column("LinkedInPostSiteID")]
    public int LinkedInPostSiteId { get; set; }

    [Column("LinkedInPostGUID")]
    public Guid LinkedInPostGuid { get; set; }

    public DateTime? LinkedInPostLastModified { get; set; }

    [StringLength(200)]
    public string? LinkedInPostUpdateKey { get; set; }

    [Column("LinkedInPostURLShortenerType")]
    public int? LinkedInPostUrlshortenerType { get; set; }

    public DateTime? LinkedInPostScheduledPublishDateTime { get; set; }

    [Column("LinkedInPostCampaignID")]
    public int? LinkedInPostCampaignId { get; set; }

    public DateTime? LinkedInPostPublishedDateTime { get; set; }

    [Column("LinkedInPostHTTPStatusCode")]
    public int? LinkedInPostHttpstatusCode { get; set; }

    public int? LinkedInPostErrorCode { get; set; }

    public string? LinkedInPostErrorMessage { get; set; }

    [Column("LinkedInPostDocumentGUID")]
    public Guid? LinkedInPostDocumentGuid { get; set; }

    public bool? LinkedInPostIsCreatedByUser { get; set; }

    public bool? LinkedInPostPostAfterDocumentPublish { get; set; }

    public DateTime? LinkedInPostInsightsLastUpdated { get; set; }

    public int? LinkedInPostCommentCount { get; set; }

    public int? LinkedInPostImpressionCount { get; set; }

    public int? LinkedInPostLikeCount { get; set; }

    public int? LinkedInPostShareCount { get; set; }

    public int? LinkedInPostClickCount { get; set; }

    public double? LinkedInPostEngagement { get; set; }

    [ForeignKey("LinkedInPostCampaignId")]
    [InverseProperty("SmLinkedInPosts")]
    public virtual AnalyticsCampaign? LinkedInPostCampaign { get; set; }

    [ForeignKey("LinkedInPostLinkedInAccountId")]
    [InverseProperty("SmLinkedInPosts")]
    public virtual SmLinkedInAccount LinkedInPostLinkedInAccount { get; set; } = null!;

    [ForeignKey("LinkedInPostSiteId")]
    [InverseProperty("SmLinkedInPosts")]
    public virtual CmsSite LinkedInPostSite { get; set; } = null!;
}
