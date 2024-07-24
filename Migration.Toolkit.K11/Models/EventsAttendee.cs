using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Events_Attendee")]
[Index("AttendeeEventNodeId", Name = "IX_Events_Attendee_AttendeeEventNodeID")]
public class EventsAttendee
{
    [Key]
    [Column("AttendeeID")]
    public int AttendeeId { get; set; }

    [StringLength(254)]
    public string AttendeeEmail { get; set; } = null!;

    [StringLength(100)]
    public string? AttendeeFirstName { get; set; }

    [StringLength(100)]
    public string? AttendeeLastName { get; set; }

    [StringLength(50)]
    public string? AttendeePhone { get; set; }

    [Column("AttendeeEventNodeID")]
    public int AttendeeEventNodeId { get; set; }

    [Column("AttendeeGUID")]
    public Guid AttendeeGuid { get; set; }

    public DateTime AttendeeLastModified { get; set; }

    [ForeignKey("AttendeeEventNodeId")]
    [InverseProperty("EventsAttendees")]
    public virtual CmsTree AttendeeEventNode { get; set; } = null!;
}
