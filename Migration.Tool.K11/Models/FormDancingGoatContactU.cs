using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("Form_DancingGoat_ContactUs")]
public class FormDancingGoatContactU
{
    [Key]
    [Column("ContactUsID")]
    public int ContactUsId { get; set; }

    [StringLength(200)]
    public string? UserFirstName { get; set; }

    [StringLength(200)]
    public string? UserLastName { get; set; }

    [StringLength(100)]
    public string UserEmail { get; set; } = null!;

    [StringLength(500)]
    public string UserMessage { get; set; } = null!;

    [Precision(0)]
    public DateTime FormInserted { get; set; }

    [Precision(0)]
    public DateTime FormUpdated { get; set; }
}
