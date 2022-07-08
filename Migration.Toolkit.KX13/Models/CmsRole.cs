using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models
{
    [Table("CMS_Role")]
    [Index("SiteId", "RoleId", Name = "IX_CMS_Role_SiteID_RoleID")]
    public partial class CmsRole
    {
        public CmsRole()
        {
            CmsAclitems = new HashSet<CmsAclitem>();
            CmsUserRoles = new HashSet<CmsUserRole>();
            CmsWidgetRoles = new HashSet<CmsWidgetRole>();
            CmsWorkflowStepRoles = new HashSet<CmsWorkflowStepRole>();
            MediaLibraryRolePermissions = new HashSet<MediaLibraryRolePermission>();
            Elements = new HashSet<CmsUielement>();
            ElementsNavigation = new HashSet<CmsUielement>();
            Forms = new HashSet<CmsForm>();
            Memberships = new HashSet<CmsMembership>();
            Permissions = new HashSet<CmsPermission>();
        }

        [Key]
        [Column("RoleID")]
        public int RoleId { get; set; }
        [StringLength(100)]
        public string RoleDisplayName { get; set; } = null!;
        [StringLength(100)]
        public string RoleName { get; set; } = null!;
        public string? RoleDescription { get; set; }
        [Column("SiteID")]
        public int? SiteId { get; set; }
        [Column("RoleGUID")]
        public Guid RoleGuid { get; set; }
        public DateTime RoleLastModified { get; set; }
        public bool? RoleIsDomain { get; set; }

        [ForeignKey("SiteId")]
        [InverseProperty("CmsRoles")]
        public virtual CmsSite? Site { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<CmsAclitem> CmsAclitems { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<CmsUserRole> CmsUserRoles { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<CmsWidgetRole> CmsWidgetRoles { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<CmsWorkflowStepRole> CmsWorkflowStepRoles { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<MediaLibraryRolePermission> MediaLibraryRolePermissions { get; set; }

        [ForeignKey("RoleId")]
        [InverseProperty("Roles")]
        public virtual ICollection<CmsUielement> Elements { get; set; }
        [ForeignKey("RoleId")]
        [InverseProperty("RolesNavigation")]
        public virtual ICollection<CmsUielement> ElementsNavigation { get; set; }
        [ForeignKey("RoleId")]
        [InverseProperty("Roles")]
        public virtual ICollection<CmsForm> Forms { get; set; }
        [ForeignKey("RoleId")]
        [InverseProperty("Roles")]
        public virtual ICollection<CmsMembership> Memberships { get; set; }
        [ForeignKey("RoleId")]
        [InverseProperty("Roles")]
        public virtual ICollection<CmsPermission> Permissions { get; set; }
    }
}
