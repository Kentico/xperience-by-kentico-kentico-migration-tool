using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_Role")]
    public partial class CmsRole
    {
        public CmsRole()
        {
            CmsApplicationPermissions = new HashSet<CmsApplicationPermission>();
            CmsUserRoles = new HashSet<CmsUserRole>();
            CmsWorkflowStepRoles = new HashSet<CmsWorkflowStepRole>();
            Forms = new HashSet<CmsForm>();
        }

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
        public virtual ICollection<CmsApplicationPermission> CmsApplicationPermissions { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<CmsUserRole> CmsUserRoles { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<CmsWorkflowStepRole> CmsWorkflowStepRoles { get; set; }

        [ForeignKey("RoleId")]
        [InverseProperty("Roles")]
        public virtual ICollection<CmsForm> Forms { get; set; }
    }
}
