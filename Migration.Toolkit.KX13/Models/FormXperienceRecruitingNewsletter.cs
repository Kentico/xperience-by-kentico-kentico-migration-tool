using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_RecruitingNewsletter")]
    public partial class FormXperienceRecruitingNewsletter
    {
        [Key]
        [Column("RecruitingNewsletterID")]
        public int RecruitingNewsletterId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [Column("DropDown_marketingplatform_CMS")]
        [StringLength(200)]
        public string DropDownMarketingplatformCms { get; set; } = null!;
        [Column("DropDown_lengthofusage")]
        [StringLength(200)]
        public string DropDownLengthofusage { get; set; } = null!;
        [Column("TextInput_specifyotherCMS")]
        [StringLength(500)]
        public string TextInputSpecifyotherCms { get; set; } = null!;
        [Column("DropDown_newsolution")]
        [StringLength(200)]
        public string DropDownNewsolution { get; set; } = null!;
        [Column("TextInput_specify4a")]
        [StringLength(500)]
        public string TextInputSpecify4a { get; set; } = null!;
        [Column("TextInput_whoisinvolved")]
        [StringLength(500)]
        public string TextInputWhoisinvolved { get; set; } = null!;
        [Column("TextInput_whowillbeusing")]
        [StringLength(500)]
        public string TextInputWhowillbeusing { get; set; } = null!;
        [Column("DropDown_implementationA")]
        [StringLength(200)]
        public string DropDownImplementationA { get; set; } = null!;
        [Column("TextInput_specifyimplementation")]
        [StringLength(500)]
        public string TextInputSpecifyimplementation { get; set; } = null!;
        [Column("TextInput_consideredother")]
        [StringLength(500)]
        public string TextInputConsideredother { get; set; } = null!;
        [Column("TextInput_whoinvolved")]
        [StringLength(500)]
        public string TextInputWhoinvolved { get; set; } = null!;
        [Column("TextInput_whousing")]
        [StringLength(500)]
        public string TextInputWhousing { get; set; } = null!;
        [Column("DropDown_whoimplemented")]
        [StringLength(200)]
        public string DropDownWhoimplemented { get; set; } = null!;
        [Column("TextInput_whoimplemented")]
        [StringLength(500)]
        public string TextInputWhoimplemented { get; set; } = null!;
        [Column("TextInput_industry")]
        [StringLength(500)]
        public string TextInputIndustry { get; set; } = null!;
        [Column("TextInput_job")]
        [StringLength(500)]
        public string TextInputJob { get; set; } = null!;
        [Column("TextInput_organization")]
        [StringLength(500)]
        public string TextInputOrganization { get; set; } = null!;
        [Column("TextArea_anythingelse")]
        public string? TextAreaAnythingelse { get; set; }
        [Column("RadioButtons_interrestedyesno")]
        [StringLength(200)]
        public string RadioButtonsInterrestedyesno { get; set; } = null!;
        [Column("EmailInput_youremail")]
        [StringLength(500)]
        public string EmailInputYouremail { get; set; } = null!;
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
        [StringLength(500)]
        public string? Validation { get; set; }
        [Column("MultipleChoice_whichconsidered")]
        [StringLength(200)]
        public string MultipleChoiceWhichconsidered { get; set; } = null!;
        [Column("MultipleChoice_whichA")]
        [StringLength(200)]
        public string MultipleChoiceWhichA { get; set; } = null!;
        [Column("MultipleChoice_whowillbeusingA")]
        [StringLength(200)]
        public string MultipleChoiceWhowillbeusingA { get; set; } = null!;
        [Column("MultipleChoice_whoisinvolvedA")]
        [StringLength(200)]
        public string MultipleChoiceWhoisinvolvedA { get; set; } = null!;
        [Column("MultipleChoice_whoinvolved")]
        [StringLength(200)]
        public string MultipleChoiceWhoinvolved { get; set; } = null!;
        [Column("MultipleChoice_whoisusing")]
        [StringLength(200)]
        public string MultipleChoiceWhoisusing { get; set; } = null!;
        [StringLength(200)]
        public string? TextBlock { get; set; }
        [Column("TextBlock_1")]
        [StringLength(200)]
        public string? TextBlock1 { get; set; }
    }
}
