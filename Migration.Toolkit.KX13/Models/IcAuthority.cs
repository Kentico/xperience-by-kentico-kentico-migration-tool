using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models
{
    [Keyless]
    [Table("_IcAuthorities")]
    public partial class IcAuthority
    {
        [Column("typ")]
        [StringLength(50)]
        public string Typ { get; set; } = null!;
        [Column("nazev")]
        [StringLength(50)]
        public string Nazev { get; set; } = null!;
        [Column("ulice")]
        [StringLength(100)]
        public string Ulice { get; set; } = null!;
        [Column("psč")]
        [StringLength(50)]
        public string Psč { get; set; } = null!;
        [Column("kraj")]
        [StringLength(50)]
        public string Kraj { get; set; } = null!;
    }
}
