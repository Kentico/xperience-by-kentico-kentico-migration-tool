using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Keyless]
[Table("CMS_ModuleUsageCounter")]
public class CmsModuleUsageCounter
{
    [Column("ModuleUsageCounterID")]
    public int ModuleUsageCounterId { get; set; }

    [StringLength(200)]
    public string ModuleUsageCounterName { get; set; } = null!;

    public long ModuleUsageCounterValue { get; set; }
}
