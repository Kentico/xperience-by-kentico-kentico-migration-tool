using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CONTENT_HeadlineBanner")]
public partial class ContentHeadlineBanner
{
    [Key]
    [Column("HeadlineBannerID")]
    public int HeadlineBannerId { get; set; }

    [StringLength(50)]
    public string HeadlineBannerDescription { get; set; } = null!;

    [StringLength(300)]
    public string? HeadlineBannerImage { get; set; }

    [StringLength(300)]
    public string HeadlineBannerDocumentUrl { get; set; } = null!;
}