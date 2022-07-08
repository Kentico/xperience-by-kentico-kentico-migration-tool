using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_ResourceDownloadFormTest")]
    public partial class FormXperienceResourceDownloadFormTest
    {
        [Key]
        [Column("ResourceDownloadFormTestID")]
        public int ResourceDownloadFormTestId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string TextInput { get; set; } = null!;
        [StringLength(500)]
        public string EmailAddress { get; set; } = null!;
        public string TextArea { get; set; } = null!;
        [StringLength(200)]
        public string? PhoneInput { get; set; }
        [StringLength(200)]
        public string CountrySelector { get; set; } = null!;
        [StringLength(200)]
        public string MultipleChoice { get; set; } = null!;
        [StringLength(200)]
        public string RadioButtons { get; set; } = null!;
        [StringLength(200)]
        public string DropDown { get; set; } = null!;
        public Guid? CustomConsentAgreement { get; set; }
        [StringLength(500)]
        public string? ResourceUrl { get; set; }
        [StringLength(500)]
        public string? PageUrl { get; set; }
    }
}
