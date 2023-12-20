using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Analytics_Campaign")]
[Index("CampaignScheduledTaskId", Name = "IX_Analytics_Campaign_CampaignScheduledTaskID")]
[Index("CampaignSiteId", Name = "IX_Analytics_Campaign_CampaignSiteID")]
public partial class AnalyticsCampaign
{
    [Key]
    [Column("CampaignID")]
    public int CampaignId { get; set; }

    [StringLength(200)]
    public string CampaignName { get; set; } = null!;

    [StringLength(100)]
    public string CampaignDisplayName { get; set; } = null!;

    public string? CampaignDescription { get; set; }

    [Column("CampaignSiteID")]
    public int CampaignSiteId { get; set; }

    public DateTime? CampaignOpenFrom { get; set; }

    public DateTime? CampaignOpenTo { get; set; }

    [Column("CampaignGUID")]
    public Guid CampaignGuid { get; set; }

    public DateTime CampaignLastModified { get; set; }

    [Column("CampaignUTMCode")]
    [StringLength(200)]
    public string? CampaignUtmcode { get; set; }

    public DateTime? CampaignCalculatedTo { get; set; }

    [Column("CampaignScheduledTaskID")]
    public int? CampaignScheduledTaskId { get; set; }

    public int? CampaignVisitors { get; set; }

    [InverseProperty("CampaignAssetCampaign")]
    public virtual ICollection<AnalyticsCampaignAsset> AnalyticsCampaignAssets { get; set; } = new List<AnalyticsCampaignAsset>();

    [InverseProperty("CampaignConversionCampaign")]
    public virtual ICollection<AnalyticsCampaignConversion> AnalyticsCampaignConversions { get; set; } = new List<AnalyticsCampaignConversion>();

    [InverseProperty("CampaignObjectiveCampaign")]
    public virtual AnalyticsCampaignObjective? AnalyticsCampaignObjective { get; set; }

    [ForeignKey("CampaignScheduledTaskId")]
    [InverseProperty("AnalyticsCampaigns")]
    public virtual CmsScheduledTask? CampaignScheduledTask { get; set; }

    [ForeignKey("CampaignSiteId")]
    [InverseProperty("AnalyticsCampaigns")]
    public virtual CmsSite CampaignSite { get; set; } = null!;

    [InverseProperty("FacebookPostCampaign")]
    public virtual ICollection<SmFacebookPost> SmFacebookPosts { get; set; } = new List<SmFacebookPost>();

    [InverseProperty("LinkedInPostCampaign")]
    public virtual ICollection<SmLinkedInPost> SmLinkedInPosts { get; set; } = new List<SmLinkedInPost>();

    [InverseProperty("TwitterPostCampaign")]
    public virtual ICollection<SmTwitterPost> SmTwitterPosts { get; set; } = new List<SmTwitterPost>();
}
