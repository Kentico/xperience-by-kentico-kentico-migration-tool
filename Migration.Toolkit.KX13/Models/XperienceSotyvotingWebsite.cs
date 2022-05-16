using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_SOTYVotingWebsite")]
    public partial class XperienceSotyvotingWebsite
    {
        [Key]
        [Column("SOTYVotingWebsiteID")]
        public int SotyvotingWebsiteId { get; set; }
        [Column("SOTYVotingWebsiteSiteID")]
        public int SotyvotingWebsiteSiteId { get; set; }
        [Column("SOTYVotingWebsiteImage")]
        [StringLength(300)]
        public string SotyvotingWebsiteImage { get; set; } = null!;
        [Column("SOTYVotingWebsiteName")]
        [StringLength(200)]
        public string SotyvotingWebsiteName { get; set; } = null!;
        [Column("SOTYVotingWebsiteSummary")]
        [StringLength(500)]
        public string SotyvotingWebsiteSummary { get; set; } = null!;
        [Column("SOTYVotingWebsiteTag")]
        [StringLength(200)]
        public string SotyvotingWebsiteTag { get; set; } = null!;
        [Column("SOTYVotingWebsiteUrl")]
        [StringLength(200)]
        public string SotyvotingWebsiteUrl { get; set; } = null!;
    }
}
