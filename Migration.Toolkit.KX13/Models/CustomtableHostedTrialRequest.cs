using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("customtable_HostedTrialRequests")]
    public partial class CustomtableHostedTrialRequest
    {
        [Key]
        [Column("ItemID")]
        public int ItemId { get; set; }
        public DateTime? ItemCreatedWhen { get; set; }
        public DateTime? ItemModifiedWhen { get; set; }
        [Column("ItemGUID")]
        public Guid ItemGuid { get; set; }
        public int? UserId { get; set; }
        [StringLength(200)]
        public string? UserEmail { get; set; }
        [StringLength(200)]
        public string? FirstName { get; set; }
        [StringLength(200)]
        public string? LastName { get; set; }
        [StringLength(200)]
        public string? Company { get; set; }
        [StringLength(200)]
        public string? Country { get; set; }
        [StringLength(200)]
        public string? State { get; set; }
        [StringLength(200)]
        public string? Phone { get; set; }
        [StringLength(200)]
        public string? Campaign { get; set; }
        public Guid? RequestGuid { get; set; }
        public bool RequestConfirmed { get; set; }
        [StringLength(100)]
        public string? Server { get; set; }
        public bool? WebExtended { get; set; }
        public Guid? CustomerOfPartner { get; set; }
        [StringLength(200)]
        public string? City { get; set; }
        public Guid? RequestPartnerGuid { get; set; }
        [Column("AreYouLookingForCMS")]
        [StringLength(10)]
        public string? AreYouLookingForCms { get; set; }
        [StringLength(50)]
        public string? Role { get; set; }
        [Column("ImportedToCRM")]
        public bool? ImportedToCrm { get; set; }
        public bool? ConsentPartner { get; set; }
        public bool? ConsentKentico { get; set; }
    }
}
