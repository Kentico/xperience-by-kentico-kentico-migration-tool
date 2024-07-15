using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("DancingGoat_ElectricGrinder")]
public partial class DancingGoatElectricGrinder
{
    [Key]
    [Column("ElectricGrinderID")]
    public int ElectricGrinderId { get; set; }

    public int ElectricGrinderPower { get; set; }

    [StringLength(200)]
    public string? ElectricGrinderPromotionTitle { get; set; }

    [StringLength(200)]
    public string? ElectricGrinderPromotionDescription { get; set; }

    [StringLength(200)]
    public string? ElectricGrinderBannerText { get; set; }
}