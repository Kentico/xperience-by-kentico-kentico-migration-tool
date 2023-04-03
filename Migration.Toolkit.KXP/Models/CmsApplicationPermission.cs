using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_ApplicationPermission")]
    [Index("RoleId", Name = "IX_CMS_ApplicationPermission_RoleID")]
    [Index("RoleId", "PermissionName", "ApplicationName", Name = "IX_CMS_ApplicationPermission_RoleID_PermissionName_ApplicationName", IsUnique = true)]
    public partial class CmsApplicationPermission
    {
        [Key]
        [Column("ApplicationPermissionID")]
        public int ApplicationPermissionId { get; set; }
        [Column("RoleID")]
        public int RoleId { get; set; }
        [StringLength(100)]
        public string ApplicationName { get; set; } = null!;
        [StringLength(50)]
        public string PermissionName { get; set; } = null!;
        public Guid ApplicationPermissionGuid { get; set; }

        [ForeignKey("RoleId")]
        [InverseProperty("CmsApplicationPermissions")]
        public virtual CmsRole Role { get; set; } = null!;
    }
}
