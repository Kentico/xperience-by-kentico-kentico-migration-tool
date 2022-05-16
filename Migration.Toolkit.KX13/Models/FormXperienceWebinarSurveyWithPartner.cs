using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_Webinar_Survey_withPartners")]
    public partial class FormXperienceWebinarSurveyWithPartner
    {
        [Key]
        [Column("Webinar_Survey_with_partnersID")]
        public int WebinarSurveyWithPartnersId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string? TextInput { get; set; }
        [Column("TextInput_1")]
        [StringLength(500)]
        public string? TextInput1 { get; set; }
        [StringLength(200)]
        public string RadioButtons { get; set; } = null!;
        [Column("RadioButtons_1")]
        [StringLength(200)]
        public string RadioButtons1 { get; set; } = null!;
        [Column("RadioButtons_2")]
        [StringLength(200)]
        public string RadioButtons2 { get; set; } = null!;
        public string? TextArea { get; set; }
        [StringLength(200)]
        public string? DropDown { get; set; }
        [StringLength(4)]
        public string? InvisibleRecaptchaV3 { get; set; }
    }
}
