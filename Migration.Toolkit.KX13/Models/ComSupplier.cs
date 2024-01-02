using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_Supplier")]
[Index("SupplierSiteId", Name = "IX_COM_Supplier_SupplierSiteID")]
public partial class ComSupplier
{
    [Key]
    [Column("SupplierID")]
    public int SupplierId { get; set; }

    [StringLength(200)]
    public string SupplierDisplayName { get; set; } = null!;

    [StringLength(50)]
    public string? SupplierPhone { get; set; }

    [StringLength(254)]
    public string? SupplierEmail { get; set; }

    [StringLength(50)]
    public string? SupplierFax { get; set; }

    [Required]
    public bool? SupplierEnabled { get; set; }

    [Column("SupplierGUID")]
    public Guid SupplierGuid { get; set; }

    public DateTime SupplierLastModified { get; set; }

    [Column("SupplierSiteID")]
    public int? SupplierSiteId { get; set; }

    [StringLength(200)]
    public string? SupplierName { get; set; }

    [InverseProperty("Skusupplier")]
    public virtual ICollection<ComSku> ComSkus { get; set; } = new List<ComSku>();

    [ForeignKey("SupplierSiteId")]
    [InverseProperty("ComSuppliers")]
    public virtual CmsSite? SupplierSite { get; set; }
}
