using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Analytics_CampaignObjective")]
[Index("CampaignObjectiveCampaignId", Name = "CK_Analytics_CampaignObjective_CampaignObjectiveCampaignID", IsUnique = true)]
[Index("CampaignObjectiveCampaignConversionId", Name = "IX_Analytics_CampaignObjective_CampaignObjectiveCampaignConversionID")]
public partial class AnalyticsCampaignObjective
{
    [Key]
    [Column("CampaignObjectiveID")]
    public int CampaignObjectiveId { get; set; }

    public Guid CampaignObjectiveGuid { get; set; }

    public DateTime CampaignObjectiveLastModified { get; set; }

    [Column("CampaignObjectiveCampaignID")]
    public int CampaignObjectiveCampaignId { get; set; }

    public int? CampaignObjectiveValue { get; set; }

    [Column("CampaignObjectiveCampaignConversionID")]
    public int CampaignObjectiveCampaignConversionId { get; set; }

    [ForeignKey("CampaignObjectiveCampaignId")]
    [InverseProperty("AnalyticsCampaignObjective")]
    public virtual AnalyticsCampaign CampaignObjectiveCampaign { get; set; } = null!;

    [ForeignKey("CampaignObjectiveCampaignConversionId")]
    [InverseProperty("AnalyticsCampaignObjectives")]
    public virtual AnalyticsCampaignConversion CampaignObjectiveCampaignConversion { get; set; } = null!;
}
