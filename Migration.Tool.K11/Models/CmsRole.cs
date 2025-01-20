using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("CMS_Role")]
[Index("RoleGroupId", Name = "IX_CMS_Role_RoleGroupID")]
[Index("SiteId", "RoleId", Name = "IX_CMS_Role_SiteID_RoleID")]
[Index("SiteId", "RoleName", "RoleGroupId", Name = "IX_CMS_Role_SiteID_RoleName_RoleGroupID", IsUnique = true)]
public class CmsRole
{
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

    [Column("RoleGroupID")]
    public int? RoleGroupId { get; set; }

    public bool? RoleIsGroupAdministrator { get; set; }

    public bool? RoleIsDomain { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<CmsAclitem> CmsAclitems { get; set; } = [];

    [InverseProperty("Role")]
    public virtual ICollection<CmsUserRole> CmsUserRoles { get; set; } = [];

    [InverseProperty("Role")]
    public virtual ICollection<CmsWidgetRole> CmsWidgetRoles { get; set; } = [];

    [InverseProperty("Role")]
    public virtual ICollection<CmsWorkflowStepRole> CmsWorkflowStepRoles { get; set; } = [];

    [InverseProperty("Role")]
    public virtual ICollection<CommunityGroupRolePermission> CommunityGroupRolePermissions { get; set; } = [];

    [InverseProperty("Role")]
    public virtual ICollection<ForumsForumRole> ForumsForumRoles { get; set; } = [];

    [InverseProperty("Role")]
    public virtual ICollection<MediaLibraryRolePermission> MediaLibraryRolePermissions { get; set; } = [];

    [ForeignKey("RoleGroupId")]
    [InverseProperty("CmsRoles")]
    public virtual CommunityGroup? RoleGroup { get; set; }

    [ForeignKey("SiteId")]
    [InverseProperty("CmsRoles")]
    public virtual CmsSite? Site { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<BoardBoard> Boards { get; set; } = [];

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<CmsUielement> Elements { get; set; } = [];

    [ForeignKey("RoleId")]
    [InverseProperty("RolesNavigation")]
    public virtual ICollection<CmsUielement> ElementsNavigation { get; set; } = [];

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<CmsForm> Forms { get; set; } = [];

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<CmsMembership> Memberships { get; set; } = [];

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<CmsPermission> Permissions { get; set; } = [];

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<PollsPoll> Polls { get; set; } = [];
}
