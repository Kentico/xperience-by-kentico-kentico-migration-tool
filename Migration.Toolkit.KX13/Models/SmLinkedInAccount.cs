using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("SM_LinkedInAccount")]
public partial class SmLinkedInAccount
{
    [Key]
    [Column("LinkedInAccountID")]
    public int LinkedInAccountId { get; set; }

    [StringLength(200)]
    public string LinkedInAccountDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string LinkedInAccountName { get; set; } = null!;

    public bool? LinkedInAccountIsDefault { get; set; }

    [StringLength(500)]
    public string LinkedInAccountAccessToken { get; set; } = null!;

    public DateTime LinkedInAccountLastModified { get; set; }

    [Column("LinkedInAccountGUID")]
    public Guid LinkedInAccountGuid { get; set; }

    [Column("LinkedInAccountSiteID")]
    public int LinkedInAccountSiteId { get; set; }

    [Column("LinkedInAccountProfileID")]
    [StringLength(50)]
    public string LinkedInAccountProfileId { get; set; } = null!;

    [Column("LinkedInAccountLinkedInApplicationID")]
    public int LinkedInAccountLinkedInApplicationId { get; set; }

    [StringLength(200)]
    public string? LinkedInAccountProfileName { get; set; }

    public DateTime? LinkedInAccountAccessTokenExpiration { get; set; }

    [InverseProperty("LinkedInPostLinkedInAccount")]
    public virtual ICollection<SmLinkedInPost> SmLinkedInPosts { get; set; } = new List<SmLinkedInPost>();
}
