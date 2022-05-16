using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_Zzz_RealEstateNewMortgage")]
    public partial class FormXperienceZzzRealEstateNewMortgage
    {
        [Key]
        [Column("Zzz_RealEstateNewMortgageID")]
        public int ZzzRealEstateNewMortgageId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        public string? FirstName { get; set; }
        [StringLength(500)]
        public string? LastName { get; set; }
        [StringLength(200)]
        public string? PhoneInput { get; set; }
        [StringLength(500)]
        public string? EmailInput { get; set; }
        [StringLength(1)]
        public string? Recaptcha { get; set; }
    }
}
