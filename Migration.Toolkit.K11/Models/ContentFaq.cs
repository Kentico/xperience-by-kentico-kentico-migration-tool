using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("CONTENT_FAQ")]
public class ContentFaq
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
