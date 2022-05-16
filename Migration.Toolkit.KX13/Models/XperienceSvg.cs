using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_SVGs")]
    public partial class XperienceSvg
    {
        [Key]
        [Column("ItemID")]
        public int ItemId { get; set; }
        public int? ItemModifiedBy { get; set; }
        public DateTime? ItemModifiedWhen { get; set; }
        public int? ItemOrder { get; set; }
        [Column("ItemGUID")]
        public Guid ItemGuid { get; set; }
        [Column("SVGName")]
        [StringLength(200)]
        public string Svgname { get; set; } = null!;
        [Column("SVGPath")]
        [StringLength(512)]
        public string Svgpath { get; set; } = null!;
        [Column("SVGWidth", TypeName = "decimal(19, 4)")]
        public decimal Svgwidth { get; set; }
        [Column("SVGHeight", TypeName = "decimal(19, 4)")]
        public decimal Svgheight { get; set; }
    }
}
