using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_SubscriptionApproval")]
    public partial class XperienceSubscriptionApproval
    {
        [Key]
        [Column("SubscriptionApprovalID")]
        public int SubscriptionApprovalId { get; set; }
        [StringLength(200)]
        public string? Name { get; set; }
    }
}
