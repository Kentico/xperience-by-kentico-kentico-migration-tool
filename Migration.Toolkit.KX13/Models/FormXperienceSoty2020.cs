using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_SOTY_2020")]
    public partial class FormXperienceSoty2020
    {
        [Key]
        [Column("SOTY_2020ID")]
        public int Soty2020id { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string WebsiteName { get; set; } = null!;
        [Column("URL")]
        [StringLength(500)]
        public string Url { get; set; } = null!;
        [StringLength(200)]
        public string CountrySelector { get; set; } = null!;
        [StringLength(500)]
        public string Implementation { get; set; } = null!;
        [StringLength(500)]
        public string EmailInput { get; set; } = null!;
        [StringLength(200)]
        public string VersionKentico { get; set; } = null!;
        [StringLength(200)]
        public string IndustryAwardCategory { get; set; } = null!;
        [StringLength(200)]
        public string? SpecialJuryAward { get; set; }
        public string ProjectReasoning { get; set; } = null!;
        public string ProjectDescription { get; set; } = null!;
        public string ProjectAchievements { get; set; } = null!;
        public string WhyKentico { get; set; } = null!;
        [StringLength(500)]
        public string NumberEditors { get; set; } = null!;
        [StringLength(500)]
        public string NumberOfPages { get; set; } = null!;
        [StringLength(500)]
        public string PageViewsNumber { get; set; } = null!;
        [StringLength(500)]
        public string? NumberVisits { get; set; }
        [StringLength(500)]
        public string Geolocation { get; set; } = null!;
        [StringLength(500)]
        public string SystemIntegration { get; set; } = null!;
        [StringLength(500)]
        public string IntegrationTools { get; set; } = null!;
        [StringLength(500)]
        public string ExtensiveIntegration { get; set; } = null!;
        [Column("CMSReplace")]
        [StringLength(500)]
        public string Cmsreplace { get; set; } = null!;
        [StringLength(500)]
        public string MigrationBenefit { get; set; } = null!;
        [Column("WhyCMSChange")]
        [StringLength(500)]
        public string? WhyCmschange { get; set; }
        [StringLength(500)]
        public string StructureChange { get; set; } = null!;
        public string MeasurableResults { get; set; } = null!;
        public string KenticoSuccess { get; set; } = null!;
        [StringLength(200)]
        public string MultipleChoice { get; set; } = null!;
        [StringLength(500)]
        public string MissingFeatures { get; set; } = null!;
        public string FuturePlans { get; set; } = null!;
        [StringLength(500)]
        public string SpecialGraphic { get; set; } = null!;
        [StringLength(500)]
        public string Inspiration { get; set; } = null!;
        [StringLength(500)]
        public string BaseDesignPrinciples { get; set; } = null!;
        [StringLength(500)]
        public string ResponsiveDesign { get; set; } = null!;
        [StringLength(500)]
        public string UniqueGraphic { get; set; } = null!;
        [StringLength(4)]
        public string? InvisibleRecaptchaV3 { get; set; }
    }
}
