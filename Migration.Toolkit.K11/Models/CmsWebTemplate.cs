using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CMS_WebTemplate")]
public partial class CmsWebTemplate
{
    [Key]
    [Column("WebTemplateID")]
    public int WebTemplateId { get; set; }

    [StringLength(200)]
    public string WebTemplateDisplayName { get; set; } = null!;

    [StringLength(100)]
    public string WebTemplateFileName { get; set; } = null!;

    public string WebTemplateDescription { get; set; } = null!;

    [Column("WebTemplateGUID")]
    public Guid WebTemplateGuid { get; set; }

    public DateTime WebTemplateLastModified { get; set; }

    [StringLength(100)]
    public string WebTemplateName { get; set; } = null!;

    public int WebTemplateOrder { get; set; }

    [StringLength(200)]
    public string WebTemplateLicenses { get; set; } = null!;

    [Column("WebTemplateThumbnailGUID")]
    public Guid? WebTemplateThumbnailGuid { get; set; }

    public string? WebTemplateShortDescription { get; set; }
}