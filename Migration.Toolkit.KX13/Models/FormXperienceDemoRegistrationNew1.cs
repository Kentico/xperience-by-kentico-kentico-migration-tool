using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_DemoRegistrationNew_1")]
    public partial class FormXperienceDemoRegistrationNew1
    {
        [Key]
        [Column("DemoRegistrationNew_1ID")]
        public int DemoRegistrationNew1id { get; set; }
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
        public string? Phone { get; set; }
        [StringLength(200)]
        public string Country { get; set; } = null!;
        public bool? GeneralNewsletterSubscription { get; set; }
        public Guid? CustomConsentAgreement { get; set; }
        public Guid? AutomaticFollowupConsent { get; set; }
        [StringLength(1)]
        public string? Recaptcha { get; set; }
        [StringLength(500)]
        public string? CookieUtmCampaign { get; set; }
        [StringLength(500)]
        public string? CookieUtmSource { get; set; }
        [StringLength(500)]
        public string? CookieUtmMedium { get; set; }
        [Column("FormGUID")]
        [StringLength(500)]
        public string? FormGuid { get; set; }
        [Column("PageURL")]
        [StringLength(200)]
        public string? PageUrl { get; set; }
    }
}
