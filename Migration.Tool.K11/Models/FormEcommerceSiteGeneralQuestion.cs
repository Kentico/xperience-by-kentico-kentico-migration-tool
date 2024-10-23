using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("Form_EcommerceSite_GeneralQuestion")]
public class FormEcommerceSiteGeneralQuestion
{
    [Key]
    [Column("GeneralQuestionID")]
    public int GeneralQuestionId { get; set; }

    [StringLength(150)]
    public string GeneralQuestionEmail { get; set; } = null!;

    public string GeneralQuestionText { get; set; } = null!;

    public DateTime FormInserted { get; set; }

    public DateTime FormUpdated { get; set; }
}
