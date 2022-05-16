using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_HostedTrialRegistration")]
    public partial class FormXperienceHostedTrialRegistration
    {
        [Key]
        [Column("HostedTrialRegistrationID")]
        public int HostedTrialRegistrationId { get; set; }
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
        [StringLength(200)]
        public string Country { get; set; } = null!;
        [StringLength(200)]
        public string? WhasIsYourRole { get; set; }
        public bool? GeneralNewsletterSubscription { get; set; }
        [Column("AreYouLookingForCMS")]
        [StringLength(200)]
        public string? AreYouLookingForCms { get; set; }
        public Guid? CustomConsentAgreement { get; set; }
        public Guid? AutomaticFollowupConsent { get; set; }
        [StringLength(200)]
        public string? Phone { get; set; }
        [StringLength(4)]
        public string? InvisibleRecaptchaV3 { get; set; }
    }
}
