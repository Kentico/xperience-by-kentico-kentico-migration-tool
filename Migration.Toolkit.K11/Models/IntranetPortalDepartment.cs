using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("IntranetPortal_Department")]
public partial class IntranetPortalDepartment
{
    [Key]
    [Column("DepartmentID")]
    public int DepartmentId { get; set; }

    [StringLength(200)]
    public string DepartmentName { get; set; } = null!;

    [StringLength(450)]
    public string? DepartmentDescription { get; set; }

    public Guid? DepartmentAvatar { get; set; }

    [StringLength(450)]
    public string? DepartmentSections { get; set; }

    [StringLength(100)]
    public string? DepartmentRoles { get; set; }
}
