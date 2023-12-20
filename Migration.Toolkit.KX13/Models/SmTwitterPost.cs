using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("SM_TwitterPost")]
[Index("TwitterPostCampaignId", Name = "IX_SM_TwitterPost_TwitterPostCampaignID")]
[Index("TwitterPostSiteId", Name = "IX_SM_TwitterPost_TwitterPostSiteID")]
[Index("TwitterPostTwitterAccountId", Name = "IX_SM_TwitterPost_TwitterPostTwitterAccountID")]
public partial class SmTwitterPost
{
    [Key]
    [Column("TwitterPostID")]
    public int TwitterPostId { get; set; }

    [Column("TwitterPostGUID")]
    public Guid TwitterPostGuid { get; set; }

    public DateTime TwitterPostLastModified { get; set; }

    [Column("TwitterPostSiteID")]
    public int TwitterPostSiteId { get; set; }

    [Column("TwitterPostTwitterAccountID")]
    public int TwitterPostTwitterAccountId { get; set; }

    public string TwitterPostText { get; set; } = null!;

    [Column("TwitterPostURLShortenerType")]
    public int? TwitterPostUrlshortenerType { get; set; }

    [Column("TwitterPostExternalID")]
    public string? TwitterPostExternalId { get; set; }

    public int? TwitterPostErrorCode { get; set; }

    public DateTime? TwitterPostPublishedDateTime { get; set; }

    public DateTime? TwitterPostScheduledPublishDateTime { get; set; }

    [Column("TwitterPostCampaignID")]
    public int? TwitterPostCampaignId { get; set; }

    public int? TwitterPostFavorites { get; set; }

    public int? TwitterPostRetweets { get; set; }

    public bool? TwitterPostPostAfterDocumentPublish { get; set; }

    public DateTime? TwitterPostInsightsUpdateDateTime { get; set; }

    [Column("TwitterPostDocumentGUID")]
    public Guid? TwitterPostDocumentGuid { get; set; }

    public bool? TwitterPostIsCreatedByUser { get; set; }

    [ForeignKey("TwitterPostCampaignId")]
    [InverseProperty("SmTwitterPosts")]
    public virtual AnalyticsCampaign? TwitterPostCampaign { get; set; }

    [ForeignKey("TwitterPostSiteId")]
    [InverseProperty("SmTwitterPosts")]
    public virtual CmsSite TwitterPostSite { get; set; } = null!;

    [ForeignKey("TwitterPostTwitterAccountId")]
    [InverseProperty("SmTwitterPosts")]
    public virtual SmTwitterAccount TwitterPostTwitterAccount { get; set; } = null!;
}
