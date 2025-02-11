using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[Table("Reporting_ReportCategory")]
[Index("CategoryParentId", Name = "IX_Reporting_ReportCategory_CategoryParentID")]
public class ReportingReportCategory
{
    [Key]
    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [StringLength(200)]
    public string CategoryDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string CategoryCodeName { get; set; } = null!;

    [Column("CategoryGUID")]
    public Guid CategoryGuid { get; set; }

    public DateTime CategoryLastModified { get; set; }

    [Column("CategoryParentID")]
    public int? CategoryParentId { get; set; }

    [StringLength(450)]
    public string? CategoryImagePath { get; set; }

    public string CategoryPath { get; set; } = null!;

    public int? CategoryLevel { get; set; }

    public int? CategoryChildCount { get; set; }

    public int? CategoryReportChildCount { get; set; }

    [ForeignKey("CategoryParentId")]
    [InverseProperty("InverseCategoryParent")]
    public virtual ReportingReportCategory? CategoryParent { get; set; }

    [InverseProperty("CategoryParent")]
    public virtual ICollection<ReportingReportCategory> InverseCategoryParent { get; set; } = [];

    [InverseProperty("ReportCategory")]
    public virtual ICollection<ReportingReport> ReportingReports { get; set; } = [];
}
