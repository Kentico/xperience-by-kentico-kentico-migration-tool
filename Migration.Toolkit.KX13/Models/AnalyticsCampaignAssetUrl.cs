using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Analytics_CampaignAssetUrl")]
[Index("CampaignAssetUrlCampaignAssetId", Name = "IX_Analytics_CampaignAssetUrl_CampaignAssetUrlCampaignAssetID")]
public partial class AnalyticsCampaignAssetUrl
{
    [Key]
    [Column("CampaignAssetUrlID")]
    public int CampaignAssetUrlId { get; set; }

    public Guid CampaignAssetUrlGuid { get; set; }

    public string CampaignAssetUrlTarget { get; set; } = null!;

    [StringLength(200)]
    public string CampaignAssetUrlPageTitle { get; set; } = null!;

    [Column("CampaignAssetUrlCampaignAssetID")]
    public int CampaignAssetUrlCampaignAssetId { get; set; }

    [ForeignKey("CampaignAssetUrlCampaignAssetId")]
    [InverseProperty("AnalyticsCampaignAssetUrls")]
    public virtual AnalyticsCampaignAsset CampaignAssetUrlCampaignAsset { get; set; } = null!;
}
