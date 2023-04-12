using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("EmailLibrary_EmailLink")]
    [Index("EmailLinkEmailConfigurationId", Name = "IX_EmailLibrary_EmailLink_EmailLinkEmailConfigurationID")]
    public partial class EmailLibraryEmailLink
    {
        public EmailLibraryEmailLink()
        {
            EmailLibraryEmailStatisticsHits = new HashSet<EmailLibraryEmailStatisticsHit>();
        }

        [Key]
        [Column("EmailLinkID")]
        public int EmailLinkId { get; set; }
        [Column("EmailLinkEmailConfigurationID")]
        public int EmailLinkEmailConfigurationId { get; set; }
        public string EmailLinkTarget { get; set; } = null!;
        [StringLength(450)]
        public string EmailLinkDescription { get; set; } = null!;
        [Column("EmailLinkGUID")]
        public Guid EmailLinkGuid { get; set; }

        [ForeignKey("EmailLinkEmailConfigurationId")]
        [InverseProperty("EmailLibraryEmailLinks")]
        public virtual EmailLibraryEmailConfiguration EmailLinkEmailConfiguration { get; set; } = null!;
        [InverseProperty("EmailStatisticsHitsEmailLink")]
        public virtual ICollection<EmailLibraryEmailStatisticsHit> EmailLibraryEmailStatisticsHits { get; set; }
    }
}
