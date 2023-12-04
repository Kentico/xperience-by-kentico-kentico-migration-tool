using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("SM_FacebookApplication")]
[Index("FacebookApplicationSiteId", Name = "IX_SM_FacebookApplication_FacebookApplicationSiteID")]
public partial class SmFacebookApplication
{
    [Key]
    [Column("FacebookApplicationID")]
    public int FacebookApplicationId { get; set; }

    [StringLength(500)]
    public string FacebookApplicationConsumerKey { get; set; } = null!;

    [StringLength(500)]
    public string FacebookApplicationConsumerSecret { get; set; } = null!;

    [StringLength(200)]
    public string FacebookApplicationName { get; set; } = null!;

    [StringLength(200)]
    public string FacebookApplicationDisplayName { get; set; } = null!;

    [Column("FacebookApplicationGUID")]
    public Guid FacebookApplicationGuid { get; set; }

    public DateTime FacebookApplicationLastModified { get; set; }

    [Column("FacebookApplicationSiteID")]
    public int FacebookApplicationSiteId { get; set; }

    [ForeignKey("FacebookApplicationSiteId")]
    [InverseProperty("SmFacebookApplications")]
    public virtual CmsSite FacebookApplicationSite { get; set; } = null!;

    [InverseProperty("FacebookAccountFacebookApplication")]
    public virtual ICollection<SmFacebookAccount> SmFacebookAccounts { get; set; } = new List<SmFacebookAccount>();
}
