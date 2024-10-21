using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("CONTENT_News")]
public class ContentNews
{
    [Key]
    [Column("NewsID")]
    public int NewsId { get; set; }

    [StringLength(450)]
    public string NewsTitle { get; set; } = null!;

    public DateTime NewsReleaseDate { get; set; }

    public string NewsSummary { get; set; } = null!;

    public string? NewsText { get; set; }

    public Guid? NewsTeaser { get; set; }
}
