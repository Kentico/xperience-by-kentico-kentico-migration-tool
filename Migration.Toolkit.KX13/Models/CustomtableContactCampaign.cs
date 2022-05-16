using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("customtable_ContactCampaign")]
    public partial class CustomtableContactCampaign
    {
        [Key]
        [Column("ItemID")]
        public int ItemId { get; set; }
        [Column("ContactID")]
        public int ContactId { get; set; }
        [StringLength(200)]
        public string CampaignName { get; set; } = null!;
        public DateTime EventDate { get; set; }
        [Column("DocumentID")]
        public int? DocumentId { get; set; }
    }
}
