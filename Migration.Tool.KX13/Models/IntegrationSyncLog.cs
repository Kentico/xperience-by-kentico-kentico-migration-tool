using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX13.Models;

[Table("Integration_SyncLog")]
[Index("SyncLogSynchronizationId", Name = "IX_Integration_SyncLog_SyncLogTaskID")]
public class IntegrationSyncLog
{
    [Key]
    [Column("SyncLogID")]
    public int SyncLogId { get; set; }

    [Column("SyncLogSynchronizationID")]
    public int SyncLogSynchronizationId { get; set; }

    public DateTime SyncLogTime { get; set; }

    public string? SyncLogErrorMessage { get; set; }

    [ForeignKey("SyncLogSynchronizationId")]
    [InverseProperty("IntegrationSyncLogs")]
    public virtual IntegrationSynchronization SyncLogSynchronization { get; set; } = null!;
}
