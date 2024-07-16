using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Keyless]
public class ViewCmsUserRoleMembershipRoleValidOnlyJoined
{
    [Column("UserID")]
    public int UserId { get; set; }

    [Column("RoleID")]
    public int RoleId { get; set; }

    public DateTime? ValidTo { get; set; }
}
