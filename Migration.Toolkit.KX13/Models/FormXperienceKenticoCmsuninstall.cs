using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_Kentico_CMSUninstall")]
    public partial class FormXperienceKenticoCmsuninstall
    {
        [Key]
        [Column("Kentico_CMSUninstall_xpID")]
        public int KenticoCmsuninstallXpId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        public string YourComment { get; set; } = null!;
        [StringLength(500)]
        public string? FirstName { get; set; }
        [StringLength(500)]
        public string? LastName { get; set; }
        [StringLength(500)]
        public string? YourEmail { get; set; }
        [StringLength(500)]
        public string? YourPhone { get; set; }
        [StringLength(500)]
        public string? CookieUtmCampaign { get; set; }
        [StringLength(500)]
        public string? CookieUtmSource { get; set; }
        [StringLength(500)]
        public string? CookieUtmMedium { get; set; }
        public Guid? AutomaticFollowupConsent { get; set; }
        [StringLength(4)]
        public string? InvisibleRecaptchaV3 { get; set; }
    }
}
