using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("OM_ContactChangeRecalculationQueue")]
public partial class OmContactChangeRecalculationQueue
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
