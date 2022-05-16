using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models
{
    [Table("export_RMKTCarImage_web")]
    public partial class ExportRmktcarImageWeb
    {
        [Key]
        [Column("FactAssetOfferImageID")]
        public int FactAssetOfferImageId { get; set; }
        [Column("CaseID")]
        public int CaseId { get; set; }
        public int ImgOrder { get; set; }
        [StringLength(255)]
        [Unicode(false)]
        public string ImgPath { get; set; } = null!;
    }
}
