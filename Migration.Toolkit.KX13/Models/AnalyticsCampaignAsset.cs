using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Analytics_CampaignAsset")]
[Index("CampaignAssetCampaignId", Name = "IX_Analytics_CampaignAsset_CampaignAssetCampaignID")]
public partial class AnalyticsCampaignAsset
{
    [Key]
    [Column("CampaignAssetID")]
    public int CampaignAssetId { get; set; }

    public Guid CampaignAssetGuid { get; set; }

    public DateTime CampaignAssetLastModified { get; set; }

    public Guid CampaignAssetAssetGuid { get; set; }

    [Column("CampaignAssetCampaignID")]
    public int CampaignAssetCampaignId { get; set; }

    [StringLength(200)]
    public string CampaignAssetType { get; set; } = null!;

    [InverseProperty("CampaignAssetUrlCampaignAsset")]
    public virtual ICollection<AnalyticsCampaignAssetUrl> AnalyticsCampaignAssetUrls { get; set; } = new List<AnalyticsCampaignAssetUrl>();

    [ForeignKey("CampaignAssetCampaignId")]
    [InverseProperty("AnalyticsCampaignAssets")]
    public virtual AnalyticsCampaign CampaignAssetCampaign { get; set; } = null!;
}
