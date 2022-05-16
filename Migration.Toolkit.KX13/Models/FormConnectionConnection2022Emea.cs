using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Connection_Connection2022EMEA")]
    public partial class FormConnectionConnection2022Emea
    {
        [Key]
        [Column("Connection2022EMEAID")]
        public int Connection2022Emeaid { get; set; }
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
        [StringLength(200)]
        public string Type { get; set; } = null!;
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
        [Column("InvisibleRecaptchaV3EMEA2022")]
        [StringLength(4)]
        public string? InvisibleRecaptchaV3emea2022 { get; set; }
        [StringLength(200)]
        public string? Date { get; set; }
    }
}
