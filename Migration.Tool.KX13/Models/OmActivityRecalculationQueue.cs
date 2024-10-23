using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.KX13.Models;

[Table("OM_ActivityRecalculationQueue")]
public class OmActivityRecalculationQueue
{
    [Key]
    [Column("ActivityRecalculationQueueID")]
    public int ActivityRecalculationQueueId { get; set; }

    [Column("ActivityRecalculationQueueActivityID")]
    public int ActivityRecalculationQueueActivityId { get; set; }
}
