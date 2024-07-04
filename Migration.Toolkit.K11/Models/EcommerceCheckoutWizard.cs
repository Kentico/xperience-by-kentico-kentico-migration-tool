using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Ecommerce_CheckoutWizard")]
public partial class EcommerceCheckoutWizard
{
    [Key]
    [Column("CheckoutWizardID")]
    public int CheckoutWizardId { get; set; }

    [StringLength(1000)]
    public string? WizardName { get; set; }
}