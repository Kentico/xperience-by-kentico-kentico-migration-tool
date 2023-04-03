using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("EmailLibrary_EmailStatisticsHits")]
    [Index("EmailStatisticsHitsEmailConfigurationId", Name = "IX_EmailLibrary_EmailStatisticsHits_EmailStatisticsHitsEmailConfigurationID")]
    [Index("EmailStatisticsHitsEmailLinkId", Name = "IX_EmailLibrary_EmailStatisticsHits_EmailStatisticsHitsEmailLinkID")]
    public partial class EmailLibraryEmailStatisticsHit
    {
        [Key]
        [Column("EmailStatisticsHitsID")]
        public long EmailStatisticsHitsId { get; set; }
        [Column("EmailStatisticsHitsEmailConfigurationID")]
        public int EmailStatisticsHitsEmailConfigurationId { get; set; }
        public int EmailStatisticsHitsType { get; set; }
        public DateTime EmailStatisticsHitsTime { get; set; }
        [Column("EmailStatisticsHitsMailoutGUID")]
        public Guid EmailStatisticsHitsMailoutGuid { get; set; }
        [Column("EmailStatisticsHitsEmailLinkID")]
        public int? EmailStatisticsHitsEmailLinkId { get; set; }

        [ForeignKey("EmailStatisticsHitsEmailConfigurationId")]
        [InverseProperty("EmailLibraryEmailStatisticsHits")]
        public virtual EmailLibraryEmailConfiguration EmailStatisticsHitsEmailConfiguration { get; set; } = null!;
        [ForeignKey("EmailStatisticsHitsEmailLinkId")]
        [InverseProperty("EmailLibraryEmailStatisticsHits")]
        public virtual EmailLibraryEmailLink? EmailStatisticsHitsEmailLink { get; set; }
    }
}
