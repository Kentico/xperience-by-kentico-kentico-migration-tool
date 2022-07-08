using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_Logo")]
    public partial class XperienceLogo
    {
        [Key]
        [Column("LogoID")]
        public int LogoId { get; set; }
        [StringLength(512)]
        public string LogoImageUrl { get; set; } = null!;
        [StringLength(200)]
        public string LogoTitle { get; set; } = null!;
        [StringLength(512)]
        public string? LogoSub { get; set; }
        [StringLength(512)]
        public string? LogoUrl { get; set; }
        [StringLength(20)]
        public string? LogoTarget { get; set; }
    }
}
