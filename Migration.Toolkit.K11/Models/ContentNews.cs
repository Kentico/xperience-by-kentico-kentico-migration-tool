using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CONTENT_News")]
public partial class ContentNews
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