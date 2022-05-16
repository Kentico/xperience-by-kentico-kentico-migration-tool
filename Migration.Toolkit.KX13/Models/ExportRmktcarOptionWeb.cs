using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models
{
    [Table("export_RMKTCarOption_web")]
    public partial class ExportRmktcarOptionWeb
    {
        [Key]
        [Column("DimCarOptionID")]
        public int DimCarOptionId { get; set; }
        [Column("DimCarOptionCategoryID")]
        public int DimCarOptionCategoryId { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string CarOptionName { get; set; } = null!;
    }
}
