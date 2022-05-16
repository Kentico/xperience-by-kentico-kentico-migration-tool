using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_Theme")]
    public partial class XperienceTheme
    {
        [Key]
        [Column("ItemID")]
        public int ItemId { get; set; }
        public int? ItemCreatedBy { get; set; }
        public int? ItemModifiedBy { get; set; }
        public DateTime? ItemModifiedWhen { get; set; }
        public int? ItemOrder { get; set; }
        [Column("ItemGUID")]
        public Guid ItemGuid { get; set; }
        [StringLength(200)]
        public string ThemeName { get; set; } = null!;
        [StringLength(10)]
        public string ThemePrimaryAccent { get; set; } = null!;
        [StringLength(10)]
        public string ThemeSecondaryAccent { get; set; } = null!;
        [StringLength(10)]
        public string ThemeTextAccent { get; set; } = null!;
    }
}
