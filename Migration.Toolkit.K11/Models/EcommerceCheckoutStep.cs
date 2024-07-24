using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("Ecommerce_CheckoutStep")]
public class EcommerceCheckoutStep
{
    [Key]
    [Column("CheckoutStepID")]
    public int CheckoutStepId { get; set; }

    [StringLength(1000)]
    public string StepName { get; set; } = null!;
}
