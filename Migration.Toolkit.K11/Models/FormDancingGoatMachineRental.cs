using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Form_DancingGoat_MachineRental")]
public partial class FormDancingGoatMachineRental
{
    [Key]
    [Column("MachineRentalID")]
    public int MachineRentalId { get; set; }

    [StringLength(200)]
    public string? Email { get; set; }

    [StringLength(200)]
    public string Machine { get; set; } = null!;

    [StringLength(200)]
    public string RentalPeriod { get; set; } = null!;

    [StringLength(200)]
    public string? Details { get; set; }

    [Precision(0)]
    public DateTime FormInserted { get; set; }

    [Precision(0)]
    public DateTime FormUpdated { get; set; }
}
