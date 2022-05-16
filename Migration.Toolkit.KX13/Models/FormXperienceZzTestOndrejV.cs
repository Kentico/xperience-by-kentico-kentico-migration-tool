using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_ZzTestOndrejV")]
    public partial class FormXperienceZzTestOndrejV
    {
        [Key]
        [Column("ZzTestOndrejVID")]
        public int ZzTestOndrejVid { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [Column("TextInput_1")]
        [StringLength(500)]
        public string? TextInput1 { get; set; }
        [Column("TextInput_2")]
        [StringLength(500)]
        public string? TextInput2 { get; set; }
        [StringLength(500)]
        public string EmailInput { get; set; } = null!;
        [StringLength(500)]
        public string? Company { get; set; }
        public Guid? Consent { get; set; }
        [StringLength(200)]
        public string Country { get; set; } = null!;
        [StringLength(4)]
        public string? InvisibleRecaptchaV3 { get; set; }
        [StringLength(200)]
        public string? TextBlock { get; set; }
        [StringLength(200)]
        public string? Phone { get; set; }
    }
}
