using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CONTENT_Office")]
public partial class ContentOffice
{
    [Key]
    [Column("OfficeID")]
    public int OfficeId { get; set; }

    [StringLength(400)]
    public string OfficeName { get; set; } = null!;

    [StringLength(200)]
    public string? OfficeCompanyName { get; set; }

    [StringLength(400)]
    public string? OfficeAddress1 { get; set; }

    [StringLength(400)]
    public string? OfficeAddress2 { get; set; }

    [StringLength(400)]
    public string? OfficeCity { get; set; }

    [Column("OfficeZIP")]
    [StringLength(50)]
    public string? OfficeZip { get; set; }

    [StringLength(200)]
    public string? OfficeState { get; set; }

    [StringLength(200)]
    public string? OfficeCountry { get; set; }

    [StringLength(100)]
    public string? OfficePhone { get; set; }

    [StringLength(254)]
    public string? OfficeEmail { get; set; }

    public string? OfficeDirections { get; set; }

    public double? OfficeLatitude { get; set; }

    public double? OfficeLongitude { get; set; }

    public Guid? OfficePhoto { get; set; }

    public string? OfficeDescription { get; set; }

    public bool? OfficeIsHeadquarters { get; set; }

    [Column("OfficeIconURL")]
    [StringLength(200)]
    public string? OfficeIconUrl { get; set; }
}