using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_RssFeed")]
    public partial class XperienceRssFeed
    {
        [Key]
        [Column("RssFeedID")]
        public int RssFeedId { get; set; }
        [StringLength(200)]
        public string? Name { get; set; }
    }
}
