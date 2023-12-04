using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("SM_LinkedInApplication")]
[Index("LinkedInApplicationSiteId", Name = "IX_SM_LinkedInApplication_LinkedInApplicationSiteID")]
public partial class SmLinkedInApplication
{
    [Key]
    [Column("LinkedInApplicationID")]
    public int LinkedInApplicationId { get; set; }

    [StringLength(200)]
    public string LinkedInApplicationDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string LinkedInApplicationName { get; set; } = null!;

    [StringLength(500)]
    public string LinkedInApplicationConsumerSecret { get; set; } = null!;

    [StringLength(500)]
    public string LinkedInApplicationConsumerKey { get; set; } = null!;

    public DateTime LinkedInApplicationLastModified { get; set; }

    [Column("LinkedInApplicationGUID")]
    public Guid LinkedInApplicationGuid { get; set; }

    [Column("LinkedInApplicationSiteID")]
    public int LinkedInApplicationSiteId { get; set; }

    [ForeignKey("LinkedInApplicationSiteId")]
    [InverseProperty("SmLinkedInApplications")]
    public virtual CmsSite LinkedInApplicationSite { get; set; } = null!;
}
