using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("DancingGoat_Ebook")]
public class DancingGoatEbook
{
    [Key]
    [Column("EbookID")]
    public int EbookId { get; set; }

    [StringLength(200)]
    public string? EbookAuthor { get; set; }

    [Column("EbookISBN")]
    [StringLength(200)]
    public string? EbookIsbn { get; set; }
}
