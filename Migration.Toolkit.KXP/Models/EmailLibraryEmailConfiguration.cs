using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("EmailLibrary_EmailConfiguration")]
    [Index("EmailConfigurationEmailTemplateId", Name = "IX_EmailLibrary_EmailConfiguration_EmailConfigurationEmailTemplateID")]
    [Index("EmailConfigurationSiteId", Name = "IX_EmailLibrary_EmailConfiguration_EmailConfigurationSiteID")]
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
        [StringLength(250)]
        public string EmailConfigurationDisplayName { get; set; } = null!;
        [StringLength(450)]
        public string? EmailConfigurationSubject { get; set; }
        [StringLength(200)]
        public string? EmailConfigurationSenderName { get; set; }
        [StringLength(254)]
        public string? EmailConfigurationSenderEmail { get; set; }
        [Column("EmailConfigurationEmailTemplateID")]
        public int EmailConfigurationEmailTemplateId { get; set; }
        public string? EmailConfigurationContent { get; set; }
        [Column("EmailConfigurationSiteID")]
        public int? EmailConfigurationSiteId { get; set; }
        [Column("EmailConfigurationGUID")]
        public Guid EmailConfigurationGuid { get; set; }
        public DateTime EmailConfigurationLastModified { get; set; }
        public string? EmailConfigurationPlainText { get; set; }
        public string? EmailConfigurationPreheader { get; set; }
        [StringLength(50)]
        public string EmailConfigurationType { get; set; } = null!;

        [ForeignKey("EmailConfigurationEmailTemplateId")]
        [InverseProperty("EmailLibraryEmailConfigurations")]
        public virtual EmailLibraryEmailTemplate EmailConfigurationEmailTemplate { get; set; } = null!;
        [ForeignKey("EmailConfigurationSiteId")]
        [InverseProperty("EmailLibraryEmailConfigurations")]
        public virtual CmsSite? EmailConfigurationSite { get; set; }
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
