using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("CONTENT_MenuItem")]
    public partial class ContentMenuItem
    {
        [Key]
        [Column("MenuItemID")]
        public int MenuItemId { get; set; }
        [StringLength(450)]
        public string MenuItemName { get; set; } = null!;
        public Guid? MenuItemTeaserImage { get; set; }
        [StringLength(100)]
        public string? MenuItemGroup { get; set; }
    }
}
