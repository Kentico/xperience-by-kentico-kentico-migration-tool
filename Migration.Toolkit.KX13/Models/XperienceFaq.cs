using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_FAQ")]
    public partial class XperienceFaq
    {
        [Key]
        [Column("FAQID")]
        public int Faqid { get; set; }
        [StringLength(512)]
        public string Question { get; set; } = null!;
        public string Answer { get; set; } = null!;
    }
}
