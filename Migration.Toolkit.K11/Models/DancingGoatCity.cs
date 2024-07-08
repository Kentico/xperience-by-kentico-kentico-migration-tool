using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("DancingGoat_City")]
public partial class DancingGoatCity
{
    [Key]
    [Column("CityID")]
    public int CityId { get; set; }

    [StringLength(50)]
    public string CityName { get; set; } = null!;

    [StringLength(100)]
    public string? CityCountry { get; set; }
}