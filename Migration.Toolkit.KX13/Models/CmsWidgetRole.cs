using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[PrimaryKey("WidgetId", "RoleId", "PermissionId")]
[Table("CMS_WidgetRole")]
[Index("PermissionId", Name = "IX_CMS_WidgetRole_PermissionID")]
[Index("RoleId", Name = "IX_CMS_WidgetRole_RoleID")]
public partial class CmsWidgetRole
{
    [Key]
    [Column("WidgetID")]
    public int WidgetId { get; set; }

    [Key]
    [Column("RoleID")]
    public int RoleId { get; set; }

    [Key]
    [Column("PermissionID")]
    public int PermissionId { get; set; }

    [ForeignKey("PermissionId")]
    [InverseProperty("CmsWidgetRoles")]
    public virtual CmsPermission Permission { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("CmsWidgetRoles")]
    public virtual CmsRole Role { get; set; } = null!;

    [ForeignKey("WidgetId")]
    [InverseProperty("CmsWidgetRoles")]
    public virtual CmsWidget Widget { get; set; } = null!;
}
