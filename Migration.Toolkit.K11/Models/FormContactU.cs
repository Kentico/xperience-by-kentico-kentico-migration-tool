using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Form_ContactUs")]
public partial class FormContactU
{
    [Key]
    [Column("ContactUsID")]
    public int ContactUsId { get; set; }

    public DateTime FormInserted { get; set; }

    public DateTime FormUpdated { get; set; }

    [StringLength(200)]
    public string FirstName { get; set; } = null!;

    [StringLength(200)]
    public string LastName { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(14)]
    public string PhoneNumber { get; set; } = null!;

    public string Message { get; set; } = null!;
}