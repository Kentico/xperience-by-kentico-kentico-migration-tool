using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Keyless]
public partial class ViewCmsUserSettingsRoleJoined
{
    [Column("UserID")]
    public int UserId { get; set; }

    [Column("RoleID")]
    public int RoleId { get; set; }

    [StringLength(100)]
    public string UserName { get; set; } = null!;

    [StringLength(450)]
    public string? FullName { get; set; }

    [StringLength(254)]
    public string? Email { get; set; }

    [StringLength(100)]
    public string RoleName { get; set; } = null!;

    [StringLength(100)]
    public string RoleDisplayName { get; set; } = null!;

    public string? RoleDescription { get; set; }

    [Column("SiteID")]
    public int SiteId { get; set; }

    [StringLength(100)]
    public string SiteName { get; set; } = null!;

    public bool UserEnabled { get; set; }
}
