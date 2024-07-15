using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models;

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

    [Column("ResourceGUID")]
    public Guid ResourceGuid { get; set; }

    public DateTime ResourceLastModified { get; set; }

    public bool? ResourceIsInDevelopment { get; set; }

    [InverseProperty("ClassResource")]
    public virtual ICollection<CmsClass> CmsClasses { get; set; } = new List<CmsClass>();

    [InverseProperty("CategoryResource")]
    public virtual ICollection<CmsSettingsCategory> CmsSettingsCategories { get; set; } = new List<CmsSettingsCategory>();

    [InverseProperty("ActionResource")]
    public virtual ICollection<CmsWorkflowAction> CmsWorkflowActions { get; set; } = new List<CmsWorkflowAction>();
}