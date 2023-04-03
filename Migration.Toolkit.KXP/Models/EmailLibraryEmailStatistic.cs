using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("EmailLibrary_EmailStatistics")]
    [Index("EmailStatisticsEmailConfigurationId", Name = "IX_EmailLibrary_EmailStatistics_EmailStatisticsEmailConfigurationID")]
    public partial class EmailLibraryEmailStatistic
    {
        [Key]
        [Column("EmailStatisticsID")]
        public int EmailStatisticsId { get; set; }
        [Column("EmailStatisticsEmailConfigurationID")]
        public int EmailStatisticsEmailConfigurationId { get; set; }
        public int EmailStatisticsEmailOpens { get; set; }
        public int EmailStatisticsEmailUniqueOpens { get; set; }
        public int EmailStatisticsEmailClicks { get; set; }
        public int EmailStatisticsEmailUniqueClicks { get; set; }
        public int EmailStatisticsTotalSent { get; set; }

        [ForeignKey("EmailStatisticsEmailConfigurationId")]
        [InverseProperty("EmailLibraryEmailStatistics")]
        public virtual EmailLibraryEmailConfiguration EmailStatisticsEmailConfiguration { get; set; } = null!;
    }
}
