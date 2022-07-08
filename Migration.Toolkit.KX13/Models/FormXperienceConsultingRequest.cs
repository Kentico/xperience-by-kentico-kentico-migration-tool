using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_ConsultingRequest")]
    public partial class FormXperienceConsultingRequest
    {
        [Key]
        [Column("KenticoConsultingRequest_xpID")]
        public int KenticoConsultingRequestXpId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string? Attachment { get; set; }
        [StringLength(500)]
        public string? FirstName { get; set; }
        [StringLength(500)]
        public string? LastName { get; set; }
        [StringLength(500)]
        public string CompanyName { get; set; } = null!;
        [StringLength(200)]
        public string Country { get; set; } = null!;
        [StringLength(500)]
        public string Email { get; set; } = null!;
        public string Topics { get; set; } = null!;
        [StringLength(500)]
        public string? SuggestedTime { get; set; }
        [StringLength(500)]
        public string? CookieUtmCampaign { get; set; }
        [StringLength(500)]
        public string? CookieUtmSource { get; set; }
        [StringLength(500)]
        public string? CookieUtmMedium { get; set; }
        public Guid? AutomaticFollowupConsent { get; set; }
        [StringLength(4)]
        public string? InvisibleRecaptchaV3 { get; set; }
    }
}
