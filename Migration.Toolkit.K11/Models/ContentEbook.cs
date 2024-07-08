using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CONTENT_Ebook")]
public partial class ContentEbook
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