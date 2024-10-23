using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("CONTENT_KBArticle")]
public class ContentKbarticle
{
    [Key]
    [Column("KBArticleID")]
    public int KbarticleId { get; set; }

    [StringLength(200)]
    public string? ArticleIdentifier { get; set; }

    [StringLength(400)]
    public string ArticleName { get; set; } = null!;

    public string ArticleSummary { get; set; } = null!;

    public string ArticleAppliesTo { get; set; } = null!;

    public string ArticleText { get; set; } = null!;

    public string? ArticleSeeAlso { get; set; }
}
