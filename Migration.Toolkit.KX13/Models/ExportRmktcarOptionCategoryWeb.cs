using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models
{
    [Table("export_RMKTCarOptionCategory_web")]
    public partial class ExportRmktcarOptionCategoryWeb
    {
        [Key]
        [Column("DimCarOptionCategoryID")]
        public int DimCarOptionCategoryId { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string CarOptionCategoryName { get; set; } = null!;
    }
}
