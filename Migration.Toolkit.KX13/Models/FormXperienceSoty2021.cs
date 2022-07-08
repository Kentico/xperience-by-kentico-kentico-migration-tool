using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_SOTY_2021")]
    public partial class FormXperienceSoty2021
    {
        [Key]
        [Column("SOTY_2021ID")]
        public int Soty2021id { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string EmailInput { get; set; } = null!;
        [StringLength(500)]
        public string SiteName { get; set; } = null!;
        [Column("SiteURL")]
        [StringLength(500)]
        public string SiteUrl { get; set; } = null!;
        [StringLength(200)]
        public string? CountryOfImplementation { get; set; }
        [StringLength(500)]
        public string ImplementationBy { get; set; } = null!;
        [StringLength(200)]
        public string Category { get; set; } = null!;
        [StringLength(200)]
        public string Version { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public string Why { get; set; } = null!;
        [Column("WhyKX")]
        public string WhyKx { get; set; } = null!;
        public string HowFeatures { get; set; } = null!;
        public string Outcomes { get; set; } = null!;
        public string? OnlineMarketingFeatures { get; set; }
        public string? Integration { get; set; }
        public string? Headless { get; set; }
        public string? Design { get; set; }
        [StringLength(500)]
        public string? CookieUtmCampaign { get; set; }
        [StringLength(500)]
        public string? CookieUtmSource { get; set; }
        [StringLength(500)]
        public string? CookieUtmMedium { get; set; }
        public Guid? CustomConsentAgreement { get; set; }
    }
}
