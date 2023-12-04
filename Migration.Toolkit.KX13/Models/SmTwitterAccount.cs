using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("SM_TwitterAccount")]
[Index("TwitterAccountSiteId", Name = "IX_SM_TwitterAccount_TwitterAccountSiteID")]
[Index("TwitterAccountTwitterApplicationId", Name = "IX_SM_TwitterAccount_TwitterAccountTwitterApplicationID")]
public partial class SmTwitterAccount
{
    [Key]
    [Column("TwitterAccountID")]
    public int TwitterAccountId { get; set; }

    [StringLength(200)]
    public string TwitterAccountDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string TwitterAccountName { get; set; } = null!;

    public DateTime TwitterAccountLastModified { get; set; }

    [Column("TwitterAccountGUID")]
    public Guid TwitterAccountGuid { get; set; }

    [Column("TwitterAccountSiteID")]
    public int TwitterAccountSiteId { get; set; }

    [StringLength(500)]
    public string TwitterAccountAccessToken { get; set; } = null!;

    [StringLength(500)]
    public string TwitterAccountAccessTokenSecret { get; set; } = null!;

    [Column("TwitterAccountTwitterApplicationID")]
    public int TwitterAccountTwitterApplicationId { get; set; }

    public int? TwitterAccountFollowers { get; set; }

    public int? TwitterAccountMentions { get; set; }

    [StringLength(40)]
    public string? TwitterAccountMentionsRange { get; set; }

    [Column("TwitterAccountUserID")]
    [StringLength(20)]
    public string? TwitterAccountUserId { get; set; }

    public bool? TwitterAccountIsDefault { get; set; }

    [InverseProperty("TwitterPostTwitterAccount")]
    public virtual ICollection<SmTwitterPost> SmTwitterPosts { get; set; } = new List<SmTwitterPost>();

    [ForeignKey("TwitterAccountSiteId")]
    [InverseProperty("SmTwitterAccounts")]
    public virtual CmsSite TwitterAccountSite { get; set; } = null!;

    [ForeignKey("TwitterAccountTwitterApplicationId")]
    [InverseProperty("SmTwitterAccounts")]
    public virtual SmTwitterApplication TwitterAccountTwitterApplication { get; set; } = null!;
}
