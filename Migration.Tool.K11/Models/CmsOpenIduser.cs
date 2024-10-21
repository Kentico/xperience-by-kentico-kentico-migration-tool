using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("CMS_OpenIDUser")]
[Index("OpenId", Name = "IX_CMS_OpenIDUser_OpenID")]
[Index("UserId", Name = "IX_CMS_OpenIDUser_UserID")]
public class CmsOpenIduser
{
    [Key]
    [Column("OpenIDUserID")]
    public int OpenIduserId { get; set; }

    [Column("OpenID")]
    public string OpenId { get; set; } = null!;

    [Column("OpenIDProviderURL")]
    [StringLength(450)]
    public string? OpenIdproviderUrl { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("CmsOpenIdusers")]
    public virtual CmsUser User { get; set; } = null!;
}
