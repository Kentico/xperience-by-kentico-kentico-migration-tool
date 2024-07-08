using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("CMS_BannedIP")]
[Index("IpaddressSiteId", Name = "IX_CMS_BannedIP_IPAddressSiteID")]
public partial class CmsBannedIp
{
    [Key]
    [Column("IPAddressID")]
    public int IpaddressId { get; set; }

    [Column("IPAddress")]
    [StringLength(100)]
    public string Ipaddress { get; set; } = null!;

    [Column("IPAddressRegular")]
    [StringLength(200)]
    public string IpaddressRegular { get; set; } = null!;

    [Column("IPAddressAllowed")]
    public bool IpaddressAllowed { get; set; }

    [Column("IPAddressAllowOverride")]
    public bool IpaddressAllowOverride { get; set; }

    [Column("IPAddressBanReason")]
    [StringLength(450)]
    public string? IpaddressBanReason { get; set; }

    [Column("IPAddressBanType")]
    [StringLength(100)]
    public string IpaddressBanType { get; set; } = null!;

    [Column("IPAddressBanEnabled")]
    public bool? IpaddressBanEnabled { get; set; }

    [Column("IPAddressSiteID")]
    public int? IpaddressSiteId { get; set; }

    [Column("IPAddressGUID")]
    public Guid IpaddressGuid { get; set; }

    [Column("IPAddressLastModified")]
    public DateTime IpaddressLastModified { get; set; }

    [ForeignKey("IpaddressSiteId")]
    [InverseProperty("CmsBannedIps")]
    public virtual CmsSite? IpaddressSite { get; set; }
}