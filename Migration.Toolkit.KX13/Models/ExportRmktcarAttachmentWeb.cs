using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models
{
    [Table("export_RMKTCarAttachment_web")]
    public partial class ExportRmktcarAttachmentWeb
    {
        [Column("CaseID")]
        public int CaseId { get; set; }
        [Column("fileID")]
        public int FileId { get; set; }
        [Column("filePath")]
        [StringLength(250)]
        [Unicode(false)]
        public string FilePath { get; set; } = null!;
        [Key]
        [Column("FactAssetOfferAttachmentID")]
        public int FactAssetOfferAttachmentId { get; set; }
    }
}
