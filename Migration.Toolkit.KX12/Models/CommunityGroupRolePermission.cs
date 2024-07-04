using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[PrimaryKey("GroupId", "RoleId", "PermissionId")]
[Table("Community_GroupRolePermission")]
[Index("PermissionId", Name = "IX_Community_GroupRolePermission_PermissionID")]
[Index("RoleId", Name = "IX_Community_GroupRolePermission_RoleID")]
public partial class CommunityGroupRolePermission
{
    [Key]
    [Column("GroupID")]
    public int GroupId { get; set; }

    [Key]
    [Column("RoleID")]
    public int RoleId { get; set; }

    [Key]
    [Column("PermissionID")]
    public int PermissionId { get; set; }

    [ForeignKey("GroupId")]
    [InverseProperty("CommunityGroupRolePermissions")]
    public virtual CommunityGroup Group { get; set; } = null!;

    [ForeignKey("PermissionId")]
    [InverseProperty("CommunityGroupRolePermissions")]
    public virtual CmsPermission Permission { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("CommunityGroupRolePermissions")]
    public virtual CmsRole Role { get; set; } = null!;
}