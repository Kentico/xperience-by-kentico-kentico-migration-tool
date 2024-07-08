using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("DancingGoat_Tableware")]
public partial class DancingGoatTableware
{
    [Key]
    [Column("TablewareID")]
    public int TablewareId { get; set; }

    [StringLength(200)]
    public string? TablewarePromotionTitle { get; set; }

    [StringLength(200)]
    public string? TablewarePromotionDescription { get; set; }

    [StringLength(200)]
    public string? TablewareBannerText { get; set; }
}