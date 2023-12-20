using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_SearchEngine")]
public partial class CmsSearchEngine
{
    [Key]
    [Column("SearchEngineID")]
    public int SearchEngineId { get; set; }

    [StringLength(200)]
    public string SearchEngineDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string SearchEngineName { get; set; } = null!;

    [StringLength(450)]
    public string SearchEngineDomainRule { get; set; } = null!;

    [StringLength(200)]
    public string? SearchEngineKeywordParameter { get; set; }

    [Column("SearchEngineGUID")]
    public Guid SearchEngineGuid { get; set; }

    public DateTime SearchEngineLastModified { get; set; }

    [StringLength(200)]
    public string? SearchEngineCrawler { get; set; }
}
