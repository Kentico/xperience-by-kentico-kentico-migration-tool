using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_OrderAddress")]
[Index("AddressCountryId", Name = "IX_COM_OrderAddress_AddressCountryID")]
[Index("AddressOrderId", "AddressType", Name = "IX_COM_OrderAddress_AddressOrderID_AddressType", IsUnique = true)]
[Index("AddressStateId", Name = "IX_COM_OrderAddress_AddressStateID")]
public partial class ComOrderAddress
{
    [Key]
    [Column("AddressID")]
    public int AddressId { get; set; }

    [StringLength(100)]
    public string AddressLine1 { get; set; } = null!;

    [StringLength(100)]
    public string? AddressLine2 { get; set; }

    [StringLength(100)]
    public string AddressCity { get; set; } = null!;

    [StringLength(20)]
    public string AddressZip { get; set; } = null!;

    [StringLength(26)]
    public string? AddressPhone { get; set; }

    [Column("AddressCountryID")]
    public int AddressCountryId { get; set; }

    [Column("AddressStateID")]
    public int? AddressStateId { get; set; }

    [StringLength(200)]
    public string AddressPersonalName { get; set; } = null!;

    [Column("AddressGUID")]
    public Guid? AddressGuid { get; set; }

    public DateTime AddressLastModified { get; set; }

    [Column("AddressOrderID")]
    public int AddressOrderId { get; set; }

    public int AddressType { get; set; }

    [ForeignKey("AddressCountryId")]
    [InverseProperty("ComOrderAddresses")]
    public virtual CmsCountry AddressCountry { get; set; } = null!;

    [ForeignKey("AddressOrderId")]
    [InverseProperty("ComOrderAddresses")]
    public virtual ComOrder AddressOrder { get; set; } = null!;

    [ForeignKey("AddressStateId")]
    [InverseProperty("ComOrderAddresses")]
    public virtual CmsState? AddressState { get; set; }
}
