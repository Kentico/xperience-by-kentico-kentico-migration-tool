using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("CONTENT_Ebook")]
public class ContentEbook
{
    [Key]
    [Column("EbookID")]
    public int EbookId { get; set; }

    [StringLength(100)]
    public string? BookAuthor { get; set; }

    [StringLength(100)]
    public string? BookPublisher { get; set; }

    [StringLength(100)]
    public string? BookFormat { get; set; }
}
