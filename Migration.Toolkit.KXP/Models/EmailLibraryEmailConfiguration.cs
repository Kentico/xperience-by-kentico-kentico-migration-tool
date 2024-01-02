using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("EmailLibrary_EmailConfiguration")]
    [Index("EmailConfigurationContentItemId", Name = "IX_EmailLibrary_EmailConfiguration_EmailConfigurationContentItemID")]
    [Index("EmailConfigurationEmailChannelId", Name = "IX_EmailLibrary_EmailConfiguration_EmailConfigurationEmailChannelID")]
    public partial class EmailLibraryEmailConfiguration
    {
        public EmailLibraryEmailConfiguration()
        {
            CmsEmails = new HashSet<CmsEmail>();
            EmailLibraryEmailLinks = new HashSet<EmailLibraryEmailLink>();
            EmailLibraryEmailMarketingRecipients = new HashSet<EmailLibraryEmailMarketingRecipient>();
            EmailLibraryEmailStatistics = new HashSet<EmailLibraryEmailStatistic>();
            EmailLibraryEmailStatisticsHits = new HashSet<EmailLibraryEmailStatisticsHit>();
        }

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

        [ForeignKey("EmailConfigurationContentItemId")]
        [InverseProperty("EmailLibraryEmailConfigurations")]
        public virtual CmsContentItem? EmailConfigurationContentItem { get; set; }
        [ForeignKey("EmailConfigurationEmailChannelId")]
        [InverseProperty("EmailLibraryEmailConfigurations")]
        public virtual EmailLibraryEmailChannel? EmailConfigurationEmailChannel { get; set; }
        [InverseProperty("SendConfigurationEmailConfiguration")]
        public virtual EmailLibrarySendConfiguration EmailLibrarySendConfiguration { get; set; } = null!;
        [InverseProperty("EmailEmailConfiguration")]
        public virtual ICollection<CmsEmail> CmsEmails { get; set; }
        [InverseProperty("EmailLinkEmailConfiguration")]
        public virtual ICollection<EmailLibraryEmailLink> EmailLibraryEmailLinks { get; set; }
        [InverseProperty("EmailMarketingRecipientEmailConfiguration")]
        public virtual ICollection<EmailLibraryEmailMarketingRecipient> EmailLibraryEmailMarketingRecipients { get; set; }
        [InverseProperty("EmailStatisticsEmailConfiguration")]
        public virtual ICollection<EmailLibraryEmailStatistic> EmailLibraryEmailStatistics { get; set; }
        [InverseProperty("EmailStatisticsHitsEmailConfiguration")]
        public virtual ICollection<EmailLibraryEmailStatisticsHit> EmailLibraryEmailStatisticsHits { get; set; }
    }
}
