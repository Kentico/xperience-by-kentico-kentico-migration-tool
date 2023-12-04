using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_Resource")]
[Index("ResourceName", Name = "IX_CMS_Resource_ResourceName")]
public partial class CmsResource
{
    [Key]
    [Column("ResourceID")]
    public int ResourceId { get; set; }

    [StringLength(100)]
    public string ResourceDisplayName { get; set; } = null!;

    [StringLength(100)]
    public string ResourceName { get; set; } = null!;

    public string? ResourceDescription { get; set; }

    public bool? ShowInDevelopment { get; set; }

    [Column("ResourceURL")]
    [StringLength(1000)]
    public string? ResourceUrl { get; set; }

    [Column("ResourceGUID")]
    public Guid ResourceGuid { get; set; }

    public DateTime ResourceLastModified { get; set; }

    public bool? ResourceIsInDevelopment { get; set; }

    public bool? ResourceHasFiles { get; set; }

    [StringLength(200)]
    public string? ResourceVersion { get; set; }

    [StringLength(200)]
    public string? ResourceAuthor { get; set; }

    [StringLength(50)]
    public string? ResourceInstallationState { get; set; }

    [StringLength(50)]
    public string? ResourceInstalledVersion { get; set; }

    [InverseProperty("ClassResource")]
    public virtual ICollection<CmsClass> CmsClasses { get; set; } = new List<CmsClass>();

    [InverseProperty("UserControlResource")]
    public virtual ICollection<CmsFormUserControl> CmsFormUserControls { get; set; } = new List<CmsFormUserControl>();

    [InverseProperty("ModuleLicenseKeyResource")]
    public virtual ICollection<CmsModuleLicenseKey> CmsModuleLicenseKeys { get; set; } = new List<CmsModuleLicenseKey>();

    [InverseProperty("Resource")]
    public virtual ICollection<CmsPermission> CmsPermissions { get; set; } = new List<CmsPermission>();

    [InverseProperty("ResourceLibraryResource")]
    public virtual ICollection<CmsResourceLibrary> CmsResourceLibraries { get; set; } = new List<CmsResourceLibrary>();

    [InverseProperty("TaskResource")]
    public virtual ICollection<CmsScheduledTask> CmsScheduledTasks { get; set; } = new List<CmsScheduledTask>();

    [InverseProperty("CategoryResource")]
    public virtual ICollection<CmsSettingsCategory> CmsSettingsCategories { get; set; } = new List<CmsSettingsCategory>();

    [InverseProperty("ElementResource")]
    public virtual ICollection<CmsUielement> CmsUielements { get; set; } = new List<CmsUielement>();

    [InverseProperty("WebPartResource")]
    public virtual ICollection<CmsWebPart> CmsWebParts { get; set; } = new List<CmsWebPart>();

    [InverseProperty("ActionResource")]
    public virtual ICollection<CmsWorkflowAction> CmsWorkflowActions { get; set; } = new List<CmsWorkflowAction>();

    [ForeignKey("ResourceId")]
    [InverseProperty("Resources")]
    public virtual ICollection<CmsSite> Sites { get; set; } = new List<CmsSite>();
}
