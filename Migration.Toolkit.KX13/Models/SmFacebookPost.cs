using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("SM_FacebookPost")]
[Index("FacebookPostCampaignId", Name = "IX_SM_FacebookPost_FacebookPostCampaignID")]
[Index("FacebookPostFacebookAccountId", Name = "IX_SM_FacebookPost_FacebookPostFacebookAccountID")]
[Index("FacebookPostSiteId", Name = "IX_SM_FacebookPost_FacebookPostSiteID")]
public partial class SmFacebookPost
{
    [Key]
    [Column("FacebookPostID")]
    public int FacebookPostId { get; set; }

    [Column("FacebookPostGUID")]
    public Guid FacebookPostGuid { get; set; }

    public DateTime FacebookPostLastModified { get; set; }

    [Column("FacebookPostSiteID")]
    public int FacebookPostSiteId { get; set; }

    [Column("FacebookPostFacebookAccountID")]
    public int FacebookPostFacebookAccountId { get; set; }

    public string FacebookPostText { get; set; } = null!;

    [Column("FacebookPostURLShortenerType")]
    public int? FacebookPostUrlshortenerType { get; set; }

    public int? FacebookPostErrorCode { get; set; }

    public int? FacebookPostErrorSubcode { get; set; }

    [Column("FacebookPostExternalID")]
    public string? FacebookPostExternalId { get; set; }

    public DateTime? FacebookPostPublishedDateTime { get; set; }

    public DateTime? FacebookPostScheduledPublishDateTime { get; set; }

    [Column("FacebookPostCampaignID")]
    public int? FacebookPostCampaignId { get; set; }

    public bool? FacebookPostPostAfterDocumentPublish { get; set; }

    public int? FacebookPostInsightPeopleReached { get; set; }

    public int? FacebookPostInsightLikesFromPage { get; set; }

    public int? FacebookPostInsightCommentsFromPage { get; set; }

    public int? FacebookPostInsightSharesFromPage { get; set; }

    public int? FacebookPostInsightLikesTotal { get; set; }

    public int? FacebookPostInsightCommentsTotal { get; set; }

    public int? FacebookPostInsightNegativeHidePost { get; set; }

    public int? FacebookPostInsightNegativeHideAllPosts { get; set; }

    public int? FacebookPostInsightNegativeReportSpam { get; set; }

    public int? FacebookPostInsightNegativeUnlikePage { get; set; }

    public DateTime? FacebookPostInsightsLastUpdated { get; set; }

    [Column("FacebookPostDocumentGUID")]
    public Guid? FacebookPostDocumentGuid { get; set; }

    public bool? FacebookPostIsCreatedByUser { get; set; }

    [ForeignKey("FacebookPostCampaignId")]
    [InverseProperty("SmFacebookPosts")]
    public virtual AnalyticsCampaign? FacebookPostCampaign { get; set; }

    [ForeignKey("FacebookPostFacebookAccountId")]
    [InverseProperty("SmFacebookPosts")]
    public virtual SmFacebookAccount FacebookPostFacebookAccount { get; set; } = null!;

    [ForeignKey("FacebookPostSiteId")]
    [InverseProperty("SmFacebookPosts")]
    public virtual CmsSite FacebookPostSite { get; set; } = null!;
}
