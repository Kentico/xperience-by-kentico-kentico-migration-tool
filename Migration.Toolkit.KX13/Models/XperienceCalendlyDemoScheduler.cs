using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_CalendlyDemoScheduler")]
    public partial class XperienceCalendlyDemoScheduler
    {
        [Key]
        [Column("CalendlyDemoSchedulerID")]
        public int CalendlyDemoSchedulerId { get; set; }
        [StringLength(200)]
        public string? Name { get; set; }
    }
}
