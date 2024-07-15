using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CONTENT_KBArticle")]
public partial class ContentKbarticle
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