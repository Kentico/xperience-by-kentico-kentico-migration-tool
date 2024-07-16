using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("STORECONTENT_Book")]
public class StorecontentBook
{
    [Key]
    [Column("BookID")]
    public int BookId { get; set; }

    [StringLength(100)]
    public string? BookAuthor { get; set; }

    public DateTime? BookPublicationDate { get; set; }

    [Column("BookISBN")]
    [StringLength(100)]
    public string? BookIsbn { get; set; }

    public int? BookEdition { get; set; }
}
