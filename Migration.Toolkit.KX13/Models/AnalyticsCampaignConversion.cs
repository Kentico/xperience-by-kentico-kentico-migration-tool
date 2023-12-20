using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Analytics_CampaignConversion")]
[Index("CampaignConversionCampaignId", Name = "IX_Analytics_CampaignConversion_CampaignConversionCampaignID")]
public partial class AnalyticsCampaignConversion
{
    [Key]
    [Column("CampaignConversionID")]
    public int CampaignConversionId { get; set; }

    public Guid CampaignConversionGuid { get; set; }

    public DateTime CampaignConversionLastModified { get; set; }

    [StringLength(100)]
    public string CampaignConversionDisplayName { get; set; } = null!;

    [StringLength(100)]
    public string CampaignConversionName { get; set; } = null!;

    [Column("CampaignConversionCampaignID")]
    public int CampaignConversionCampaignId { get; set; }

    public int CampaignConversionOrder { get; set; }

    [StringLength(250)]
    public string CampaignConversionActivityType { get; set; } = null!;

    public int CampaignConversionHits { get; set; }

    [Column("CampaignConversionItemID")]
    public int? CampaignConversionItemId { get; set; }

    public double CampaignConversionValue { get; set; }

    public bool CampaignConversionIsFunnelStep { get; set; }

    [Column("CampaignConversionURL")]
    public string? CampaignConversionUrl { get; set; }

    [InverseProperty("CampaignConversionHitsConversion")]
    public virtual ICollection<AnalyticsCampaignConversionHit> AnalyticsCampaignConversionHits { get; set; } = new List<AnalyticsCampaignConversionHit>();

    [InverseProperty("CampaignObjectiveCampaignConversion")]
    public virtual ICollection<AnalyticsCampaignObjective> AnalyticsCampaignObjectives { get; set; } = new List<AnalyticsCampaignObjective>();

    [ForeignKey("CampaignConversionCampaignId")]
    [InverseProperty("AnalyticsCampaignConversions")]
    public virtual AnalyticsCampaign CampaignConversionCampaign { get; set; } = null!;
}
