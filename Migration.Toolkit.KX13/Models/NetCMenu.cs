using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("NetC_Menu")]
    public partial class NetCMenu
    {
        [Key]
        [Column("MenuID")]
        public int MenuId { get; set; }
        public Guid MenuGuid { get; set; }
        public DateTime MenuLastModified { get; set; }
        [Column("MenuSiteID")]
        public int MenuSiteId { get; set; }
        [StringLength(200)]
        public string MenuDisplayName { get; set; } = null!;
        [StringLength(200)]
        public string MenuName { get; set; } = null!;
    }
}
