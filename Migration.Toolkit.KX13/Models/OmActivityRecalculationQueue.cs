using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("OM_ActivityRecalculationQueue")]
    public partial class OmActivityRecalculationQueue
    {
        [Key]
        [Column("ActivityRecalculationQueueID")]
        public int ActivityRecalculationQueueId { get; set; }
        [Column("ActivityRecalculationQueueActivityID")]
        public int ActivityRecalculationQueueActivityId { get; set; }
    }
}
