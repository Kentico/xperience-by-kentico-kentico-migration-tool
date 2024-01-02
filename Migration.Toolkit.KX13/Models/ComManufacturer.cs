using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_Manufacturer")]
[Index("ManufacturerSiteId", Name = "IX_COM_Manufacturer_ManufacturerSiteID")]
public partial class ComManufacturer
{
    [Key]
    [Column("ManufacturerID")]
    public int ManufacturerId { get; set; }

    [StringLength(200)]
    public string ManufacturerDisplayName { get; set; } = null!;

    [StringLength(400)]
    public string? ManufactureHomepage { get; set; }

    [Required]
    public bool? ManufacturerEnabled { get; set; }

    [Column("ManufacturerGUID")]
    public Guid ManufacturerGuid { get; set; }

    public DateTime ManufacturerLastModified { get; set; }

    [Column("ManufacturerSiteID")]
    public int? ManufacturerSiteId { get; set; }

    [Column("ManufacturerThumbnailGUID")]
    public Guid? ManufacturerThumbnailGuid { get; set; }

    public string? ManufacturerDescription { get; set; }

    [StringLength(200)]
    public string? ManufacturerName { get; set; }

    [InverseProperty("Skumanufacturer")]
    public virtual ICollection<ComSku> ComSkus { get; set; } = new List<ComSku>();

    [ForeignKey("ManufacturerSiteId")]
    [InverseProperty("ComManufacturers")]
    public virtual CmsSite? ManufacturerSite { get; set; }
}
