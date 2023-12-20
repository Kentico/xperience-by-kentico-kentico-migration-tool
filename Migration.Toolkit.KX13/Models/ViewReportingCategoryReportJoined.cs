using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Keyless]
public partial class ViewReportingCategoryReportJoined
{
    [Column("ObjectID")]
    public int ObjectId { get; set; }

    [StringLength(200)]
    public string CodeName { get; set; } = null!;

    [StringLength(440)]
    public string DisplayName { get; set; } = null!;

    [Column("ParentID")]
    public int? ParentId { get; set; }

    [Column("GUID")]
    public Guid Guid { get; set; }

    public DateTime LastModified { get; set; }

    [StringLength(450)]
    public string? CategoryImagePath { get; set; }

    [StringLength(651)]
    public string? ObjectPath { get; set; }

    public int? ObjectLevel { get; set; }

    public int? CategoryChildCount { get; set; }

    public int? CategoryReportChildCount { get; set; }

    public int? CompleteChildCount { get; set; }

    public string? ReportLayout { get; set; }

    public string? ReportParameters { get; set; }

    public int? ReportAccess { get; set; }

    [StringLength(14)]
    [Unicode(false)]
    public string ObjectType { get; set; } = null!;
}
