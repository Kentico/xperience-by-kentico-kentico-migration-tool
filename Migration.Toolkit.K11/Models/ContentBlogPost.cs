using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("CONTENT_BlogPost")]
public partial class ContentBlogPost
{
    [Key]
    [Column("BlogPostID")]
    public int BlogPostId { get; set; }

    [StringLength(200)]
    public string BlogPostTitle { get; set; } = null!;

    public DateTime BlogPostDate { get; set; }

    public string? BlogPostSummary { get; set; }

    public string BlogPostBody { get; set; } = null!;

    public Guid? BlogPostTeaser { get; set; }

    [Required]
    public bool? BlogPostAllowComments { get; set; }

    public bool? BlogLogActivity { get; set; }

    public string? FacebookAutoPost { get; set; }

    public string? TwitterAutoPost { get; set; }

    [StringLength(200)]
    public string? LinkedInAutoPost { get; set; }
}
