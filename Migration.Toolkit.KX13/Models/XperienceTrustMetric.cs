using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_TrustMetrics")]
    public partial class XperienceTrustMetric
    {
        [Key]
        [Column("TrustMetricsID")]
        public int TrustMetricsId { get; set; }
        [StringLength(200)]
        public string? Title { get; set; }
        [StringLength(200)]
        public string? Item1Title { get; set; }
        public int? Item1StartValue { get; set; }
        public int? Item1EndValue { get; set; }
        [StringLength(200)]
        public string? Item1Prefix { get; set; }
        [StringLength(200)]
        public string? Item1Suffix { get; set; }
        [StringLength(200)]
        public string? Item2Title { get; set; }
        public int? Item2StartValue { get; set; }
        public int? Item2EndValue { get; set; }
        [StringLength(200)]
        public string? Item2Prefix { get; set; }
        [StringLength(200)]
        public string? Item2Suffix { get; set; }
        [StringLength(200)]
        public string? Item3Title { get; set; }
        public int? Item3StartValue { get; set; }
        public int? Item3EndValue { get; set; }
        [StringLength(200)]
        public string? Item3Prefix { get; set; }
        [StringLength(200)]
        public string? Item3Suffix { get; set; }
        public string? Item1Content { get; set; }
        [StringLength(200)]
        public string? Item1CtaText { get; set; }
        [StringLength(200)]
        public string? Item1LogoPublicId { get; set; }
        public Guid? Item1UrlInternal { get; set; }
        [StringLength(512)]
        public string? Item1UrlExternal { get; set; }
        public string? Item2Content { get; set; }
        [StringLength(200)]
        public string? Item2CtaText { get; set; }
        [StringLength(200)]
        public string? Item2LogoPublicId { get; set; }
        public Guid? Item2UrlInternal { get; set; }
        [StringLength(512)]
        public string? Item2UrlExternal { get; set; }
        public string? Item3Content { get; set; }
        [StringLength(200)]
        public string? Item3CtaText { get; set; }
        [StringLength(200)]
        public string? Item3LogoPublicId { get; set; }
        public Guid? Item3UrlInternal { get; set; }
        [StringLength(512)]
        public string? Item3UrlExternal { get; set; }
    }
}
