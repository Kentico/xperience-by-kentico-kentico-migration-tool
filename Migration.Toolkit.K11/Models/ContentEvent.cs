using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CONTENT_Event")]
public partial class ContentEvent
{
    [Key]
    [Column("EventID")]
    public int EventId { get; set; }

    [StringLength(200)]
    public string EventName { get; set; } = null!;

    public string? EventSummary { get; set; }

    public string? EventDetails { get; set; }

    public string? EventLocation { get; set; }

    public DateTime? EventDate { get; set; }
}