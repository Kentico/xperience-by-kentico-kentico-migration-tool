using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[Keyless]
public class ViewCmsUserRoleMembershipRole
{
    [Column("RoleID")]
    public int RoleId { get; set; }

    [StringLength(100)]
    public string RoleName { get; set; } = null!;

    [Column("SiteID")]
    public int? SiteId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    public DateTime? ValidTo { get; set; }
}
