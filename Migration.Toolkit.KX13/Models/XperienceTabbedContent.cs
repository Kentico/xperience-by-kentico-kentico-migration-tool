using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_TabbedContent")]
    public partial class XperienceTabbedContent
    {
        [Key]
        [Column("TabbedContentID")]
        public int TabbedContentId { get; set; }
        [StringLength(200)]
        public string Heading { get; set; } = null!;
        [StringLength(200)]
        public string Icon { get; set; } = null!;
    }
}
