using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("custom_ONDREJV")]
    public partial class CustomOndrejv
    {
        [Key]
        [Column("ONDREJVID")]
        public int Ondrejvid { get; set; }
        [Column("jedna")]
        [StringLength(200)]
        public string Jedna { get; set; } = null!;
    }
}
