using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_WebinarRegistration_Connreg")]
    public partial class FormXperienceWebinarRegistrationConnreg
    {
        [Key]
        [Column("WebinarRegistration_ConnregID")]
        public int WebinarRegistrationConnregId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string FirstName { get; set; } = null!;
        [StringLength(500)]
        public string LastName { get; set; } = null!;
        [StringLength(500)]
        public string Email { get; set; } = null!;
        [StringLength(500)]
        public string? Company { get; set; }
        [StringLength(500)]
        public string Jobtitle { get; set; } = null!;
        [StringLength(200)]
        public string Country { get; set; } = null!;
        public Guid? ConsentAgreement { get; set; }
        [StringLength(500)]
        public string? CookieUtmCampaign { get; set; }
        [StringLength(500)]
        public string? CookieUtmSource { get; set; }
        [StringLength(500)]
        public string? CookieUtmMedium { get; set; }
        [Column("PageURL")]
        [StringLength(200)]
        public string? PageUrl { get; set; }
        [StringLength(200)]
        public string TimePreference { get; set; } = null!;
        [StringLength(200)]
        public string Type { get; set; } = null!;
        [StringLength(4)]
        public string? InvisibleRecaptchaV3 { get; set; }
    }
}
