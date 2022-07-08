using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("customtable_MailingRegion")]
    public partial class CustomtableMailingRegion
    {
        [Key]
        [Column("ItemID")]
        public int ItemId { get; set; }
        [Column("ItemGUID")]
        public Guid ItemGuid { get; set; }
        [StringLength(50)]
        public string? MailingRegionDisplayName { get; set; }
        [StringLength(50)]
        public string? MailingRegionCodeName { get; set; }
    }
}
