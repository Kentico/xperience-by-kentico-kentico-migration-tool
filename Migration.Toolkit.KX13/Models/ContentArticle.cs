using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("CONTENT_Article")]
    public partial class ContentArticle
    {
        [Key]
        [Column("ArticleID")]
        public int ArticleId { get; set; }
        [StringLength(450)]
        public string ArticleName { get; set; } = null!;
        public string? ArticleTeaserText { get; set; }
        public Guid? ArticleTeaserImage { get; set; }
        public string? ArticleText { get; set; }
    }
}
