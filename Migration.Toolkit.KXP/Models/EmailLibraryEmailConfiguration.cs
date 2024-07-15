using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models;

[Table("EmailLibrary_EmailConfiguration")]
[Index("EmailConfigurationContentItemId", Name = "IX_EmailLibrary_EmailConfiguration_EmailConfigurationContentItemID")]
[Index("EmailConfigurationEmailChannelId", Name = "IX_EmailLibrary_EmailConfiguration_EmailConfigurationEmailChannelID")]
public partial class EmailLibraryEmailConfiguration
{
    [Key]
    [Column("EmailConfigurationID")]
    public int EmailConfigurationId { get; set; }

    [StringLength(250)]
    public string EmailConfigurationName { get; set; } = null!;

    [Column("EmailConfigurationGUID")]
    public Guid EmailConfigurationGuid { get; set; }

    public DateTime EmailConfigurationLastModified { get; set; }

    [StringLength(50)]
    public string EmailConfigurationPurpose { get; set; } = null!;

    [Column("EmailConfigurationEmailChannelID")]
    public int? EmailConfigurationEmailChannelId { get; set; }

    [Column("EmailConfigurationContentItemID")]
    public int? EmailConfigurationContentItemId { get; set; }

    [InverseProperty("EmailEmailConfiguration")]
    public virtual ICollection<CmsEmail> CmsEmails { get; set; } = new List<CmsEmail>();

    [ForeignKey("EmailConfigurationContentItemId")]
    [InverseProperty("EmailLibraryEmailConfigurations")]
    public virtual CmsContentItem? EmailConfigurationContentItem { get; set; }

    [ForeignKey("EmailConfigurationEmailChannelId")]
    [InverseProperty("EmailLibraryEmailConfigurations")]
    public virtual EmailLibraryEmailChannel? EmailConfigurationEmailChannel { get; set; }

    [InverseProperty("EmailLinkEmailConfiguration")]
    public virtual ICollection<EmailLibraryEmailLink> EmailLibraryEmailLinks { get; set; } = new List<EmailLibraryEmailLink>();

    [InverseProperty("EmailMarketingRecipientEmailConfiguration")]
    public virtual ICollection<EmailLibraryEmailMarketingRecipient> EmailLibraryEmailMarketingRecipients { get; set; } = new List<EmailLibraryEmailMarketingRecipient>();

    [InverseProperty("EmailStatisticsEmailConfiguration")]
    public virtual ICollection<EmailLibraryEmailStatistic> EmailLibraryEmailStatistics { get; set; } = new List<EmailLibraryEmailStatistic>();

    [InverseProperty("EmailStatisticsHitsEmailConfiguration")]
    public virtual ICollection<EmailLibraryEmailStatisticsHit> EmailLibraryEmailStatisticsHits { get; set; } = new List<EmailLibraryEmailStatisticsHit>();

    [InverseProperty("SendConfigurationEmailConfiguration")]
    public virtual EmailLibrarySendConfiguration? EmailLibrarySendConfiguration { get; set; }
}