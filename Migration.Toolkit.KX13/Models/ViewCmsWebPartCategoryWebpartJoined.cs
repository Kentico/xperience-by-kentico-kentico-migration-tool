using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Keyless]
public partial class ViewCmsWebPartCategoryWebpartJoined
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
    public string? CategoryImagePath { get; set; }

    [StringLength(551)]
    public string? ObjectPath { get; set; }

    public int? ObjectLevel { get; set; }

    public int? CategoryChildCount { get; set; }

    public int? CategoryWebPartChildCount { get; set; }

    public int? CompleteChildCount { get; set; }

    [Column("WebPartParentID")]
    public int? WebPartParentId { get; set; }

    [StringLength(100)]
    public string? WebPartFileName { get; set; }

    [Column("WebPartGUID")]
    public Guid? WebPartGuid { get; set; }

    public int? WebPartType { get; set; }

    [StringLength(1000)]
    public string? WebPartDescription { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string ObjectType { get; set; } = null!;

    [Column("ThumbnailGUID")]
    public Guid? ThumbnailGuid { get; set; }

    [StringLength(200)]
    public string? IconClass { get; set; }

    public bool? WebPartSkipInsertProperties { get; set; }
}
