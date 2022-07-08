using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_Price")]
    public partial class XperiencePrice
    {
        [Key]
        [Column("PriceID")]
        public int PriceId { get; set; }
        public Guid PriceGuid { get; set; }
        public DateTime PriceLastModified { get; set; }
        [Column("USDPrice", TypeName = "decimal(18, 9)")]
        public decimal? Usdprice { get; set; }
        [Column("GBPPrice", TypeName = "decimal(18, 9)")]
        public decimal? Gbpprice { get; set; }
        [Column("EURPrice", TypeName = "decimal(18, 9)")]
        public decimal? Eurprice { get; set; }
        [Column("AUDPrice", TypeName = "decimal(18, 9)")]
        public decimal? Audprice { get; set; }
        [Column("CZKPrice", TypeName = "decimal(18, 9)")]
        public decimal? Czkprice { get; set; }
        public Guid PriceNodeGuid { get; set; }
        [Column("JPYPrice", TypeName = "decimal(18, 9)")]
        public decimal? Jpyprice { get; set; }
        [Column("USDAnnualPrice", TypeName = "decimal(18, 9)")]
        public decimal? UsdannualPrice { get; set; }
        [Column("GBPAnnualPrice", TypeName = "decimal(18, 9)")]
        public decimal? GbpannualPrice { get; set; }
        [Column("EURAnnualPrice", TypeName = "decimal(18, 9)")]
        public decimal? EurannualPrice { get; set; }
        [Column("AUDAnnualPrice", TypeName = "decimal(18, 9)")]
        public decimal? AudannualPrice { get; set; }
        [Column("CZKAnnualPrice", TypeName = "decimal(18, 9)")]
        public decimal? CzkannualPrice { get; set; }
        [Column("JPYAnnualPrice", TypeName = "decimal(18, 9)")]
        public decimal? JpyannualPrice { get; set; }
        [Column("USDShareitCode")]
        [StringLength(20)]
        public string? UsdshareitCode { get; set; }
        [Column("GBPShareitCode")]
        [StringLength(20)]
        public string? GbpshareitCode { get; set; }
        [Column("EURShareitCode")]
        [StringLength(20)]
        public string? EurshareitCode { get; set; }
    }
}
