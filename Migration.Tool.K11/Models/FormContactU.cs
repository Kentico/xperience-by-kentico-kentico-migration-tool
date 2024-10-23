using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("Form_ContactUs")]
public class FormContactU
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
