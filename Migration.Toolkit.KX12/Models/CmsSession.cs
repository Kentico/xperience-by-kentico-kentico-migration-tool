using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("CMS_Session")]
[Index("SessionIdentificator", Name = "IX_CMS_Session_SessionIdentificator", IsUnique = true)]
[Index("SessionSiteId", Name = "IX_CMS_Session_SessionSiteID")]
[Index("SessionUserId", Name = "IX_CMS_Session_SessionUserID")]
[Index("SessionUserIsHidden", Name = "IX_CMS_Session_SessionUserIsHidden")]
public partial class CmsSession
{
    [StringLength(50)]
    public string SessionIdentificator { get; set; } = null!;

    [Column("SessionUserID")]
    public int? SessionUserId { get; set; }

    [StringLength(450)]
    public string? SessionLocation { get; set; }

    public DateTime SessionLastActive { get; set; }

    public DateTime? SessionLastLogon { get; set; }

    public DateTime SessionExpires { get; set; }

    public bool SessionExpired { get; set; }

    [Column("SessionSiteID")]
    public int? SessionSiteId { get; set; }

    public bool SessionUserIsHidden { get; set; }

    [StringLength(450)]
    public string? SessionFullName { get; set; }

    [StringLength(254)]
    public string? SessionEmail { get; set; }

    [StringLength(254)]
    public string? SessionUserName { get; set; }

    [StringLength(254)]
    public string? SessionNickName { get; set; }

    public DateTime? SessionUserCreated { get; set; }

    [Column("SessionContactID")]
    public int? SessionContactId { get; set; }

    [Key]
    [Column("SessionID")]
    public int SessionId { get; set; }

    [ForeignKey("SessionSiteId")]
    [InverseProperty("CmsSessions")]
    public virtual CmsSite? SessionSite { get; set; }

    [ForeignKey("SessionUserId")]
    [InverseProperty("CmsSessions")]
    public virtual CmsUser? SessionUser { get; set; }
}