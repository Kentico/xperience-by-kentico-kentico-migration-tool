using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("CMS_SiteDomainAlias")]
[Index("SiteDomainAliasName", Name = "IX_CMS_SiteDomainAlias_SiteDomainAliasName")]
[Index("SiteId", Name = "IX_CMS_SiteDomainAlias_SiteID")]
public class CmsSiteDomainAlias
{
    [Key]
    [Column("SiteDomainAliasID")]
    public int SiteDomainAliasId { get; set; }

    [StringLength(400)]
    public string SiteDomainAliasName { get; set; } = null!;

    [Column("SiteID")]
    public int SiteId { get; set; }

    [StringLength(50)]
    public string? SiteDefaultVisitorCulture { get; set; }

    [Column("SiteDomainGUID")]
    public Guid? SiteDomainGuid { get; set; }

    public DateTime SiteDomainLastModified { get; set; }

    [StringLength(450)]
    public string? SiteDomainDefaultAliasPath { get; set; }

    [StringLength(450)]
    public string? SiteDomainRedirectUrl { get; set; }

    [ForeignKey("SiteId")]
    [InverseProperty("CmsSiteDomainAliases")]
    public virtual CmsSite Site { get; set; } = null!;
}
