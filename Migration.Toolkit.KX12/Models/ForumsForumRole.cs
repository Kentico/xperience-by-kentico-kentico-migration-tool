using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[PrimaryKey("ForumId", "RoleId", "PermissionId")]
[Table("Forums_ForumRoles")]
[Index("PermissionId", Name = "IX_Forums_ForumRoles_PermissionID")]
[Index("RoleId", Name = "IX_Forums_ForumRoles_RoleID")]
public partial class ForumsForumRole
{
    [Key]
    [Column("ForumID")]
    public int ForumId { get; set; }

    [Key]
    [Column("RoleID")]
    public int RoleId { get; set; }

    [Key]
    [Column("PermissionID")]
    public int PermissionId { get; set; }

    [ForeignKey("ForumId")]
    [InverseProperty("ForumsForumRoles")]
    public virtual ForumsForum Forum { get; set; } = null!;

    [ForeignKey("PermissionId")]
    [InverseProperty("ForumsForumRoles")]
    public virtual CmsPermission Permission { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("ForumsForumRoles")]
    public virtual CmsRole Role { get; set; } = null!;
}
