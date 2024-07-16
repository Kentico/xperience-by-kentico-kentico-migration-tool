using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KXP.Models;

[Table("CMS_Role")]
public class CmsRole
{
    [Key]
    [Column("RoleID")]
    public int RoleId { get; set; }

    [StringLength(100)]
    public string RoleDisplayName { get; set; } = null!;

    [StringLength(100)]
    public string RoleName { get; set; } = null!;

    public string? RoleDescription { get; set; }

    [Column("RoleGUID")]
    public Guid RoleGuid { get; set; }

    public DateTime RoleLastModified { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<CmsApplicationPermission> CmsApplicationPermissions { get; set; } = new List<CmsApplicationPermission>();

    [InverseProperty("ContentWorkflowStepRoleRole")]
    public virtual ICollection<CmsContentWorkflowStepRole> CmsContentWorkflowStepRoles { get; set; } = new List<CmsContentWorkflowStepRole>();

    [InverseProperty("Role")]
    public virtual ICollection<CmsUserRole> CmsUserRoles { get; set; } = new List<CmsUserRole>();

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<CmsForm> Forms { get; set; } = new List<CmsForm>();
}
