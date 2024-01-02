using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("SM_TwitterApplication")]
[Index("TwitterApplicationSiteId", Name = "IX_SM_TwitterApplication_TwitterApplicationSiteID")]
public partial class SmTwitterApplication
{
    [Key]
    [Column("TwitterApplicationID")]
    public int TwitterApplicationId { get; set; }

    [StringLength(200)]
    public string TwitterApplicationDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string TwitterApplicationName { get; set; } = null!;

    public DateTime TwitterApplicationLastModified { get; set; }

    [Column("TwitterApplicationGUID")]
    public Guid TwitterApplicationGuid { get; set; }

    [Column("TwitterApplicationSiteID")]
    public int TwitterApplicationSiteId { get; set; }

    [StringLength(500)]
    public string TwitterApplicationConsumerKey { get; set; } = null!;

    [StringLength(500)]
    public string TwitterApplicationConsumerSecret { get; set; } = null!;

    [InverseProperty("TwitterAccountTwitterApplication")]
    public virtual ICollection<SmTwitterAccount> SmTwitterAccounts { get; set; } = new List<SmTwitterAccount>();

    [ForeignKey("TwitterApplicationSiteId")]
    [InverseProperty("SmTwitterApplications")]
    public virtual CmsSite TwitterApplicationSite { get; set; } = null!;
}
