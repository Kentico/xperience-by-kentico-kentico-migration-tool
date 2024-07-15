using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CONTENT_BookingEvent")]
public partial class ContentBookingEvent
{
    [Key]
    [Column("BookingEventID")]
    public int BookingEventId { get; set; }

    [StringLength(200)]
    public string EventName { get; set; } = null!;

    public string? EventSummary { get; set; }

    public string? EventDetails { get; set; }

    public string? EventLocation { get; set; }

    public DateTime? EventDate { get; set; }

    public DateTime? EventEndDate { get; set; }

    public bool? EventAllDay { get; set; }

    public int? EventCapacity { get; set; }

    public bool? EventAllowRegistrationOverCapacity { get; set; }

    public DateTime? EventOpenFrom { get; set; }

    public DateTime? EventOpenTo { get; set; }

    public bool? EventLogActivity { get; set; }
}