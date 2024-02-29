using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Form_DancingGoat_BusinessCustomerRegistration")]
public partial class FormDancingGoatBusinessCustomerRegistration
{
    [Key]
    [Column("BusinessCustomerRegistrationID")]
    public int BusinessCustomerRegistrationId { get; set; }

    [StringLength(200)]
    public string CompanyName { get; set; } = null!;

    [StringLength(200)]
    public string FirstName { get; set; } = null!;

    [StringLength(200)]
    public string LastName { get; set; } = null!;

    [StringLength(200)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string Email { get; set; } = null!;

    [StringLength(200)]
    public string BecomePartner { get; set; } = null!;

    [Precision(0)]
    public DateTime FormInserted { get; set; }

    [Precision(0)]
    public DateTime FormUpdated { get; set; }
}
