using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_InternalStatus")]
[Index("InternalStatusSiteId", "InternalStatusDisplayName", "InternalStatusEnabled", Name = "IX_COM_InternalStatus_InternalStatusSiteID_InternalStatusDisplayName_InternalStatusEnabled")]
public partial class ComInternalStatus
{
    [Key]
    [Column("InternalStatusID")]
    public int InternalStatusId { get; set; }

    [StringLength(200)]
    public string InternalStatusName { get; set; } = null!;

    [StringLength(200)]
    public string InternalStatusDisplayName { get; set; } = null!;

    [Required]
    public bool? InternalStatusEnabled { get; set; }

    [Column("InternalStatusGUID")]
    public Guid InternalStatusGuid { get; set; }

    public DateTime InternalStatusLastModified { get; set; }

    [Column("InternalStatusSiteID")]
    public int? InternalStatusSiteId { get; set; }

    [InverseProperty("SkuinternalStatus")]
    public virtual ICollection<ComSku> ComSkus { get; set; } = new List<ComSku>();

    [ForeignKey("InternalStatusSiteId")]
    [InverseProperty("ComInternalStatuses")]
    public virtual CmsSite? InternalStatusSite { get; set; }
}
