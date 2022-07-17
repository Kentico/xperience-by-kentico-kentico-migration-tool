namespace Migration.Toolkit.KXP.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    [Table("CMS_ModuleUsageCounter")]
    public partial class CmsModuleUsageCounter
    {
        [Column("ModuleUsageCounterID")]
        public int ModuleUsageCounterId { get; set; }
        [StringLength(200)]
        public string ModuleUsageCounterName { get; set; } = null!;
        public long ModuleUsageCounterValue { get; set; }
    }
}
