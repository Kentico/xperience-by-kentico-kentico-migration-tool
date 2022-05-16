using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_FolderURL")]
    public partial class XperienceFolderUrl
    {
        [Key]
        [Column("FolderURLID")]
        public int FolderUrlid { get; set; }
        [StringLength(200)]
        public string Name { get; set; } = null!;
    }
}
