using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Form_DancingGoat_TryFreeSample")]
public partial class FormDancingGoatTryFreeSample
{
    [Key]
    [Column("TryAFreeSampleID")]
    public int TryAfreeSampleId { get; set; }

    [Precision(0)]
    public DateTime FormInserted { get; set; }

    [Precision(0)]
    public DateTime FormUpdated { get; set; }

    [StringLength(500)]
    public string FirstName { get; set; } = null!;

    [StringLength(500)]
    public string LastName { get; set; } = null!;

    [StringLength(500)]
    public string EmailAddress { get; set; } = null!;

    [StringLength(500)]
    public string Address { get; set; } = null!;

    [StringLength(500)]
    public string City { get; set; } = null!;

    [Column("ZIPCode")]
    [StringLength(500)]
    public string Zipcode { get; set; } = null!;

    [StringLength(3)]
    public string Country { get; set; } = null!;

    [StringLength(500)]
    public string? State { get; set; }
}