using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_KX_Sydney_Partner_Day")]
    public partial class FormXperienceKxSydneyPartnerDay
    {
        [Key]
        [Column("KX_Sydney_Partner_DayID")]
        public int KxSydneyPartnerDayId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string FirstName { get; set; } = null!;
        [StringLength(500)]
        public string LastName { get; set; } = null!;
        [StringLength(500)]
        public string Company { get; set; } = null!;
        [StringLength(500)]
        public string Jobposition { get; set; } = null!;
        [StringLength(500)]
        public string EmailInput { get; set; } = null!;
        public Guid? CustomConsentAgreement { get; set; }
        [StringLength(4)]
        public string? InvisibleRecaptchaV3 { get; set; }
    }
}
