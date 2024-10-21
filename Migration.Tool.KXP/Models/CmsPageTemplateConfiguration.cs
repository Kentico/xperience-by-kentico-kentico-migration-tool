﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.KXP.Models;

[Table("CMS_PageTemplateConfiguration")]
public class CmsPageTemplateConfiguration
{
    [Key]
    [Column("PageTemplateConfigurationID")]
    public int PageTemplateConfigurationId { get; set; }

    [Column("PageTemplateConfigurationGUID")]
    public Guid PageTemplateConfigurationGuid { get; set; }

    public DateTime PageTemplateConfigurationLastModified { get; set; }

    [StringLength(200)]
    public string PageTemplateConfigurationName { get; set; } = null!;

    public string? PageTemplateConfigurationDescription { get; set; }

    public string PageTemplateConfigurationTemplate { get; set; } = null!;

    public string? PageTemplateConfigurationWidgets { get; set; }

    [StringLength(200)]
    public string PageTemplateConfigurationIcon { get; set; } = null!;
}
