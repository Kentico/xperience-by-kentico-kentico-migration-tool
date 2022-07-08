using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("customtable_SOTYVotingResults")]
    public partial class CustomtableSotyvotingResult
    {
        [Key]
        [Column("ItemID")]
        public int ItemId { get; set; }
        public int? ItemCreatedBy { get; set; }
        public DateTime? ItemCreatedWhen { get; set; }
        public int? ItemModifiedBy { get; set; }
        public DateTime? ItemModifiedWhen { get; set; }
        public int? ItemOrder { get; set; }
        [Column("ItemGUID")]
        public Guid ItemGuid { get; set; }
        [Column("SiteID")]
        public int SiteId { get; set; }
        [Column("SubmissionIP")]
        [StringLength(200)]
        public string? SubmissionIp { get; set; }
        [StringLength(600)]
        public string? SubmissionUserAgent { get; set; }
    }
}
