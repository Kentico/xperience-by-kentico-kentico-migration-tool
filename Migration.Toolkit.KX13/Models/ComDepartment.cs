using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_Department")]
[Index("DepartmentDefaultTaxClassId", Name = "IX_COM_Department_DepartmentDefaultTaxClassID")]
[Index("DepartmentDisplayName", Name = "IX_COM_Department_DepartmentDisplayName")]
[Index("DepartmentName", "DepartmentSiteId", Name = "IX_COM_Department_DepartmentName_DepartmentSiteID", IsUnique = true)]
[Index("DepartmentSiteId", Name = "IX_COM_Department_DepartmentSiteID")]
public partial class ComDepartment
{
    [Key]
    [Column("DepartmentID")]
    public int DepartmentId { get; set; }

    [StringLength(200)]
    public string DepartmentName { get; set; } = null!;

    [StringLength(200)]
    public string DepartmentDisplayName { get; set; } = null!;

    [Column("DepartmentDefaultTaxClassID")]
    public int? DepartmentDefaultTaxClassId { get; set; }

    [Column("DepartmentGUID")]
    public Guid DepartmentGuid { get; set; }

    public DateTime DepartmentLastModified { get; set; }

    [Column("DepartmentSiteID")]
    public int? DepartmentSiteId { get; set; }

    [InverseProperty("Skudepartment")]
    public virtual ICollection<ComSku> ComSkus { get; set; } = new List<ComSku>();

    [ForeignKey("DepartmentDefaultTaxClassId")]
    [InverseProperty("ComDepartments")]
    public virtual ComTaxClass? DepartmentDefaultTaxClass { get; set; }

    [ForeignKey("DepartmentSiteId")]
    [InverseProperty("ComDepartments")]
    public virtual CmsSite? DepartmentSite { get; set; }

    [ForeignKey("DepartmentId")]
    [InverseProperty("Departments")]
    public virtual ICollection<ComMultiBuyDiscount> MultiBuyDiscounts { get; set; } = new List<ComMultiBuyDiscount>();
}
