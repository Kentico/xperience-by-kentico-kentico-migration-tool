using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_WebinarRegistration_LeadGen_EmailResponseOnly")]
    public partial class FormXperienceWebinarRegistrationLeadGenEmailResponseOnly
    {
        [Key]
        [Column("WebinarRegistration_LeadGen_EmailResponseOnlyID")]
        public int WebinarRegistrationLeadGenEmailResponseOnlyId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string FirstName { get; set; } = null!;
        [StringLength(500)]
        public string LastName { get; set; } = null!;
        [StringLength(500)]
        public string? Company { get; set; }
        [StringLength(500)]
        public string? JobTitle { get; set; }
        public Guid? CustomConsentAgreement { get; set; }
        [StringLength(500)]
        public string? CookieUtmCampaign { get; set; }
        [StringLength(500)]
        public string? CookieUtmSource { get; set; }
        [StringLength(500)]
        public string? CookieUtmMedium { get; set; }
        [Column("PageURL")]
        [StringLength(200)]
        public string? PageUrl { get; set; }
        [StringLength(500)]
        public string Email { get; set; } = null!;
        [Column("FormContactGUID")]
        public Guid? FormContactGuid { get; set; }
    }
}
