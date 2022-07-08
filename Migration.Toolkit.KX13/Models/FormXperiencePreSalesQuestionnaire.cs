using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_Pre_salesQuestionnaire")]
    public partial class FormXperiencePreSalesQuestionnaire
    {
        [Key]
        [Column("Pre_salesQuestionnaireID")]
        public int PreSalesQuestionnaireId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string EmailInput { get; set; } = null!;
        public string WhatAspectYouLike { get; set; } = null!;
        public string? UpdateImprove { get; set; }
        [StringLength(500)]
        public string CurrentPlatform { get; set; } = null!;
        public string Details { get; set; } = null!;
        [StringLength(200)]
        public string DropDown { get; set; } = null!;
        [StringLength(200)]
        public string Workflow { get; set; } = null!;
        [StringLength(200)]
        public string Localization { get; set; } = null!;
        [StringLength(200)]
        public string TranslationInHouse { get; set; } = null!;
        [StringLength(500)]
        public string WhatTranslatinServices { get; set; } = null!;
        [StringLength(200)]
        public string SecureContent { get; set; } = null!;
        [StringLength(200)]
        public string RdPartyApps { get; set; } = null!;
        [StringLength(200)]
        public string DigitalEngagementStrategy { get; set; } = null!;
        public string SpecifyDigitalStrategy { get; set; } = null!;
        [StringLength(200)]
        public string CriticalCapabilities { get; set; } = null!;
        [StringLength(200)]
        public string Redesign { get; set; } = null!;
        public string SiteExamples { get; set; } = null!;
        [StringLength(200)]
        public string InternalDevelopers { get; set; } = null!;
        [StringLength(200)]
        public string AgencyOfRecord { get; set; } = null!;
        [StringLength(200)]
        public string AgencyRecommendation { get; set; } = null!;
        [StringLength(500)]
        public string SpecifyEnvironment { get; set; } = null!;
        public string ParticipatingInEvaluation { get; set; } = null!;
        [StringLength(200)]
        public string ExecutiveSponsor { get; set; } = null!;
        [StringLength(500)]
        public string NameOfSponsor { get; set; } = null!;
        [StringLength(500)]
        public string EmailOfSponsor { get; set; } = null!;
        [Column("DropDown_1")]
        [StringLength(200)]
        public string DropDown1 { get; set; } = null!;
        [StringLength(500)]
        public string AgencySpecify { get; set; } = null!;
        [Column("DropDown_2")]
        [StringLength(200)]
        public string DropDown2 { get; set; } = null!;
        public string Specify3rdParty { get; set; } = null!;
        [StringLength(500)]
        public string SpecifyObjectives { get; set; } = null!;
        [StringLength(500)]
        public string SpecifyImportantAspect { get; set; } = null!;
        [StringLength(200)]
        public string? HowDidYouHear { get; set; }
        [Column("AspectOfDXP")]
        [StringLength(200)]
        public string AspectOfDxp { get; set; } = null!;
        [StringLength(200)]
        public string KeyObjectives { get; set; } = null!;
        [StringLength(200)]
        public string? EnvironmentForHosting { get; set; }
    }
}
