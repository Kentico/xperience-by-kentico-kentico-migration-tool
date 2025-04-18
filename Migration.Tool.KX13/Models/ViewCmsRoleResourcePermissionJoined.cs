using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX13.Models;

[Keyless]
public class ViewCmsRoleResourcePermissionJoined
{
    [Column("RoleID")]
    public int RoleId { get; set; }

    [StringLength(100)]
    public string ResourceName { get; set; } = null!;

    [StringLength(100)]
    public string PermissionName { get; set; } = null!;

    [Column("PermissionID")]
    public int PermissionId { get; set; }
}
