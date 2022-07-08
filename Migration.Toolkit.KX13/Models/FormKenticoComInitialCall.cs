using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_KenticoCom_InitialCall")]
    public partial class FormKenticoComInitialCall
    {
        [Key]
        [Column("InitialCallID")]
        public int InitialCallId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string FirstName { get; set; } = null!;
        [StringLength(500)]
        public string LastName { get; set; } = null!;
        [StringLength(500)]
        public string Company { get; set; } = null!;
        public Guid? CustomConsentAgreement { get; set; }
        [StringLength(500)]
        public string EmailAddress { get; set; } = null!;
        [StringLength(200)]
        public string? Phone { get; set; }
        [StringLength(200)]
        public string Country { get; set; } = null!;
        public bool? GeneralNewsletterSubscription { get; set; }
        public Guid? AutomaticFollowupConsent { get; set; }
        [StringLength(500)]
        public string? PageUrl { get; set; }
    }
}
