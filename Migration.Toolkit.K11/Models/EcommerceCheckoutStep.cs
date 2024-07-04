using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Ecommerce_CheckoutStep")]
public partial class EcommerceCheckoutStep
{
    [Key]
    [Column("CheckoutStepID")]
    public int CheckoutStepId { get; set; }

    [StringLength(1000)]
    public string StepName { get; set; } = null!;
}