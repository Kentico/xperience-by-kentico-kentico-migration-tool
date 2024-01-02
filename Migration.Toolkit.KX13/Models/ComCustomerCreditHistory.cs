using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_CustomerCreditHistory")]
[Index("EventCustomerId", "EventDate", Name = "IX_COM_CustomerCreditHistory_EventCustomerID_EventDate", IsDescending = new[] { false, true })]
[Index("EventSiteId", Name = "IX_COM_CustomerCreditHistory_EventSiteID")]
public partial class ComCustomerCreditHistory
{
    [Key]
    [Column("EventID")]
    public int EventId { get; set; }

    [StringLength(200)]
    public string EventName { get; set; } = null!;

    [Column(TypeName = "decimal(18, 9)")]
    public decimal EventCreditChange { get; set; }

    public DateTime EventDate { get; set; }

    public string? EventDescription { get; set; }

    [Column("EventCustomerID")]
    public int EventCustomerId { get; set; }

    [Column("EventCreditGUID")]
    public Guid? EventCreditGuid { get; set; }

    public DateTime EventCreditLastModified { get; set; }

    [Column("EventSiteID")]
    public int? EventSiteId { get; set; }

    [ForeignKey("EventCustomerId")]
    [InverseProperty("ComCustomerCreditHistories")]
    public virtual ComCustomer EventCustomer { get; set; } = null!;

    [ForeignKey("EventSiteId")]
    [InverseProperty("ComCustomerCreditHistories")]
    public virtual CmsSite? EventSite { get; set; }
}
