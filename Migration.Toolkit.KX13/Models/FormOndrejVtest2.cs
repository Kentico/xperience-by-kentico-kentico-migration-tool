using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("form_OndrejVTEST2")]
    public partial class FormOndrejVtest2
    {
        [Key]
        [Column("OndrejVTEST2ID")]
        public int OndrejVtest2id { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(200)]
        public string? Phone { get; set; }
        [StringLength(200)]
        public string Country { get; set; } = null!;
    }
}
