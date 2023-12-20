using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("SM_FacebookAccount")]
[Index("FacebookAccountFacebookApplicationId", Name = "IX_SM_FacebookAccount_FacebookAccountFacebookApplicationID")]
[Index("FacebookAccountSiteId", Name = "IX_SM_FacebookAccount_FacebookAccountSiteID")]
public partial class SmFacebookAccount
{
    [Key]
    [Column("FacebookAccountID")]
    public int FacebookAccountId { get; set; }

    [Column("FacebookAccountGUID")]
    public Guid FacebookAccountGuid { get; set; }

    public DateTime FacebookAccountLastModified { get; set; }

    [Column("FacebookAccountSiteID")]
    public int FacebookAccountSiteId { get; set; }

    [StringLength(200)]
    public string FacebookAccountName { get; set; } = null!;

    [StringLength(200)]
    public string FacebookAccountDisplayName { get; set; } = null!;

    [Column("FacebookAccountPageID")]
    [StringLength(500)]
    public string FacebookAccountPageId { get; set; } = null!;

    public string FacebookAccountPageAccessToken { get; set; } = null!;

    [Column("FacebookAccountFacebookApplicationID")]
    public int FacebookAccountFacebookApplicationId { get; set; }

    public DateTime? FacebookAccountPageAccessTokenExpiration { get; set; }

    public bool? FacebookAccountIsDefault { get; set; }

    [ForeignKey("FacebookAccountFacebookApplicationId")]
    [InverseProperty("SmFacebookAccounts")]
    public virtual SmFacebookApplication FacebookAccountFacebookApplication { get; set; } = null!;

    [ForeignKey("FacebookAccountSiteId")]
    [InverseProperty("SmFacebookAccounts")]
    public virtual CmsSite FacebookAccountSite { get; set; } = null!;

    [InverseProperty("FacebookPostFacebookAccount")]
    public virtual ICollection<SmFacebookPost> SmFacebookPosts { get; set; } = new List<SmFacebookPost>();
}
