using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("NetC_MenuItem")]
    public partial class NetCMenuItem
    {
        [Key]
        [Column("MenuItemID")]
        public int MenuItemId { get; set; }
        public Guid MenuItemGuid { get; set; }
        public DateTime MenuItemLastModified { get; set; }
        [Column("MenuItemMenuID")]
        public int MenuItemMenuId { get; set; }
        public int MenuItemOrder { get; set; }
        [StringLength(200)]
        public string MenuItemDisplayName { get; set; } = null!;
        [StringLength(512)]
        public string? MenuItemImage { get; set; }
        [StringLength(512)]
        public string? MenuItemContent { get; set; }
        [StringLength(20)]
        public string? MenuItemTheme { get; set; }
        [StringLength(200)]
        public string MenuItemUrl { get; set; } = null!;
        [StringLength(20)]
        public string? MenuItemTarget { get; set; }
        [StringLength(200)]
        public string? MenuItemLinkText { get; set; }
    }
}
