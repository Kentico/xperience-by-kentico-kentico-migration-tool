using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_DemoRegistrationNew")]
    public partial class FormXperienceDemoRegistrationNew
    {
        [Key]
        [Column("DemoRegistrationNew_xpID")]
        public int DemoRegistrationNewXpId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string FirstName { get; set; } = null!;
        [StringLength(500)]
        public string LastName { get; set; } = null!;
        [StringLength(500)]
        public string Company { get; set; } = null!;
        [StringLength(500)]
        public string Email { get; set; } = null!;
        [StringLength(200)]
        public string Country { get; set; } = null!;
        [StringLength(500)]
        public string? CookieUtmCampaign { get; set; }
        [StringLength(500)]
        public string? CookieUtmSource { get; set; }
        [StringLength(500)]
        public string? CookieUtmMedium { get; set; }
        public bool? GeneralNewsletterSubscription { get; set; }
        [Column("FormGUID")]
        [StringLength(500)]
        public string? FormGuid { get; set; }
        public Guid? AutomaticFollowupConsent { get; set; }
        public Guid? CustomConsentAgreement { get; set; }
        [Column("PageURL")]
        [StringLength(200)]
        public string? PageUrl { get; set; }
        [StringLength(200)]
        public string? Phone { get; set; }
        [StringLength(4)]
        public string? InvisibleRecaptchaV3 { get; set; }
    }
}
