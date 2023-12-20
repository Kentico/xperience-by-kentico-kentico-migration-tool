using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_PublicStatus")]
[Index("PublicStatusSiteId", Name = "IX_COM_PublicStatus_PublicStatusSiteID")]
public partial class ComPublicStatus
{
    [Key]
    [Column("PublicStatusID")]
    public int PublicStatusId { get; set; }

    [StringLength(200)]
    public string PublicStatusName { get; set; } = null!;

    [StringLength(200)]
    public string PublicStatusDisplayName { get; set; } = null!;

    [Required]
    public bool? PublicStatusEnabled { get; set; }

    [Column("PublicStatusGUID")]
    public Guid? PublicStatusGuid { get; set; }

    public DateTime PublicStatusLastModified { get; set; }

    [Column("PublicStatusSiteID")]
    public int? PublicStatusSiteId { get; set; }

    [InverseProperty("SkupublicStatus")]
    public virtual ICollection<ComSku> ComSkus { get; set; } = new List<ComSku>();

    [ForeignKey("PublicStatusSiteId")]
    [InverseProperty("ComPublicStatuses")]
    public virtual CmsSite? PublicStatusSite { get; set; }
}
