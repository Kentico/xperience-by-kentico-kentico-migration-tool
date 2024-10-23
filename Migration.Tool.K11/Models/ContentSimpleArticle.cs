using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("CONTENT_SimpleArticle")]
public class ContentSimpleArticle
{
    [Key]
    [Column("ArticleID")]
    public int ArticleId { get; set; }

    [StringLength(400)]
    public string ArticleTitle { get; set; } = null!;

    public string ArticleText { get; set; } = null!;
}
