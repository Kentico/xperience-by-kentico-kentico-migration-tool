using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("IntranetPortal_WorkingEnvironment")]
public partial class IntranetPortalWorkingEnvironment
{
    [Key]
    [Column("workingenvironmentID")]
    public int WorkingenvironmentId { get; set; }

    public DateTime FormInserted { get; set; }

    public DateTime FormUpdated { get; set; }

    [StringLength(200)]
    public string? YourName { get; set; }

    [StringLength(500)]
    public string OfficeSatisfaction { get; set; } = null!;

    public string? Suggestions { get; set; }
}
