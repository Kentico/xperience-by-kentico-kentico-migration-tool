using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Keyless]
public partial class ViewCmsSiteRoleResourceUielementJoined
{
    [StringLength(100)]
    public string RoleName { get; set; } = null!;

    [Column("RoleID")]
    public int RoleId { get; set; }

    [StringLength(200)]
    public string ElementName { get; set; } = null!;

    [StringLength(100)]
    public string? SiteName { get; set; }

    [StringLength(100)]
    public string ResourceName { get; set; } = null!;

    [Column("RoleSiteID")]
    public int? RoleSiteId { get; set; }
}
