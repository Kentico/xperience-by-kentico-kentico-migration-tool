using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KXP.Models;

[Table("OM_ContactChangeRecalculationQueue")]
public class OmContactChangeRecalculationQueue
{
    [Key]
    [Column("ContactChangeRecalculationQueueID")]
    public int ContactChangeRecalculationQueueId { get; set; }

    [Column("ContactChangeRecalculationQueueContactID")]
    public int ContactChangeRecalculationQueueContactId { get; set; }

    public string? ContactChangeRecalculationQueueChangedColumns { get; set; }

    public bool ContactChangeRecalculationQueueContactIsNew { get; set; }

    public bool ContactChangeRecalculationQueueContactWasMerged { get; set; }
}
