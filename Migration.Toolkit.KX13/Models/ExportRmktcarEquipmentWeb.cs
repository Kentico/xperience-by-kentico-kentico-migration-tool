using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("export_RMKTCarEquipment_web")]
    public partial class ExportRmktcarEquipmentWeb
    {
        [Key]
        [Column("CaseID")]
        public int CaseId { get; set; }
        [Key]
        [Column("DimCarOptionID")]
        public int DimCarOptionId { get; set; }
    }
}
