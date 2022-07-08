using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("CONTENT_FAQ")]
    public partial class ContentFaq
    {
        [Key]
        [Column("FAQID")]
        public int Faqid { get; set; }
        [Column("FAQQuestion")]
        [StringLength(450)]
        public string Faqquestion { get; set; } = null!;
        [Column("FAQAnswer")]
        public string Faqanswer { get; set; } = null!;
    }
}
