using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_AU_finance_march_2022")]
    public partial class FormXperienceAuFinanceMarch2022
    {
        [Key]
        [Column("AU_finance_march_2022ID")]
        public int AuFinanceMarch2022id { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string FirstName { get; set; } = null!;
        [StringLength(500)]
        public string LastName { get; set; } = null!;
        [StringLength(500)]
        public string Company { get; set; } = null!;
        public Guid? CustomConsentAgreement { get; set; }
        [StringLength(500)]
        public string EmailInput { get; set; } = null!;
        [StringLength(200)]
        public string PhoneInput { get; set; } = null!;
        [StringLength(200)]
        public string Jobtitle { get; set; } = null!;
        [StringLength(4)]
        public string? InvisibleRecaptchaV3 { get; set; }
    }
}
