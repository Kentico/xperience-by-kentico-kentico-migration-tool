using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("Form_EcommerceSite_ProductQuestion")]
public class FormEcommerceSiteProductQuestion
{
    [Key]
    [Column("ProductQuestionID")]
    public int ProductQuestionId { get; set; }

    [StringLength(150)]
    public string ProductQuestionEmail { get; set; } = null!;

    public string ProductQuestionText { get; set; } = null!;

    [StringLength(500)]
    public string ProductQuestionProduct { get; set; } = null!;

    public DateTime FormInserted { get; set; }

    public DateTime FormUpdated { get; set; }
}
