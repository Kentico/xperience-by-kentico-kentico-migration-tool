using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("DancingGoat_FilterPack")]
public partial class DancingGoatFilterPack
{
    [Key]
    [Column("FilterPackID")]
    public int FilterPackId { get; set; }

    public int FilterPackQuantity { get; set; }

    [StringLength(200)]
    public string? FilterPromotionTitle { get; set; }

    [StringLength(200)]
    public string? FilterPromotionDescription { get; set; }

    [StringLength(200)]
    public string? FilterBannerText { get; set; }
}