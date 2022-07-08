using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_BBB_DigitalTrendsInGovernment2022")]
    public partial class FormXperienceBbbDigitalTrendsInGovernment2022
    {
        [Key]
        [Column("BBB_DigitalTrendsInGovernment2022ID")]
        public int BbbDigitalTrendsInGovernment2022Id { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string TextInput { get; set; } = null!;
        [Column("TextInput_1")]
        [StringLength(500)]
        public string TextInput1 { get; set; } = null!;
        [StringLength(500)]
        public string EmailInput { get; set; } = null!;
        [StringLength(500)]
        public string? CookieUtmCampaign { get; set; }
        [StringLength(500)]
        public string? CookieUtmSource { get; set; }
        [StringLength(500)]
        public string? CookieUtmMedium { get; set; }
        [Column("FormGUID")]
        [StringLength(500)]
        public string? FormGuid { get; set; }
        [Column("PageURL")]
        [StringLength(200)]
        public string? PageUrl { get; set; }
        [StringLength(4)]
        public string? InvisibleRecaptchaV3 { get; set; }
        public Guid? CustomConsentAgreement { get; set; }
        public string? MultipleChoice { get; set; }
        [StringLength(200)]
        public string? TextBlock { get; set; }
        [Column("TextInput_4")]
        [StringLength(500)]
        public string? TextInput4 { get; set; }
        [Column("TextInput_5")]
        [StringLength(500)]
        public string TextInput5 { get; set; } = null!;
        [Column("TextInput_2")]
        [StringLength(500)]
        public string? TextInput2 { get; set; }
    }
}
