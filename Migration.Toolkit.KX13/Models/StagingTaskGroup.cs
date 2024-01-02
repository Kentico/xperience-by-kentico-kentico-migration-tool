using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("staging_TaskGroup")]
public partial class StagingTaskGroup
{
    [Key]
    [Column("TaskGroupID")]
    public int TaskGroupId { get; set; }

    [StringLength(50)]
    public string TaskGroupCodeName { get; set; } = null!;

    public Guid TaskGroupGuid { get; set; }

    public string? TaskGroupDescription { get; set; }

    [InverseProperty("TaskGroup")]
    public virtual ICollection<StagingTaskGroupTask> StagingTaskGroupTasks { get; set; } = new List<StagingTaskGroupTask>();

    [InverseProperty("TaskGroup")]
    public virtual ICollection<StagingTaskGroupUser> StagingTaskGroupUsers { get; set; } = new List<StagingTaskGroupUser>();
}
