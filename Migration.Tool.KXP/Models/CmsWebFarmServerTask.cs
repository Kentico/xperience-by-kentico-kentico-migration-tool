using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KXP.Models;

[PrimaryKey("ServerId", "TaskId")]
[Table("CMS_WebFarmServerTask")]
[Index("TaskId", Name = "IX_CMS_WebFarmServerTask_TaskID")]
public class CmsWebFarmServerTask
{
    [Key]
    [Column("ServerID")]
    public int ServerId { get; set; }

    [Key]
    [Column("TaskID")]
    public int TaskId { get; set; }

    public string? ErrorMessage { get; set; }

    [ForeignKey("ServerId")]
    [InverseProperty("CmsWebFarmServerTasks")]
    public virtual CmsWebFarmServer Server { get; set; } = null!;

    [ForeignKey("TaskId")]
    [InverseProperty("CmsWebFarmServerTasks")]
    public virtual CmsWebFarmTask Task { get; set; } = null!;
}
