using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("SM_LinkedInAccount")]
public class SmLinkedInAccount
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

    [StringLength(500)]
    public string LinkedInAccountAccessTokenSecret { get; set; } = null!;

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
