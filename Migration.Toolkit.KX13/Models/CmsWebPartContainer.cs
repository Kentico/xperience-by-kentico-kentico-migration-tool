using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_WebPartContainer")]
[Index("ContainerName", Name = "IX_CMS_WebPartContainer_ContainerName")]
public partial class CmsWebPartContainer
{
    [Key]
    [Column("ContainerID")]
    public int ContainerId { get; set; }

    [StringLength(200)]
    public string ContainerDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string ContainerName { get; set; } = null!;

    public string? ContainerTextBefore { get; set; }

    public string? ContainerTextAfter { get; set; }

    [Column("ContainerGUID")]
    public Guid ContainerGuid { get; set; }

    public DateTime ContainerLastModified { get; set; }

    [Column("ContainerCSS")]
    public string? ContainerCss { get; set; }

    [ForeignKey("ContainerId")]
    [InverseProperty("Containers")]
    public virtual ICollection<CmsSite> Sites { get; set; } = new List<CmsSite>();
}
