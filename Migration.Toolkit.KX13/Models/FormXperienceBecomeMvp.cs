using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_Become_MVP")]
    public partial class FormXperienceBecomeMvp
    {
        [Key]
        [Column("Become_MVPID")]
        public int BecomeMvpid { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string TextInput { get; set; } = null!;
        [StringLength(500)]
        public string EmailInput { get; set; } = null!;
        [Column("YourbBogURL")]
        [StringLength(500)]
        public string? YourbBogUrl { get; set; }
        [StringLength(500)]
        public string? Twitter { get; set; }
        public string? RecentSpeakingEngagements { get; set; }
        public string? OtherActivitiesYouWouldLikeUsToConsider { get; set; }
        public string? FocusAreas { get; set; }
        public Guid? CustomConsentAgreement { get; set; }
        [StringLength(4)]
        public string? InvisibleRecaptchaV3 { get; set; }
    }
}
