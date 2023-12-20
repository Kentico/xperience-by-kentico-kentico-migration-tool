using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Keyless]
public partial class ViewCmsPageTemplateCategoryPageTemplateJoined
{
    [Column("ObjectID")]
    public int ObjectId { get; set; }

    [StringLength(200)]
    public string? CodeName { get; set; }

    [StringLength(200)]
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

    public int? CategoryTemplateChildCount { get; set; }

    public int? CompleteChildCount { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string ObjectType { get; set; } = null!;

    [StringLength(10)]
    public string? PageTemplateType { get; set; }

    [StringLength(200)]
    public string? PageTemplateIconClass { get; set; }
}
