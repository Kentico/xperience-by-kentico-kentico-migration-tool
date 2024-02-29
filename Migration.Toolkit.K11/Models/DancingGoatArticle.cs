using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("DancingGoat_Article")]
public partial class DancingGoatArticle
{
    [Key]
    [Column("ArticleID")]
    public int ArticleId { get; set; }

    [StringLength(450)]
    public string ArticleTitle { get; set; } = null!;

    public Guid? ArticleTeaser { get; set; }

    [StringLength(190)]
    public string ArticleSummary { get; set; } = null!;

    public string ArticleText { get; set; } = null!;
}
