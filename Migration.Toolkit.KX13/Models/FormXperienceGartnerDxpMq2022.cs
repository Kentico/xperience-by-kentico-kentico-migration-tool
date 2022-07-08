using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_Gartner_DXP_MQ_2022")]
    public partial class FormXperienceGartnerDxpMq2022
    {
        [Key]
        [Column("Gartner_DXP_MQ_22ID")]
        public int GartnerDxpMq22id { get; set; }
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
        public Guid? CustomConsentAgreement { get; set; }
        [StringLength(500)]
        public string? CookieUtmCampaign { get; set; }
        [StringLength(500)]
        public string? CookieUtmSource { get; set; }
        [StringLength(500)]
        public string? CookieUtmMedium { get; set; }
        [StringLength(4)]
        public string? InvisibleRecaptchaV3 { get; set; }
        [StringLength(200)]
        public string? JobPosition { get; set; }
        [StringLength(200)]
        public string Phone { get; set; } = null!;
        [StringLength(200)]
        public string? DocumentFieldComponent { get; set; }
    }
}
