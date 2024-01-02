using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_TaxClass")]
[Index("TaxClassSiteId", Name = "IX_COM_TaxClass_TaxClassSiteID")]
public partial class ComTaxClass
{
    [Key]
    [Column("TaxClassID")]
    public int TaxClassId { get; set; }

    [StringLength(200)]
    public string TaxClassName { get; set; } = null!;

    [StringLength(200)]
    public string TaxClassDisplayName { get; set; } = null!;

    [Column("TaxClassZeroIfIDSupplied")]
    public bool? TaxClassZeroIfIdsupplied { get; set; }

    [Column("TaxClassGUID")]
    public Guid TaxClassGuid { get; set; }

    public DateTime TaxClassLastModified { get; set; }

    [Column("TaxClassSiteID")]
    public int? TaxClassSiteId { get; set; }

    [InverseProperty("DepartmentDefaultTaxClass")]
    public virtual ICollection<ComDepartment> ComDepartments { get; set; } = new List<ComDepartment>();

    [InverseProperty("ShippingOptionTaxClass")]
    public virtual ICollection<ComShippingOption> ComShippingOptions { get; set; } = new List<ComShippingOption>();

    [InverseProperty("SkutaxClass")]
    public virtual ICollection<ComSku> ComSkus { get; set; } = new List<ComSku>();

    [InverseProperty("TaxClass")]
    public virtual ICollection<ComTaxClassCountry> ComTaxClassCountries { get; set; } = new List<ComTaxClassCountry>();

    [InverseProperty("TaxClass")]
    public virtual ICollection<ComTaxClassState> ComTaxClassStates { get; set; } = new List<ComTaxClassState>();

    [ForeignKey("TaxClassSiteId")]
    [InverseProperty("ComTaxClasses")]
    public virtual CmsSite? TaxClassSite { get; set; }
}
