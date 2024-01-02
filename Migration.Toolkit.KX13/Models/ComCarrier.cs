using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_Carrier")]
[Index("CarrierSiteId", Name = "IX_COM_Carrier_CarrierSiteID")]
public partial class ComCarrier
{
    [Key]
    [Column("CarrierID")]
    public int CarrierId { get; set; }

    [StringLength(200)]
    public string CarrierDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string CarrierName { get; set; } = null!;

    [Column("CarrierSiteID")]
    public int CarrierSiteId { get; set; }

    [Column("CarrierGUID")]
    public Guid CarrierGuid { get; set; }

    [StringLength(200)]
    public string CarrierAssemblyName { get; set; } = null!;

    [StringLength(200)]
    public string CarrierClassName { get; set; } = null!;

    public DateTime CarrierLastModified { get; set; }

    [ForeignKey("CarrierSiteId")]
    [InverseProperty("ComCarriers")]
    public virtual CmsSite CarrierSite { get; set; } = null!;

    [InverseProperty("ShippingOptionCarrier")]
    public virtual ICollection<ComShippingOption> ComShippingOptions { get; set; } = new List<ComShippingOption>();
}
