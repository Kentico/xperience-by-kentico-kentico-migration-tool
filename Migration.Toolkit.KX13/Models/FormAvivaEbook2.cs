using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_AvivaEbook_2")]
    public partial class FormAvivaEbook2
    {
        [Key]
        [Column("Resource_AvivaEbook_2ID")]
        public int ResourceAvivaEbook2id { get; set; }
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
        public Guid? CustomConsentAgreement { get; set; }
        [StringLength(500)]
        public string? CookieUtmCampaign { get; set; }
        [StringLength(500)]
        public string? CookieUtmSource { get; set; }
        [StringLength(500)]
        public string? CookieUtmMedium { get; set; }
        [StringLength(4)]
        public string? InvisibleRecaptchaV3 { get; set; }
    }
}
