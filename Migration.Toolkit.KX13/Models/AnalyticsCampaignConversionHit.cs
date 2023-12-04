using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Analytics_CampaignConversionHits")]
[Index("CampaignConversionHitsConversionId", Name = "IX_Analytics_CampaignConversionHits_CampaignConversionHitsConversionID")]
public partial class AnalyticsCampaignConversionHit
{
    [Key]
    [Column("CampaignConversionHitsID")]
    public int CampaignConversionHitsId { get; set; }

    [Column("CampaignConversionHitsConversionID")]
    public int CampaignConversionHitsConversionId { get; set; }

    public int CampaignConversionHitsCount { get; set; }

    [StringLength(200)]
    public string CampaignConversionHitsSourceName { get; set; } = null!;

    [StringLength(200)]
    public string? CampaignConversionHitsContentName { get; set; }

    [ForeignKey("CampaignConversionHitsConversionId")]
    [InverseProperty("AnalyticsCampaignConversionHits")]
    public virtual AnalyticsCampaignConversion CampaignConversionHitsConversion { get; set; } = null!;
}
