using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_NewsletterSubscriptionInternal")]
    public partial class FormXperienceNewsletterSubscriptionInternal
    {
        [Key]
        [Column("NewsletterSubscriptionInternalID")]
        public int NewsletterSubscriptionInternalId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string Email { get; set; } = null!;
        public bool? GeneralNewsletterSubscription { get; set; }
        public Guid? CustomConsentAgreement { get; set; }
    }
}
