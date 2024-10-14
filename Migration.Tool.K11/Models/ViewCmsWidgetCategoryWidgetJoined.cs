using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Keyless]
public class ViewCmsWidgetCategoryWidgetJoined
{
    [Column("ObjectID")]
    public int ObjectId { get; set; }

    [StringLength(100)]
    public string CodeName { get; set; } = null!;

    [StringLength(100)]
    public string DisplayName { get; set; } = null!;

    [Column("ParentID")]
    public int? ParentId { get; set; }

    [Column("GUID")]
    public Guid Guid { get; set; }

    public DateTime LastModified { get; set; }

    [StringLength(450)]
    public string? WidgetCategoryImagePath { get; set; }

    [StringLength(551)]
    public string? ObjectPath { get; set; }

    public int? ObjectLevel { get; set; }

    public int? WidgetCategoryChildCount { get; set; }

    public int? WidgetCategoryWidgetChildCount { get; set; }

    public int? CompleteChildCount { get; set; }

    [Column("WidgetWebPartID")]
    public int? WidgetWebPartId { get; set; }

    public int WidgetSecurity { get; set; }

    public bool? WidgetForGroup { get; set; }

    public bool? WidgetForInline { get; set; }

    public bool? WidgetForUser { get; set; }

    public bool? WidgetForEditor { get; set; }

    public bool? WidgetForDashboard { get; set; }

    [Column("WidgetGUID")]
    public Guid? WidgetGuid { get; set; }

    [StringLength(14)]
    [Unicode(false)]
    public string ObjectType { get; set; } = null!;
}
