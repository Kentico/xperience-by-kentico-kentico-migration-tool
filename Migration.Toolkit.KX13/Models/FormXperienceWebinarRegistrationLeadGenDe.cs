using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_WebinarRegistration_LeadGen_DE")]
    public partial class FormXperienceWebinarRegistrationLeadGenDe
    {
        [Key]
        [Column("WebinarRegistration_LeadGen_DEID")]
        public int WebinarRegistrationLeadGenDeid { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string FirstName { get; set; } = null!;
        [StringLength(500)]
        public string LastName { get; set; } = null!;
        [StringLength(500)]
        public string Email { get; set; } = null!;
        [StringLength(500)]
        public string Company { get; set; } = null!;
        [StringLength(500)]
        public string? Jobtitle { get; set; }
        public Guid? CustomConsentAgreement { get; set; }
        [StringLength(500)]
        public string? Campaign { get; set; }
        [StringLength(500)]
        public string? CookieUtmSource { get; set; }
        [StringLength(500)]
        public string? CookieUtmMedium { get; set; }
        [Column("PageURL")]
        [StringLength(200)]
        public string? PageUrl { get; set; }
        [Column("WebinarEventID")]
        [StringLength(200)]
        public string? WebinarEventId { get; set; }
        [StringLength(4)]
        public string? InvisibleRecaptchaV3 { get; set; }
    }
}
