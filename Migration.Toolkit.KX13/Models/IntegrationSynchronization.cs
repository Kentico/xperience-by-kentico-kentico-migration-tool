﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Integration_Synchronization")]
[Index("SynchronizationConnectorId", Name = "IX_Integration_Synchronization_SynchronizationConnectorID")]
[Index("SynchronizationTaskId", Name = "IX_Integration_Synchronization_SynchronizationTaskID")]
public partial class IntegrationSynchronization
{
    [Key]
    [Column("SynchronizationID")]
    public int SynchronizationId { get; set; }

    [Column("SynchronizationTaskID")]
    public int SynchronizationTaskId { get; set; }

    [Column("SynchronizationConnectorID")]
    public int SynchronizationConnectorId { get; set; }

    public DateTime SynchronizationLastRun { get; set; }

    public string? SynchronizationErrorMessage { get; set; }

    public bool? SynchronizationIsRunning { get; set; }

    [InverseProperty("SyncLogSynchronization")]
    public virtual ICollection<IntegrationSyncLog> IntegrationSyncLogs { get; set; } = new List<IntegrationSyncLog>();

    [ForeignKey("SynchronizationConnectorId")]
    [InverseProperty("IntegrationSynchronizations")]
    public virtual IntegrationConnector SynchronizationConnector { get; set; } = null!;

    [ForeignKey("SynchronizationTaskId")]
    [InverseProperty("IntegrationSynchronizations")]
    public virtual IntegrationTask SynchronizationTask { get; set; } = null!;
}
