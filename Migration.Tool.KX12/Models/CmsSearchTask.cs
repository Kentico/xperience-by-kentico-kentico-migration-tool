using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.KX12.Models;

[Table("CMS_SearchTask")]
public class CmsSearchTask
{
    [Key]
    [Column("SearchTaskID")]
    public int SearchTaskId { get; set; }

    [StringLength(100)]
    public string SearchTaskType { get; set; } = null!;

    [StringLength(100)]
    public string? SearchTaskObjectType { get; set; }

    [StringLength(200)]
    public string? SearchTaskField { get; set; }

    [StringLength(600)]
    public string SearchTaskValue { get; set; } = null!;

    [StringLength(200)]
    public string? SearchTaskServerName { get; set; }

    [StringLength(100)]
    public string SearchTaskStatus { get; set; } = null!;

    public int SearchTaskPriority { get; set; }

    public DateTime SearchTaskCreated { get; set; }

    public string? SearchTaskErrorMessage { get; set; }

    [Column("SearchTaskRelatedObjectID")]
    public int? SearchTaskRelatedObjectId { get; set; }
}
