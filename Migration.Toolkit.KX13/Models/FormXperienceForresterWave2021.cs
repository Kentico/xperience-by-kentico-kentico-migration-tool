using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_Forrester_Wave_2021")]
    public partial class FormXperienceForresterWave2021
    {
        [Key]
        [Column("Forrester_wave_2021ID")]
        public int ForresterWave2021id { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string FirstName { get; set; } = null!;
        [StringLength(500)]
        public string LastName { get; set; } = null!;
        [StringLength(500)]
        public string BusinessEmail { get; set; } = null!;
        [StringLength(500)]
        public string Company { get; set; } = null!;
        [StringLength(500)]
        public string? TextInput { get; set; }
        [StringLength(200)]
        public string Country { get; set; } = null!;
        public Guid? CustomConsentAgreement { get; set; }
        [StringLength(500)]
        public string? CookieUtmCampaign { get; set; }
        [StringLength(500)]
        public string? CookieUtmSource { get; set; }
        [StringLength(500)]
        public string? CookieUtmMedium { get; set; }
        [StringLength(200)]
        public string Phone { get; set; } = null!;
        [StringLength(4)]
        public string? InvisibleRecaptchaV3 { get; set; }
    }
}
