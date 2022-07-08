using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("customtable_MailingRegionCountry")]
    public partial class CustomtableMailingRegionCountry
    {
        [Key]
        [Column("ItemID")]
        public int ItemId { get; set; }
        [Column("ItemGUID")]
        public Guid ItemGuid { get; set; }
        [Column("MailingRegionID")]
        public int? MailingRegionId { get; set; }
        [Column("CountryID")]
        public int? CountryId { get; set; }
    }
}
