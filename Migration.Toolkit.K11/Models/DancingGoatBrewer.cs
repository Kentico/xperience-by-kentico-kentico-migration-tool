using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("DancingGoat_Brewer")]
public partial class DancingGoatBrewer
{
    [Key]
    [Column("BrewerID")]
    public int BrewerId { get; set; }

    [StringLength(200)]
    public string? BrewerPromotionTitle { get; set; }

    [StringLength(200)]
    public string? BrewerPromotionDescription { get; set; }

    [StringLength(200)]
    public string? BrewerBannerText { get; set; }
}